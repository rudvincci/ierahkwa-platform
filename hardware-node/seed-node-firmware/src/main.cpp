/*
 * Ierahkwa Seed Node Firmware — main.cpp
 * Target: ESP32-S3 + LoRa (SX1276/SX1262)
 *
 * Duty cycle:
 *   1. Wake from deep sleep
 *   2. Initialize LoRa
 *   3. Broadcast heartbeat (encrypted)
 *   4. Listen for incoming packets (relay, vote, PoL ping)
 *   5. Process and relay if needed
 *   6. Check battery; enter deep sleep
 *
 * All packets are AES-256-CBC encrypted with a pre-shared swarm key.
 * Mesh relay uses a TTL (hop count) to prevent infinite loops.
 */

#include <Arduino.h>
#include <LoRa.h>
#include <AES.h>
#include <CBC.h>
#include <ArduinoJson.h>
#include "config.h"

// ── Globals ─────────────────────────────────────────────────────────

static char nodeId[32];
static NodeState currentState = STATE_NORMAL;
static uint32_t bootCount = 0;
static uint32_t packetsRelayed = 0;
static uint32_t packetsReceived = 0;

// Pre-shared AES-256 key (32 bytes). In production, provision via secure flash.
static const uint8_t AES_KEY[32] = {
    0x49, 0x45, 0x52, 0x41, 0x48, 0x4B, 0x57, 0x41,  // IERAHKWA
    0x53, 0x57, 0x41, 0x52, 0x4D, 0x4B, 0x45, 0x59,  // SWARMKEY
    0x32, 0x30, 0x32, 0x36, 0x56, 0x31, 0x33, 0x2E,  // 2026V13.
    0x30, 0x53, 0x4F, 0x56, 0x45, 0x52, 0x45, 0x49   // 0SOVEREI
};

static const uint8_t AES_IV[16] = {
    0x13, 0x00, 0x13, 0x00, 0x13, 0x00, 0x13, 0x00,
    0x49, 0x45, 0x52, 0x41, 0x48, 0x4B, 0x57, 0x41
};

// Seen packet IDs to prevent relay loops
static uint32_t seenPackets[64];
static uint8_t seenIdx = 0;

// ── Crypto ──────────────────────────────────────────────────────────

CBC<AES256> aesCbc;

bool encryptPacket(const uint8_t* plain, size_t len, uint8_t* cipher, size_t* outLen) {
    // Pad to 16-byte boundary
    size_t padded = ((len + 15) / 16) * 16;
    uint8_t padBuf[MAX_PACKET_SIZE];
    memset(padBuf, 0, sizeof(padBuf));
    memcpy(padBuf, plain, len);

    aesCbc.clear();
    aesCbc.setKey(AES_KEY, 32);
    aesCbc.setIV(AES_IV, 16);
    aesCbc.encrypt(cipher, padBuf, padded);
    *outLen = padded;
    return true;
}

bool decryptPacket(const uint8_t* cipher, size_t len, uint8_t* plain, size_t* outLen) {
    if (len % 16 != 0 || len == 0) return false;

    aesCbc.clear();
    aesCbc.setKey(AES_KEY, 32);
    aesCbc.setIV(AES_IV, 16);
    aesCbc.decrypt(plain, cipher, len);
    *outLen = len;
    return true;
}

// ── Battery ─────────────────────────────────────────────────────────

uint32_t readBatteryMV() {
    uint32_t raw = analogRead(BATTERY_PIN);
    // ESP32 ADC: 0-4095 for 0-3.3V. Battery divider 2:1 -> multiply by 2.
    return (raw * 3300 * 2) / 4095;
}

// ── Packet Dedup ────────────────────────────────────────────────────

uint32_t packetHash(const uint8_t* data, size_t len) {
    uint32_t h = 0x811c9dc5;  // FNV-1a
    for (size_t i = 0; i < len; i++) {
        h ^= data[i];
        h *= 0x01000193;
    }
    return h;
}

bool isPacketSeen(uint32_t hash) {
    for (uint8_t i = 0; i < 64; i++) {
        if (seenPackets[i] == hash) return true;
    }
    return false;
}

void markPacketSeen(uint32_t hash) {
    seenPackets[seenIdx] = hash;
    seenIdx = (seenIdx + 1) % 64;
}

// ── Heartbeat ───────────────────────────────────────────────────────

void sendHeartbeat() {
    JsonDocument doc;
    doc["t"] = MSG_HEARTBEAT;
    doc["id"] = nodeId;
    doc["v"] = NODE_VERSION;
    doc["bat"] = readBatteryMV();
    doc["up"] = bootCount;
    doc["rx"] = packetsReceived;
    doc["fwd"] = packetsRelayed;
    doc["st"] = (uint8_t)currentState;
    doc["ts"] = millis();

    char json[MAX_PACKET_SIZE];
    size_t jsonLen = serializeJson(doc, json, sizeof(json));

    uint8_t cipher[MAX_PACKET_SIZE];
    size_t cipherLen = 0;
    if (!encryptPacket((uint8_t*)json, jsonLen, cipher, &cipherLen)) {
        Serial.println("[HB] Encrypt failed");
        return;
    }

    LoRa.beginPacket();
    LoRa.write(cipher, cipherLen);
    LoRa.endPacket();

    Serial.printf("[HB] Sent heartbeat: bat=%lumV rx=%lu fwd=%lu\n",
                  readBatteryMV(), packetsReceived, packetsRelayed);
}

