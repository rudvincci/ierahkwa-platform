// ion-crypto.js
const fs = require('fs');
const { canonicalize } = require('jsonld');
const { computeCommitment } = require('@decentralized-identity/ion-tools');
const { generateOperationHash, signJws } = require('@decentralized-identity/ion-tools');

// Usage: node ion-crypto.js <cmd> <payloadFile> <privateKeyJwkFile>
const [,, cmd, payloadFile, privateKeyFile, jwkFile] = process.argv;

if (cmd === 'commitment') {
    const jwk = JSON.parse(fs.readFileSync(jwkFile));
    // ION expects secp256k1, "ES256K"
    const commitment = computeCommitment({ publicJwk: jwk });
    console.log(commitment);
}
if (cmd === "canonicalize") {
    const json = JSON.parse(fs.readFileSync(payloadFile));
    canonicalize(json).then((result) => {
        console.log(result);
    });
} else if (cmd === "jwsSign") {
    const payload = JSON.parse(fs.readFileSync(payloadFile));
    const privateKey = JSON.parse(fs.readFileSync(privateKeyFile));
    signJws(payload, privateKey).then((jws) => {
        console.log(jws);
    });
}

// ...add more commands as needed
