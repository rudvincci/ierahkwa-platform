# Support

## Getting Help

### Documentation

- [README.md](README.md) - Overview and quick start
- [GETTING_STARTED.md](docs/GETTING_STARTED.md) - Detailed setup guide
- [API_REFERENCE.md](docs/API_REFERENCE.md) - Complete API documentation
- [EXAMPLES.md](docs/EXAMPLES.md) - Code examples and patterns
- [MIGRATION_GUIDE.md](docs/MIGRATION_GUIDE.md) - Version migration guide

### Issues

For bugs, feature requests, or questions:

1. **Search existing issues** - Your issue may already be reported
2. **Create a new issue** - Provide:
   - Library version
   - .NET version
   - Authentik version
   - Steps to reproduce
   - Expected vs actual behavior
   - Error messages and stack traces

### Code Generation Issues

If you encounter issues with code generation:

1. Verify Authentik instance is accessible
2. Check OpenAPI schema is valid: `https://your-instance/api/v3/schema/`
3. Ensure NSwag CLI is installed: `dotnet tool install -g NSwag.ConsoleCore`
4. Review generated code for errors
5. Check Authentik version compatibility

### Common Issues

#### Authentication Failures

- Verify API token is valid and has required permissions
- Check OAuth2 client credentials are correct
- Ensure token endpoint URL is correct
- Verify SSL certificate validation settings

#### Connection Timeouts

- Increase timeout in `AuthentikOptions`
- Check network connectivity
- Verify Authentik instance is running
- Review firewall/proxy settings

#### Code Generation Errors

- Ensure NSwag CLI is up to date
- Check OpenAPI schema format
- Verify target framework matches project
- Review NSwag configuration in scripts

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for contribution guidelines.

## License

AGPL-3.0 - See LICENSE file for details.
