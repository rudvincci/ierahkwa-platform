require("@nomicfoundation/hardhat-toolbox");
require("dotenv").config();

module.exports = {
  solidity: {
    version: "0.8.24",
    settings: {
      optimizer: { enabled: true, runs: 200 },
      viaIR: true,
      evmVersion: "shanghai"
    }
  },
  networks: {
    mameynode: {
      url: process.env.MAMEYNODE_RPC || "http://localhost:8545",
      chainId: 777777,
      accounts: process.env.DEPLOYER_PRIVATE_KEY ? [process.env.DEPLOYER_PRIVATE_KEY] : [],
      gasPrice: 0,
      gas: 30000000
    },
    mameynode_testnet: {
      url: process.env.MAMEYNODE_TESTNET_RPC || "https://testnet.chain.soberano.bo",
      chainId: 5740,
      accounts: process.env.DEPLOYER_PRIVATE_KEY ? [process.env.DEPLOYER_PRIVATE_KEY] : [],
      gasPrice: 0,
      gas: 30000000
    }
  },
  etherscan: {
    apiKey: process.env.EXPLORER_KEY || "sovereign"
  },
  paths: {
    sources: "./contracts",
    tests: "./test",
    cache: "./cache",
    artifacts: "./artifacts"
  }
};