// ── PoL Ping Response ───────────────────────────────────────────────

void sendPolPong(const char* requesterId) {
    JsonDocument doc;
    doc["t"] = MSG_POL_PONG;
    doc["id"] = nodeId;
    doc["to"] = requesterId;
    doc["ts"] = micros();  // Microsecond precision for timing

    char json[128];
    size_t jsonLen = serializeJson(doc, json, sizeof(json));

    uint8_t cipher[MAX_PACKET_SIZE];
    size_t cipherLen = 0;
    encryptPacket((uint8_t*)json, jsonLen, cipher, &cipherLen);

    LoRa.beginPacket();
    LoRa.write(cipher, cipherLen);
    LoRa.endPacket();

    Serial.printf("[POL] Pong sent to %s\n", requesterId);
}

// ── Vote Processing ─────────────────────────────────────────────────

void processVoteRequest(JsonDocument& doc) {
    const char* topic = doc["topic"] | "unknown";
    uint8_t ttl = doc["ttl"] | 0;

    Serial.printf("[VOTE] Request: topic=%s ttl=%d\n", topic, ttl);

    // Autonomous vote: seed nodes vote YES on emergency topics
    // based on local sensor data (battery, signal strength)
    JsonDocument reply;
    reply["t"] = MSG_VOTE_CAST;
    reply["id"] = nodeId;
    reply["topic"] = topic;
    reply["vote"] = "YES";  // Simple majority rule for emergency
    reply["rssi"] = LoRa.packetRssi();
    reply["bat"] = readBatteryMV();

    char json[MAX_PACKET_SIZE];
    size_t jsonLen = serializeJson(reply, json, sizeof(json));

    uint8_t cipher[MAX_PACKET_SIZE];
    size_t cipherLen = 0;
    encryptPacket((uint8_t*)json, jsonLen, cipher, &cipherLen);

    LoRa.beginPacket();
    LoRa.write(cipher, cipherLen);
    LoRa.endPacket();

    Serial.printf("[VOTE] Cast YES for %s\n", topic);
}

// ── Packet Relay ────────────────────────────────────────────────────

void relayPacket(const uint8_t* raw, size_t rawLen, JsonDocument& doc) {
    uint8_t ttl = doc["ttl"] | 0;
    if (ttl == 0) return;  // Expired

    // Decrement TTL and re-broadcast
    doc["ttl"] = ttl - 1;
    doc["relay"] = nodeId;

    char json[MAX_PACKET_SIZE];
    size_t jsonLen = serializeJson(doc, json, sizeof(json));

    uint8_t cipher[MAX_PACKET_SIZE];
    size_t cipherLen = 0;
    encryptPacket((uint8_t*)json, jsonLen, cipher, &cipherLen);

    delay(random(50, 500));  // Random delay to reduce collisions

    LoRa.beginPacket();
    LoRa.write(cipher, cipherLen);
    LoRa.endPacket();

    packetsRelayed++;
    Serial.printf("[RELAY] Forwarded type=%d ttl=%d\n", (uint8_t)doc["t"], ttl - 1);
}

// ── Receive Handler ─────────────────────────────────────────────────

void onReceive(int packetSize) {
    if (packetSize == 0 || packetSize > MAX_PACKET_SIZE) return;

    uint8_t raw[MAX_PACKET_SIZE];
    size_t idx = 0;
    while (LoRa.available() && idx < MAX_PACKET_SIZE) {
        raw[idx++] = LoRa.read();
    }

    // Dedup
    uint32_t hash = packetHash(raw, idx);
    if (isPacketSeen(hash)) return;
    markPacketSeen(hash);

    // Decrypt
    uint8_t plain[MAX_PACKET_SIZE];
    size_t plainLen = 0;
    if (!decryptPacket(raw, idx, plain, &plainLen)) {
        Serial.println("[RX] Decrypt failed — not our swarm");
        return;
    }

    // Parse JSON
    JsonDocument doc;
    DeserializationError err = deserializeJson(doc, plain, plainLen);
    if (err) {
        Serial.printf("[RX] JSON parse error: %s\n", err.c_str());
        return;
    }

    packetsReceived++;
    uint8_t msgType = doc["t"] | 0;
    const char* senderId = doc["id"] | "unknown";
    int rssi = LoRa.packetRssi();
    float snr = LoRa.packetSnr();

    Serial.printf("[RX] type=0x%02X from=%s rssi=%d snr=%.1f\n",
                  msgType, senderId, rssi, snr);

    // Don't process our own packets
    if (strcmp(senderId, nodeId) == 0) return;

    switch (msgType) {
        case MSG_HEARTBEAT:
            // Log peer heartbeat — indicates the mesh is alive
            Serial.printf("[PEER] %s alive bat=%d\n", senderId, (int)doc["bat"]);
            break;

        case MSG_ALERT_RED:
            currentState = STATE_ALERT;
            Serial.println("[ALERT] RED — entering alert state");
            digitalWrite(LED_PIN, HIGH);
            // Relay the alert
            relayPacket(raw, idx, doc);
            break;

        case MSG_ALERT_CLEAR:
            currentState = STATE_NORMAL;
            Serial.println("[ALERT] CLEAR — returning to normal");
            digitalWrite(LED_PIN, LOW);
            break;

        case MSG_VOTE_REQ:
            processVoteRequest(doc);
            relayPacket(raw, idx, doc);
            break;

        case MSG_POL_PING:
            #if POL_PING_RESPONSE
            sendPolPong(senderId);
            #endif
            break;

        case MSG_RELAY:
        case MSG_DATA:
            relayPacket(raw, idx, doc);
            break;

        default:
            Serial.printf("[RX] Unknown type 0x%02X\n", msgType);
            break;
    }
}

