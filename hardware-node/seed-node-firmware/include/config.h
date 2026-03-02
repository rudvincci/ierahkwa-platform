/*
 * Ierahkwa Seed Node — Configuration
 * Hardware: ESP32-S3 + SX1276/SX1262 LoRa
 *
 * This node runs on milliamps, hidden in nature (rocks, trees, walls).
 * It provides: mesh relay, emergency voting, heartbeat pulse, PoL pings.
 */

#ifndef IERAHKWA_CONFIG_H
#define IERAHKWA_CONFIG_H

#include <Arduino.h>

// ── Node Identity ───────────────────────────────────────────────────
// Unique per node; set during first flash or via LoRa config command
#define NODE_ID_PREFIX      "SEED"
#define NODE_VERSION        "13.0.0"
#define MANIFEST_HASH       "IERAHKWA_SWARM_v13"

// ── LoRa Configuration ─────────────────────────────────────────────
#ifndef LORA_FREQ
#define LORA_FREQ           433E6       // 433 MHz (Americas ISM)
#endif

#ifndef IERAHKWA_SYNC_WORD
#define IERAHKWA_SYNC_WORD  0x13        // Sovereign mesh identifier
#endif

#define LORA_BANDWIDTH      125E3       // 125 kHz
#define LORA_SPREAD_FACTOR  10          // SF10: good range, moderate speed
#define LORA_CODING_RATE    5           // 4/5
#define LORA_TX_POWER       17          // dBm (max 20 for SX1276)
#define LORA_PREAMBLE       8

// ── Pin Mapping (per board) ─────────────────────────────────────────
#ifdef BOARD_TBEAM
    #define LORA_SS         18
    #define LORA_RST        23
    #define LORA_DIO0       26
    #define LED_PIN         4
    #define BATTERY_PIN     35
    #define HAS_GPS         1
#elif defined(BOARD_HELTEC_V3)
    #define LORA_SS         8
    #define LORA_RST        12
    #define LORA_DIO0       14
    #define LED_PIN         35
    #define BATTERY_PIN     1
    #define HAS_GPS         0
#else
    #define LORA_SS         5
    #define LORA_RST        14
    #define LORA_DIO0       2
    #define LED_PIN         LED_BUILTIN
    #define BATTERY_PIN     34
    #define HAS_GPS         0
#endif

// ── Power Management ────────────────────────────────────────────────
#ifndef NODE_SLEEP_MINUTES
#define NODE_SLEEP_MINUTES  5           // Deep sleep between duty cycles
#endif

#define SLEEP_US            (NODE_SLEEP_MINUTES * 60ULL * 1000000ULL)
#define AWAKE_WINDOW_MS     30000       // 30 seconds awake per cycle
#define BATTERY_LOW_MV      3200        // Enter extended sleep below this
#define BATTERY_CRITICAL_MV 3000        // Emergency: stop TX, deep sleep 1 hour

// ── Protocol ────────────────────────────────────────────────────────
#define MAX_PACKET_SIZE     200         // LoRa payload limit
#define MAX_HOPS            5           // Mesh relay hop limit
#define HEARTBEAT_INTERVAL  60000       // ms between heartbeat broadcasts
#define VOTE_TIMEOUT_MS     300000      // 5 minutes to cast emergency vote
#define POL_PING_RESPONSE   1           // Respond to Proof-of-Location pings

// ── Encryption ──────────────────────────────────────────────────────
#ifndef AES_KEY_SIZE
#define AES_KEY_SIZE        32          // AES-256
#endif

// ── Message Types ───────────────────────────────────────────────────
enum MsgType : uint8_t {
    MSG_HEARTBEAT   = 0x01,
    MSG_RELAY       = 0x02,
    MSG_VOTE_REQ    = 0x03,
    MSG_VOTE_CAST   = 0x04,
    MSG_ALERT_RED   = 0x05,
    MSG_ALERT_CLEAR = 0x06,
    MSG_POL_PING    = 0x07,
    MSG_POL_PONG    = 0x08,
    MSG_CONFIG      = 0x09,
    MSG_ACK         = 0x0A,
    MSG_DATA        = 0x0B,
};

// ── Node State ──────────────────────────────────────────────────────
enum NodeState : uint8_t {
    STATE_NORMAL    = 0,
    STATE_ALERT     = 1,
    STATE_AUTONOMOUS = 2,   // Network fragmented; local governance active
    STATE_SLEEP     = 3,
    STATE_CRITICAL  = 4,    // Battery critical
};

#endif // IERAHKWA_CONFIG_H
