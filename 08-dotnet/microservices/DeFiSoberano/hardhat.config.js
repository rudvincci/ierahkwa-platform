// ============================================================================
// IERAHKWA SOVEREIGN DEFI - HARDHAT CONFIGURATION
// Smart contract development and deployment configuration
// ============================================================================

require("@nomicfoundation/hardhat-toolbox");
require("dotenv").config();

const PRIVATE_KEY = process.env.PRIVATE_KEY || "0x0000000000000000000000000000000000000000000000000000000000000001";
const ETHERSCAN_API_KEY = process.env.ETHERSCAN_API_KEY || "";
const INFURA_PROJECT_ID = process.env.INFURA_PROJECT_ID || "";
const ALCHEMY_API_KEY = process.env.ALCHEMY_API_KEY || "";

/** @type import('hardhat/config').HardhatUserConfig */
module.exports = {
  solidity: {
    version: "0.8.20",
    settings: {
      optimizer: {
        enabled: true,
        runs: 200
      },
      viaIR: true
    }
  },
  networks: {
    hardhat: {
      chainId: 31337,
      forking: {
        url: `https://eth-mainnet.g.alchemy.com/v2/${ALCHEMY_API_KEY}`,
        enabled: false // Enable for mainnet forking
      }
    },
    localhost: {
      url: "http://127.0.0.1:8545",
      chainId: 31337
    },
    // Testnets
    sepolia: {
      url: `https://sepolia.infura.io/v3/${INFURA_PROJECT_ID}`,
      accounts: [PRIVATE_KEY],
      chainId: 11155111
    },
    goerli: {
      url: `https://goerli.infura.io/v3/${INFURA_PROJECT_ID}`,
      accounts: [PRIVATE_KEY],
      chainId: 5
    },
    mumbai: {
      url: `https://polygon-mumbai.infura.io/v3/${INFURA_PROJECT_ID}`,
      accounts: [PRIVATE_KEY],
      chainId: 80001
    },
    // Mainnets
    mainnet: {
      url: `https://mainnet.infura.io/v3/${INFURA_PROJECT_ID}`,
      accounts: [PRIVATE_KEY],
      chainId: 1
    },
    polygon: {
      url: `https://polygon-mainnet.infura.io/v3/${INFURA_PROJECT_ID}`,
      accounts: [PRIVATE_KEY],
      chainId: 137
    },
    bsc: {
      url: "https://bsc-dataseed.binance.org/",
      accounts: [PRIVATE_KEY],
      chainId: 56
    },
    arbitrum: {
      url: `https://arbitrum-mainnet.infura.io/v3/${INFURA_PROJECT_ID}`,
      accounts: [PRIVATE_KEY],
      chainId: 42161
    },
    base: {
      url: "https://mainnet.base.org",
      accounts: [PRIVATE_KEY],
      chainId: 8453
    },
    // IERAHKWA Sovereign Chain (Custom)
    ierahkwa: {
      url: process.env.IERAHKWA_RPC_URL || "http://localhost:8545",
      accounts: [PRIVATE_KEY],
      chainId: 7777
    }
  },
  etherscan: {
    apiKey: {
      mainnet: ETHERSCAN_API_KEY,
      sepolia: ETHERSCAN_API_KEY,
      polygon: process.env.POLYGONSCAN_API_KEY || "",
      bsc: process.env.BSCSCAN_API_KEY || "",
      arbitrumOne: process.env.ARBISCAN_API_KEY || ""
    }
  },
  gasReporter: {
    enabled: process.env.REPORT_GAS === "true",
    currency: "USD",
    coinmarketcap: process.env.COINMARKETCAP_API_KEY,
    outputFile: "gas-report.txt",
    noColors: true
  },
  paths: {
    sources: "./contracts",
    tests: "./test",
    cache: "./cache",
    artifacts: "./artifacts"
  },
  mocha: {
    timeout: 60000
  }
};
