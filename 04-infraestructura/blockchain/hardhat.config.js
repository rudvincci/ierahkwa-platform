require("@nomicfoundation/hardhat-toolbox");

module.exports = {
  solidity: { version: "0.8.20", settings: { optimizer: { enabled: true, runs: 200 } } },
  networks: {
    mameynode: {
      url: process.env.MAMEYNODE_RPC || "http://localhost:8545",
      chainId: 574,
      accounts: process.env.DEPLOYER_PRIVATE_KEY ? [process.env.DEPLOYER_PRIVATE_KEY] : [],
    },
    mameynode_testnet: {
      url: "https://testnet.chain.soberano.bo",
      chainId: 5740,
      accounts: process.env.DEPLOYER_PRIVATE_KEY ? [process.env.DEPLOYER_PRIVATE_KEY] : [],
    }
  },
  etherscan: { apiKey: process.env.EXPLORER_KEY }
};