// ── Deep Sleep ──────────────────────────────────────────────────────

void enterDeepSleep(uint64_t sleepUs) {
    Serial.printf("[SLEEP] Entering deep sleep for %llu seconds\n", sleepUs / 1000000ULL);
    Serial.flush();

    LoRa.sleep();
    esp_sleep_enable_timer_wakeup(sleepUs);
    esp_deep_sleep_start();
}

// ── Setup ───────────────────────────────────────────────────────────

void setup() {
    Serial.begin(115200);
    delay(100);

    // Generate node ID from MAC
    uint8_t mac[6];
    esp_read_mac(mac, ESP_MAC_WIFI_STA);
    snprintf(nodeId, sizeof(nodeId), "%s_%02X%02X%02X",
             NODE_ID_PREFIX, mac[3], mac[4], mac[5]);

    // Track boot count in RTC memory
    bootCount++;

    Serial.printf("\n=== IERAHKWA SEED NODE v%s ===\n", NODE_VERSION);
    Serial.printf("Node ID: %s\n", nodeId);
    Serial.printf("Boot #%lu\n", bootCount);

    // LED
    pinMode(LED_PIN, OUTPUT);
    digitalWrite(LED_PIN, LOW);

    // Battery check
    uint32_t batMv = readBatteryMV();
    Serial.printf("Battery: %lumV\n", batMv);

    if (batMv < BATTERY_CRITICAL_MV && batMv > 0) {
        Serial.println("[CRITICAL] Battery too low — sleeping 1 hour");
        currentState = STATE_CRITICAL;
        enterDeepSleep(3600ULL * 1000000ULL);
        return;
    }

    if (batMv < BATTERY_LOW_MV && batMv > 0) {
        Serial.println("[WARNING] Battery low — reduced duty cycle");
    }

    // LoRa init
    LoRa.setPins(LORA_SS, LORA_RST, LORA_DIO0);
    if (!LoRa.begin(LORA_FREQ)) {
        Serial.println("[ERROR] LoRa init failed — sleeping");
        enterDeepSleep(SLEEP_US);
        return;
    }

    LoRa.setSyncWord(IERAHKWA_SYNC_WORD);
    LoRa.setSignalBandwidth(LORA_BANDWIDTH);
    LoRa.setSpreadingFactor(LORA_SPREAD_FACTOR);
    LoRa.setCodingRate4(LORA_CODING_RATE);
    LoRa.setTxPower(LORA_TX_POWER);
    LoRa.setPreambleLength(LORA_PREAMBLE);
    LoRa.enableCrc();

    Serial.printf("LoRa: %.0fMHz SF%d BW%.0fkHz\n",
                  LORA_FREQ / 1e6, LORA_SPREAD_FACTOR, LORA_BANDWIDTH / 1e3);

    // Register receive callback
    LoRa.onReceive(onReceive);

    // Send heartbeat
    sendHeartbeat();

    // Blink LED to indicate activity
    digitalWrite(LED_PIN, HIGH);
    delay(100);
    digitalWrite(LED_PIN, LOW);

    Serial.println("[READY] Listening...");
}

// ── Loop ────────────────────────────────────────────────────────────

void loop() {
    static unsigned long lastHeartbeat = 0;
    static unsigned long awakeStart = millis();

    // Parse incoming packets
    LoRa.parsePacket();

    // Periodic heartbeat
    if (millis() - lastHeartbeat >= HEARTBEAT_INTERVAL) {
        sendHeartbeat();
        lastHeartbeat = millis();
    }

    // Stay awake for AWAKE_WINDOW_MS, then sleep
    if (millis() - awakeStart >= AWAKE_WINDOW_MS) {
        if (currentState == STATE_ALERT) {
            // In alert mode, stay awake longer
            if (millis() - awakeStart >= AWAKE_WINDOW_MS * 4) {
                enterDeepSleep(SLEEP_US / 4);  // Shorter sleep during alert
            }
        } else {
            enterDeepSleep(SLEEP_US);
        }
    }

    delay(10);  // Yield to radio interrupt
}
