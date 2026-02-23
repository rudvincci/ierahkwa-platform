/**
 * FutureWampumID Biometric Capture JavaScript Module
 * Provides camera, fingerprint (WebAuthn), and voice capture capabilities
 */
window.BiometricCapture = window.BiometricCapture || {};

(function(BC) {
    'use strict';

    // Track active streams
    let activeVideoStream = null;
    let activeAudioStream = null;
    let mediaRecorder = null;
    let audioChunks = [];

    // ==================== CAMERA / FACE CAPTURE ====================

    /**
     * Initializes camera stream and attaches to video element
     * @param {string} videoElementId - ID of the video element
     * @param {object} options - Camera options (facingMode, width, height)
     * @returns {Promise<boolean>}
     */
    BC.initCamera = async function(videoElementId, options = {}) {
        try {
            const constraints = {
                video: {
                    facingMode: options.facingMode || 'user',
                    width: { ideal: options.width || 640 },
                    height: { ideal: options.height || 480 }
                },
                audio: false
            };

            activeVideoStream = await navigator.mediaDevices.getUserMedia(constraints);
            
            const videoElement = document.getElementById(videoElementId);
            if (videoElement) {
                videoElement.srcObject = activeVideoStream;
                await videoElement.play();
                return true;
            }
            return false;
        } catch (error) {
            console.error('Camera init error:', error);
            throw error;
        }
    };

    /**
     * Stops the active camera stream
     */
    BC.stopCamera = function() {
        if (activeVideoStream) {
            activeVideoStream.getTracks().forEach(track => track.stop());
            activeVideoStream = null;
        }
    };

    /**
     * Captures a frame from the video element as base64
     * @param {string} videoElementId - ID of the video element
     * @param {string} canvasElementId - ID of the canvas element for capture
     * @returns {string} Base64 encoded image
     */
    BC.captureFrame = function(videoElementId, canvasElementId) {
        const video = document.getElementById(videoElementId);
        const canvas = document.getElementById(canvasElementId);
        
        if (!video || !canvas) {
            throw new Error('Video or canvas element not found');
        }

        const context = canvas.getContext('2d');
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        
        // Draw video frame to canvas
        context.drawImage(video, 0, 0);
        
        // Return as base64 JPEG
        return canvas.toDataURL('image/jpeg', 0.9).split(',')[1];
    };

    /**
     * Performs basic face detection quality check
     * @param {string} canvasElementId - ID of the canvas with captured image
     * @returns {object} Quality assessment result
     */
    BC.assessFaceQuality = function(canvasElementId) {
        const canvas = document.getElementById(canvasElementId);
        if (!canvas) {
            return { isValid: false, error: 'Canvas not found' };
        }

        const context = canvas.getContext('2d');
        const imageData = context.getImageData(0, 0, canvas.width, canvas.height);
        const data = imageData.data;

        // Calculate brightness
        let totalBrightness = 0;
        for (let i = 0; i < data.length; i += 4) {
            const brightness = (data[i] + data[i + 1] + data[i + 2]) / 3;
            totalBrightness += brightness;
        }
        const avgBrightness = totalBrightness / (data.length / 4);

        // Calculate contrast (standard deviation of brightness)
        let contrastSum = 0;
        for (let i = 0; i < data.length; i += 4) {
            const brightness = (data[i] + data[i + 1] + data[i + 2]) / 3;
            contrastSum += Math.pow(brightness - avgBrightness, 2);
        }
        const contrast = Math.sqrt(contrastSum / (data.length / 4));

        // Quality assessment
        const isBrightnessOk = avgBrightness > 60 && avgBrightness < 200;
        const isContrastOk = contrast > 30;

        return {
            isValid: isBrightnessOk && isContrastOk,
            brightness: avgBrightness,
            contrast: contrast,
            issues: [
                ...(!isBrightnessOk ? ['Lighting may be too dark or too bright'] : []),
                ...(!isContrastOk ? ['Image may be blurry or low contrast'] : [])
            ]
        };
    };

    /**
     * Detects face position in the frame (simplified bounding box estimation)
     * @param {string} canvasElementId - ID of the canvas
     * @returns {object} Face position info
     */
    BC.detectFacePosition = function(canvasElementId) {
        // This is a simplified estimation - in production, use TensorFlow.js or similar
        const canvas = document.getElementById(canvasElementId);
        if (!canvas) {
            return { detected: false };
        }

        // Return mock centered position for now
        // In production, integrate with face-api.js or TensorFlow.js
        return {
            detected: true,
            centered: true,
            x: canvas.width / 2,
            y: canvas.height / 2,
            width: canvas.width * 0.4,
            height: canvas.height * 0.5
        };
    };

    // ==================== FINGERPRINT (WebAuthn) ====================

    /**
     * Checks if WebAuthn/Fingerprint is available
     * @returns {Promise<boolean>}
     */
    BC.isWebAuthnAvailable = async function() {
        if (!window.PublicKeyCredential) {
            return false;
        }
        
        try {
            const available = await PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable();
            return available;
        } catch {
            return false;
        }
    };

    /**
     * Registers a new WebAuthn credential (fingerprint)
     * @param {object} options - Registration options from server
     * @returns {Promise<object>} Credential response
     */
    BC.registerFingerprint = async function(options) {
        try {
            // Convert base64 challenge to ArrayBuffer
            const publicKeyCredentialCreationOptions = {
                challenge: Uint8Array.from(atob(options.challenge), c => c.charCodeAt(0)),
                rp: {
                    name: options.rpName || 'FutureWampumID',
                    id: options.rpId || window.location.hostname
                },
                user: {
                    id: Uint8Array.from(atob(options.userId), c => c.charCodeAt(0)),
                    name: options.userName,
                    displayName: options.displayName
                },
                pubKeyCredParams: [
                    { type: 'public-key', alg: -7 },  // ES256
                    { type: 'public-key', alg: -257 } // RS256
                ],
                authenticatorSelection: {
                    authenticatorAttachment: 'platform',
                    userVerification: 'required',
                    residentKey: 'preferred'
                },
                timeout: 60000,
                attestation: 'direct'
            };

            const credential = await navigator.credentials.create({
                publicKey: publicKeyCredentialCreationOptions
            });

            // Convert response to base64 for server
            return {
                id: credential.id,
                rawId: btoa(String.fromCharCode(...new Uint8Array(credential.rawId))),
                type: credential.type,
                response: {
                    clientDataJSON: btoa(String.fromCharCode(...new Uint8Array(credential.response.clientDataJSON))),
                    attestationObject: btoa(String.fromCharCode(...new Uint8Array(credential.response.attestationObject)))
                }
            };
        } catch (error) {
            console.error('Fingerprint registration error:', error);
            throw error;
        }
    };

    /**
     * Verifies fingerprint using stored credential
     * @param {object} options - Verification options from server
     * @returns {Promise<object>} Assertion response
     */
    BC.verifyFingerprint = async function(options) {
        try {
            const publicKeyCredentialRequestOptions = {
                challenge: Uint8Array.from(atob(options.challenge), c => c.charCodeAt(0)),
                timeout: 60000,
                rpId: options.rpId || window.location.hostname,
                userVerification: 'required',
                allowCredentials: options.allowCredentials?.map(cred => ({
                    type: 'public-key',
                    id: Uint8Array.from(atob(cred.id), c => c.charCodeAt(0))
                })) || []
            };

            const assertion = await navigator.credentials.get({
                publicKey: publicKeyCredentialRequestOptions
            });

            return {
                id: assertion.id,
                rawId: btoa(String.fromCharCode(...new Uint8Array(assertion.rawId))),
                type: assertion.type,
                response: {
                    clientDataJSON: btoa(String.fromCharCode(...new Uint8Array(assertion.response.clientDataJSON))),
                    authenticatorData: btoa(String.fromCharCode(...new Uint8Array(assertion.response.authenticatorData))),
                    signature: btoa(String.fromCharCode(...new Uint8Array(assertion.response.signature))),
                    userHandle: assertion.response.userHandle 
                        ? btoa(String.fromCharCode(...new Uint8Array(assertion.response.userHandle))) 
                        : null
                }
            };
        } catch (error) {
            console.error('Fingerprint verification error:', error);
            throw error;
        }
    };

    // ==================== VOICE CAPTURE ====================

    /**
     * Starts audio recording for voice capture
     * @returns {Promise<boolean>}
     */
    BC.startVoiceRecording = async function() {
        try {
            activeAudioStream = await navigator.mediaDevices.getUserMedia({ 
                audio: {
                    echoCancellation: true,
                    noiseSuppression: true,
                    sampleRate: 44100
                } 
            });

            audioChunks = [];
            mediaRecorder = new MediaRecorder(activeAudioStream, {
                mimeType: 'audio/webm;codecs=opus'
            });

            mediaRecorder.ondataavailable = (event) => {
                if (event.data.size > 0) {
                    audioChunks.push(event.data);
                }
            };

            mediaRecorder.start(100); // Collect data every 100ms
            return true;
        } catch (error) {
            console.error('Voice recording start error:', error);
            throw error;
        }
    };

    /**
     * Stops voice recording and returns the audio data
     * @returns {Promise<string>} Base64 encoded audio
     */
    BC.stopVoiceRecording = function() {
        return new Promise((resolve, reject) => {
            if (!mediaRecorder) {
                reject(new Error('No active recording'));
                return;
            }

            mediaRecorder.onstop = async () => {
                try {
                    const audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
                    const base64 = await blobToBase64(audioBlob);
                    
                    // Clean up
                    if (activeAudioStream) {
                        activeAudioStream.getTracks().forEach(track => track.stop());
                        activeAudioStream = null;
                    }
                    audioChunks = [];
                    
                    resolve(base64);
                } catch (error) {
                    reject(error);
                }
            };

            mediaRecorder.stop();
        });
    };

    /**
     * Gets current audio level for visualization (0-100)
     * @returns {number}
     */
    BC.getAudioLevel = function() {
        if (!activeAudioStream) return 0;

        // Create analyzer if not exists
        if (!BC._audioContext) {
            BC._audioContext = new (window.AudioContext || window.webkitAudioContext)();
            BC._analyzer = BC._audioContext.createAnalyser();
            BC._analyzer.fftSize = 256;
            
            const source = BC._audioContext.createMediaStreamSource(activeAudioStream);
            source.connect(BC._analyzer);
        }

        const dataArray = new Uint8Array(BC._analyzer.frequencyBinCount);
        BC._analyzer.getByteFrequencyData(dataArray);
        
        const average = dataArray.reduce((a, b) => a + b) / dataArray.length;
        return Math.min(100, Math.round(average / 2.55 * 2));
    };

    /**
     * Assesses voice recording quality
     * @param {string} base64Audio - Base64 encoded audio
     * @returns {object} Quality assessment
     */
    BC.assessVoiceQuality = async function(base64Audio) {
        // In production, this would analyze the audio for:
        // - Duration (should be 3-10 seconds)
        // - Volume levels
        // - Background noise
        // - Speech detection

        // Simplified check based on data size
        const dataSize = base64Audio.length * 0.75; // Approximate decoded size
        const estimatedDuration = dataSize / 5000; // Rough estimate

        return {
            isValid: estimatedDuration >= 2 && estimatedDuration <= 15,
            duration: estimatedDuration,
            issues: [
                ...(estimatedDuration < 2 ? ['Recording too short'] : []),
                ...(estimatedDuration > 15 ? ['Recording too long'] : [])
            ]
        };
    };

    // ==================== LIVENESS DETECTION ====================

    /**
     * Prompts for liveness action and detects completion
     * @param {string} action - 'blink', 'turn_left', 'turn_right', 'smile'
     * @param {string} videoElementId - Video element ID
     * @param {number} timeout - Timeout in ms
     * @returns {Promise<object>} Detection result
     */
    BC.detectLivenessAction = async function(action, videoElementId, timeout = 10000) {
        // In production, integrate with TensorFlow.js face-landmarks-detection
        // For now, simulate detection after a delay
        
        return new Promise((resolve) => {
            const detectionTime = Math.random() * 3000 + 1000; // 1-4 seconds
            
            setTimeout(() => {
                resolve({
                    detected: true,
                    action: action,
                    confidence: 0.85 + Math.random() * 0.15
                });
            }, Math.min(detectionTime, timeout));
        });
    };

    /**
     * Runs a full liveness check sequence
     * @param {string} videoElementId - Video element ID
     * @param {string[]} actions - Array of actions to perform
     * @param {function} onProgress - Progress callback
     * @returns {Promise<object>} Liveness check result
     */
    BC.runLivenessCheck = async function(videoElementId, actions, onProgress) {
        const results = [];
        
        for (let i = 0; i < actions.length; i++) {
            const action = actions[i];
            
            if (onProgress) {
                onProgress({
                    step: i + 1,
                    total: actions.length,
                    action: action,
                    status: 'detecting'
                });
            }
            
            const result = await BC.detectLivenessAction(action, videoElementId);
            results.push(result);
            
            if (onProgress) {
                onProgress({
                    step: i + 1,
                    total: actions.length,
                    action: action,
                    status: result.detected ? 'success' : 'failed'
                });
            }
            
            // Small delay between actions
            await new Promise(r => setTimeout(r, 500));
        }
        
        const allPassed = results.every(r => r.detected);
        const avgConfidence = results.reduce((sum, r) => sum + r.confidence, 0) / results.length;
        
        return {
            passed: allPassed,
            confidence: avgConfidence,
            results: results
        };
    };

    // ==================== UTILITY FUNCTIONS ====================

    /**
     * Converts blob to base64
     */
    async function blobToBase64(blob) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onloadend = () => {
                const base64 = reader.result.split(',')[1];
                resolve(base64);
            };
            reader.onerror = reject;
            reader.readAsDataURL(blob);
        });
    }

    /**
     * Gets available media devices
     * @returns {Promise<object>}
     */
    BC.getAvailableDevices = async function() {
        try {
            const devices = await navigator.mediaDevices.enumerateDevices();
            return {
                hasCamera: devices.some(d => d.kind === 'videoinput'),
                hasMicrophone: devices.some(d => d.kind === 'audioinput'),
                cameras: devices.filter(d => d.kind === 'videoinput').map(d => ({
                    deviceId: d.deviceId,
                    label: d.label || 'Camera'
                })),
                microphones: devices.filter(d => d.kind === 'audioinput').map(d => ({
                    deviceId: d.deviceId,
                    label: d.label || 'Microphone'
                }))
            };
        } catch (error) {
            return {
                hasCamera: false,
                hasMicrophone: false,
                cameras: [],
                microphones: [],
                error: error.message
            };
        }
    };

    /**
     * Checks browser permissions for camera/microphone
     * @param {string} type - 'camera' or 'microphone'
     * @returns {Promise<string>} 'granted', 'denied', or 'prompt'
     */
    BC.checkPermission = async function(type) {
        try {
            const permissionName = type === 'camera' ? 'camera' : 'microphone';
            const result = await navigator.permissions.query({ name: permissionName });
            return result.state;
        } catch {
            return 'prompt'; // Fallback if Permissions API not supported
        }
    };

    /**
     * Releases all active media resources
     */
    BC.releaseAll = function() {
        BC.stopCamera();
        if (activeAudioStream) {
            activeAudioStream.getTracks().forEach(track => track.stop());
            activeAudioStream = null;
        }
        if (BC._audioContext) {
            BC._audioContext.close();
            BC._audioContext = null;
            BC._analyzer = null;
        }
        audioChunks = [];
        mediaRecorder = null;
    };

})(window.BiometricCapture);
