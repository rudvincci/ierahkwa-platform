'use strict';

const { rateLimiters } = require('../../shared/security');

const rateLimiter = rateLimiters.api();

module.exports = { rateLimiter };
