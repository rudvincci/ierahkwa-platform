module.exports = {
  testEnvironment: 'node',
  testMatch: ['**/03-backend/**/__tests__/**/*.test.js'],
  coverageDirectory: 'coverage',
  collectCoverageFrom: ['03-backend/**/src/**/*.js', '03-backend/**/*.js', '!**/node_modules/**'],
  coverageReporters: ['text', 'lcov', 'clover'],
  testTimeout: 10000
};
