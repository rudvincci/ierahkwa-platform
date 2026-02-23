# HashiCorp Vault
Vault will be used as a store for sensitive data: secrets, certificates, connections strings, etc. For development and production we will need to configure i∏t so that the microservices can consume this service.

## Setup Vault

## Access
You could access the configuration either by the console or the GUI located at http://localhost:8200. 

![alt text](images/vault-login.png)

Local development configuration will use a the token `secret` for accessing vault.

![alt text](images/vault-dashboard.png)

## Create Vault Secret Engines
![Create secrets Engine](images/vault-secrets-engine.png)

![Create database engine](images/vault-create-kv.png)


![Create database engine](images/vault-create-kv-2.png)

![Create database engine](images/vault-create-database.png)

![Create database engine](images/vault-create-pki.png)

Open Vault CLI
![alt text](images/vault-cli-button.png)


![alt text](images/vault-cli-panel.png)

### Database
```bash

```

### PKI
```bash
```
### Console
1. Open a terminal and enter the Docker container.
    ```bash
    docker exec -it vault /bin/sh
    ```
2. Login to Vault and enter token
    ```bash
    vault login
    Token (will be hidden):

    # Command output
    Success! You are now authenticated. The token information displayed below
    is already stored in the token helper. You do NOT need to run "vault login"
    again. Future Vault requests will automatically use this token.

    Key                  Value
    ---                  -----
    token                secret
    token_accessor       8w1tfT1zwBWSbFFGt9WHpcaP
    token_duration       ∞
    token_renewable      false
    token_policies       ["root"]
    identity_policies    []
    policies             ["root"]
    
    ```

## Enable the Secrets, Database connection string management, and PKI management
In the console, enter the following commands to enable the secrets, database, and PKI management features.

```bash
# Enable KV v2 engine for key-value secrets
vault secrets enable -path="kv" -version=2 -description="Mamey KV" kv

# Enable database secrets engine for dynamic database credentials
vault secrets enable database

# Enable PKI secrets engine for certificate management
vault secrets enable pki

# Enable transit secrets engine for encryption as a service
vault secrets enable transit

# Enable RabbitMQ secrets engine (if needed)
vault secrets enable rabbitmq

# Enable Consul secrets engine (if needed)
vault secrets enable consul
```

Create a new secret for the `appsettings.json` file of each service.

```bash
vault kv put kv/mamey/<service-name>/appsettings <KEY>=<VALUE>
```

**Important**: For KV v2, the path format is `kv/mamey/<service-name>/appsettings`. The `/data/` prefix is automatically handled by Vault when using `vault kv put`.


### Appsettings configuration
Modify the configuration settings, if not done already, to match the configuration entered in Vault. This is as follows:

```json
{
  "vault": {
    "enabled": true,
    "url": "http://localhost:8200",
    "authType": "token",
    "token": "secret",
    "username": "user",
    "password": "secret",
    "kv": {
      "enabled": true,
      "engineVersion": 2,
      "mountPoint": "kv",
      "path": "<service-name>/appsettings"
    },
    "pki": {
      "enabled": true,
      "roleName": "<service-name>",
      "commonName": "<service-name>.mamey.io"
    },
    "lease": {
      "mongo": {
        "type": "database",
        "roleName": "<service-name>",
        "enabled": true,
        "autoRenewal": true,
        "templates": {
          "connectionString": "mongodb://{{username}}:{{password}}@mongo:27017"
        }
      },
      "postgres": {
        "type": "database",
        "roleName": "<service-name>-postgres",
        "enabled": true,
        "autoRenewal": true,
        "templates": {
          "connectionString": "Host=postgres;Database=<service-name>;Username={{username}};Password={{password}};Port=5432"
        }
      }
    }
  }
}
```

**Important**: 
- Replace `<service-name>` with actual service name
- The `type` is always `"database"` for any database type (MongoDB, PostgreSQL, MySQL, etc.)
- The `roleName` must match a role configured in Vault for the specific database type
- The `templates` use `{{username}}` and `{{password}}` placeholders that are automatically replaced by the library
- Use `mongo:27017` for Docker environments, `localhost:27017` for local development

## PKI Root Configuration

Configure the PKI root certificate authority (one-time setup):

```bash
# Set PKI URLs
vault write pki/config/urls \
    issuing_certificates="http://localhost:8200/v1/pki/ca" \
    crl_distribution_points="http://localhost:8200/v1/pki/crl"

# Generate root CA (if not already done)
vault write -field=certificate pki/root/generate/internal \
    common_name="mamey.io Root CA" \
    ttl=87600h > ca_cert.crt

# Configure CA certificate
vault write pki/config/ca \
    pem_bundle=@ca_cert.crt
```

## Database Configuration

### Configure MongoDB Connection

Configure the MongoDB database connection (one-time setup):
```bash
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="identity-service,organizations-service,employees-service,notifications-service,people-service,bank-accounts-service,bank-branches-service,bank-members-service,bank-ktt-service,bank-cards-service,bank-currencies-service,bank-customers-service,bank-deposits-service,bank-employees-service,bank-fdr-service,bank-giftcards-service,bank-loans-service,bank-notifier-service,bank-organizations-service,bank-paymentgateways-service,bank-products-service,bank-savings-service,bank-support-service,bank-teller-service,bank-transactions-service,bank-transfers-service,bank-users-service,bank-wallets-service,bank-withdraws-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"
```

**Note**: The `allowed_roles` list should include all services that need database access. Update this list as you add new services.

### Configure PostgreSQL Connection

Configure the PostgreSQL database connection (one-time setup):

```bash
vault write database/config/postgresql \
    plugin_name=postgresql-database-plugin \
    allowed_roles="identity-service,organizations-service,employees-service,notifications-service,people-service" \
    connection_url="postgresql://{{username}}:{{password}}@postgres:5432/postgres?sslmode=disable" \
    username="admin" \
    password="secret" \
    max_open_connections=5 \
    max_idle_connections=5 \
    max_connection_lifetime="5s"
```

**Note**: The `allowed_roles` list should include all services that need PostgreSQL access. Update this list as you add new services. You can configure multiple database types (MongoDB, PostgreSQL, MySQL, etc.) - each with its own configuration path.

## RabbitMQ Configuration

### Configure RabbitMQ Connection

Configure the RabbitMQ connection (one-time setup):

```bash
# Configure RabbitMQ connection
vault write rabbitmq/config/connection \
    connection_uri="http://rabbitmq:15672" \
    username="admin" \
    password="secret"
```

## Consul Configuration

### Configure Consul Connection

Configure the Consul connection (one-time setup):

```bash
# Configure Consul connection
vault write consul/config/access \
    address="http://consul:8500" \
    token="secret-consul-token"
```

## Microservice Configuration

Below are the configurations for each of the services. Each service requires:
1. Database role configuration (MongoDB and/or PostgreSQL)
2. PKI role configuration
3. RabbitMQ role configuration (if used)
4. Consul role configuration (if used)
5. KV secret creation (example shown for first service)

**Note**: Each microservice should have a `vault-config.md` file in its root directory with service-specific Vault configuration instructions. See `.cursor/templates/vault-config.md` for the template.

#### Identity Service
```bash
# MongoDB database role
vault write database/roles/identity-service \
    db_name=mongodb \
    creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "identity-service"}] }' \
    default_ttl="1h" \
    max_ttl="24h"

# PostgreSQL database role (if used)
vault write database/roles/identity-service-postgres \
    db_name=postgresql \
    creation_statements="CREATE ROLE \"{{name}}\" WITH LOGIN PASSWORD '{{password}}' VALID UNTIL '{{expiration}}'; GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO \"{{name}}\"; GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO \"{{name}}\";" \
    revocation_statements="REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA public FROM \"{{name}}\"; REVOKE ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public FROM \"{{name}}\"; DROP ROLE IF EXISTS \"{{name}}\";" \
    default_ttl="1h" \
    max_ttl="24h"

# PKI role
vault write pki/roles/identity-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

# Issue certificate (optional - for testing)
vault write pki/issue/identity-service \
    common_name=identity-service.mamey.io
```
#### Organizations Service
```bash
# MongoDB database role
vault write database/roles/organizations-service \
    db_name=mongodb \
    creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "organizations-service"}] }' \
    default_ttl="1h" \
    max_ttl="24h"

# PostgreSQL database role (if used)
vault write database/roles/organizations-service-postgres \
    db_name=postgresql \
    creation_statements="CREATE ROLE \"{{name}}\" WITH LOGIN PASSWORD '{{password}}' VALID UNTIL '{{expiration}}'; GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO \"{{name}}\"; GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO \"{{name}}\";" \
    revocation_statements="REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA public FROM \"{{name}}\"; REVOKE ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public FROM \"{{name}}\"; DROP ROLE IF EXISTS \"{{name}}\";" \
    default_ttl="1h" \
    max_ttl="24h"

# PKI role
vault write pki/roles/organizations-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

# Issue certificate (optional - for testing)
vault write pki/issue/organizations-service \
    common_name=organizations-service.mamey.io
```
#### Employees Service
```bash
vault write database/roles/employees-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "employees-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"
vault write pki/roles/employees-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h
vault write pki/issue/employees-service \
    common_name=employees-service.mamey.io
```
#### Notifications Service
```bash
vault write database/roles/notifications-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "notifications-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"
vault write pki/roles/notifications-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h
vault write pki/issue/notifications-service \
    common_name=notifications-service.mamey.io
```
#### People Service
```bash
# MongoDB database role
vault write database/roles/people-service \
    db_name=mongodb \
    creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "people-service"}] }' \
    default_ttl="1h" \
    max_ttl="24h"

# PostgreSQL database role (if used)
vault write database/roles/people-service-postgres \
    db_name=postgresql \
    creation_statements="CREATE ROLE \"{{name}}\" WITH LOGIN PASSWORD '{{password}}' VALID UNTIL '{{expiration}}'; GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO \"{{name}}\"; GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO \"{{name}}\";" \
    revocation_statements="REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA public FROM \"{{name}}\"; REVOKE ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public FROM \"{{name}}\"; DROP ROLE IF EXISTS \"{{name}}\";" \
    default_ttl="1h" \
    max_ttl="24h"

# PKI role
vault write pki/roles/people-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

# Issue certificate (optional - for testing)
vault write pki/issue/people-service \
    common_name=people-service.mamey.io
```
#### Accounts
----------------
Establish the connection to the database connection. This configuration is for mongodb.
```bash
vault write pki/roles/bank-accounts-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-accounts-service \
    common_name=bank-accounts-service.mamey.io
```

#### Branches
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-branches-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-branches-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-branches-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

# PKI Configuration

vault write pki/roles/bank-branches-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-branches-service \
    common_name=bank-branches-service.mamey.io
```

#### Cards
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-cards-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-cards-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-cards-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

# PKI Configuration

vault write pki/roles/bank-cards-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-cards-service \
    common_name=bank-cards-service.mamey.io
```

#### Currencies
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-currencies-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-currencies-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-currencies-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"


vault write pki/roles/bank-currencies-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-currencies-service \
    common_name=bank-currencies-service.mamey.io
```

#### Customers
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-customers-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-customers-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-customers-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"


vault write pki/roles/bank-customers-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-customers-service \
    common_name=bank-customers-service.mamey.io
```

#### Deposits
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-deposits-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-deposits-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-deposits-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

vault write pki/roles/bank-deposits-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-deposits-service \
    common_name=bank-deposits-service.mamey.io
```

#### Employees
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-employees-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-employees-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-employees-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

vault write pki/roles/bank-employees-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-employees-service \
    common_name=bank-employees-service.mamey.io
```

#### FixedDepositReceipts
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-fdr-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-fdr-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-fdr-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"


vault write pki/roles/bank-fdr-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-fdr-service \
    common_name=bank-fdr-service.mamey.io
```

#### Gateway
----------------


#### Gift Cards
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-giftcards-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-giftcards-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-giftcards-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

vault write pki/roles/bank-giftcards-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-giftcards-service \
    common_name=bank-giftcards-service.mamey.io
```

#### Loans
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-loans-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-loans-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-loans-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"


vault write pki/roles/bank-loans-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-loans-service \
    common_name=bank-loans-service.mamey.io
```

#### Notifier
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-notifier-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-notifier-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-notifier-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"


vault write pki/roles/bank-notifier-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-notifier-service \
    common_name=bank-notifier-service.mamey.io
```

#### Organizations
----------------
```bash
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-organizations-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-organizations-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-organizations-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"
     

vault write pki/roles/bank-organizations-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-organizations-service \
    common_name=bank-organizations-service.mamey.io
```

#### Payment Gateways
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-paymentgateways-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"

vault write database/roles/bank-paymentgateways-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-paymentgateways-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

vault write pki/roles/bank-paymentgateways-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-paymentgateways-service \
    common_name=bank-paymentgateways-service.mamey.io
```

#### Products
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-products-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-products-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-products-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"


vault write pki/roles/bank-products-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-products-service \
    common_name=bank-products-service.mamey.io
```

#### Savings
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-savings-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-savings-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-savings-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"


vault write pki/roles/bank-savings-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-savings-service \
    common_name=bank-savings-service.mamey.io
```

#### Support
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-support-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-support-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-support-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

vault write pki/roles/bank-support-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-support-service \
    common_name=bank-support-service.mamey.io
```

#### Teller
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-teller-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-teller-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-teller-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

vault write pki/roles/bank-teller-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-teller-service \
    common_name=bank-teller-service.mamey.io
```

#### Transactions
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-transactions-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-transactions-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-transactions-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"


vault write pki/roles/bank-transactions-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-transactions-service \
    common_name=bank-transactions-service.mamey.io
```

#### Transfers
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-transfers-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-transfers-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-transfers-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

vault write pki/roles/bank-transfers-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-transfers-service \
    common_name=bank-transfers-service.mamey.io
```

#### UI Core
----------------


#### UI External
----------------


#### UI Interal
----------------


#### Users
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-users-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-users-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-users-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

vault write pki/roles/bank-users-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-users-service \
    common_name=bank-users-service.mamey.io
```

#### Wallets
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-wallets-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-wallets-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-wallets-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"


vault write pki/roles/bank-wallets-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-wallets-service \
    common_name=bank-wallets-service.mamey.io
```

#### Withdraws
----------------
```bash
# Database connection string configuration
vault write database/config/mongodb \
     plugin_name=mongodb-database-plugin \
     allowed_roles="bank-withdraws-service" \
     connection_url="mongodb://{{username}}:{{password}}@mongo:27017" \
     username="root" \
     password="secret"


vault write database/roles/bank-withdraws-service \
     db_name=mongodb \
     creation_statements='{ "db": "admin", "roles": [{"role": "readWrite", "db": "bank-withdraws-service"}] }' \
     default_ttl="1h" \
     max_ttl="24h"

vault write pki/roles/bank-withdraws-service \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h

vault write pki/issue/bank-withdraws-service \
    common_name=bank-withdraws-service.mamey.io
```

#### Feeds Aggregator
----------------
```bash
# Add configuration as needed
```

#### Feeds Blockchain
----------------
```bash
# Add configuration as needed
```

## Service-Specific Vault Configuration

Each microservice should have a `vault-config.md` file in its root directory with service-specific Vault configuration instructions.

**Template Location:** `.cursor/templates/vault-config.md`

**File Structure:**
```
{ServiceRoot}/
├── vault-config.md          # Service-specific Vault configuration
├── src/
│   └── {Service}.Api/
│       ├── appsettings.json
│       ├── appsettings.local.json
│       ├── appsettings.development.json
│       └── appsettings.docker.json
└── ...
```

**Required Content:**
- Service-specific Vault role configurations
- Database lease configurations (MongoDB, PostgreSQL)
- RabbitMQ lease configuration (if used)
- Consul lease configuration (if used)
- PKI role configuration
- KV secrets path configuration
- Environment-specific notes

**Reference:**
- Library Usage: `Mamey/docs/libraries/infrastructure/secrets-vault.md`
- Creation Rules: `.cursor/rules/microservice-creation.md`
- Maintenance Rules: `.cursor/rules/microservice-maintenance.md`

## Validation and Testing

### Test KV Access

```bash
# Read a secret
vault kv get kv/mamey/<service-name>/appsettings

# List secrets
vault kv list kv/mamey/
```

### Test Database Lease

```bash
# Request MongoDB credentials
vault read database/creds/<service-name>

# Verify MongoDB credentials work
mongosh "mongodb://<username>:<password>@mongo:27017/<database-name>"

# Request PostgreSQL credentials
vault read database/creds/<service-name>-postgres

# Verify PostgreSQL credentials work
psql "host=postgres port=5432 dbname=<database-name> user=<username> password=<password>"
```

### Test PKI Certificate

```bash
# Issue a certificate
vault write pki/issue/<service-name> \
    common_name=<service-name>.mamey.io

# View certificate details
vault read pki/cert/<certificate-serial>
```

### Test RabbitMQ Lease

```bash
# Request RabbitMQ credentials
vault read rabbitmq/creds/<service-name>

# Verify credentials work
# Use username and password to connect to RabbitMQ
```

### Test Consul Lease

```bash
# Request Consul token
vault read consul/creds/<service-name>

# Verify token works
# Use token to authenticate with Consul
```

## Security Best Practices

### Development Environment

- Use token authentication with short-lived tokens
- Never commit tokens or secrets to version control
- Use environment variables for sensitive configuration
- Rotate tokens regularly

### Production Environment

- **Use AppRole or Kubernetes authentication** instead of static tokens
- Enable audit logging for all Vault operations
- Use proper TLS certificates for Vault API access
- Implement secret rotation policies
- Use Vault policies to enforce least privilege access
- Enable Vault seal/unseal with multiple unseal keys
- Store unseal keys securely (e.g., in a separate key management system)

### Example: AppRole Authentication

```bash
# Enable AppRole auth method
vault auth enable approle

# Create a role
vault write auth/approle/role/<service-name> \
    token_policies="<policy-name>" \
    token_ttl=1h \
    token_max_ttl=4h

# Get role ID
vault read auth/approle/role/<service-name>/role-id

# Get secret ID
vault write -f auth/approle/role/<service-name>/secret-id
```

## Troubleshooting

### Common Issues

1. **"Permission denied" errors**: Check that the token has the correct policies
2. **"Database connection failed"**: Verify MongoDB/PostgreSQL is running and credentials are correct
3. **"PKI certificate not found"**: Ensure PKI root CA is configured
4. **"KV secret not found"**: Verify the path and mount point are correct
5. **"Lease renewal failed"**: Check that autoRenewal is enabled and lease is renewable

### Debug Commands

```bash
# Check Vault status
vault status

# List enabled secret engines
vault secrets list

# List enabled auth methods
vault auth list

# Check token capabilities
vault token capabilities <token> <path>

# View audit logs
vault audit list
```

## Integration with Mamey Framework

The Mamey Framework provides `Mamey.Secrets.Vault` library for seamless integration. Configure your service's `Program.cs`:

```csharp
using Mamey.Secrets.Vault;

var builder = WebApplication.CreateBuilder(args);

// Add Vault configuration
builder.UseVault(keyValuePath: "<service-name>/appsettings");

var app = builder.Build();
app.Run();
```

The library automatically:
- Loads configuration from Vault KV engine
- Manages database credential leases
- Issues and renews PKI certificates
- Handles lease renewal in the background

For more details, see the [Mamey.Secrets.Vault documentation](../../Mamey/docs/libraries/infrastructure/secrets-vault.md).

## Summary

This guide covers:
- Vault server setup and configuration
- Database connection configuration (MongoDB, PostgreSQL)
- RabbitMQ and Consul configuration
- PKI certificate management
- KV secrets management
- Service-specific configuration requirements
- Security best practices
- Troubleshooting

**Next Steps:**
1. Set up Vault server using this guide
2. Create service-specific `vault-config.md` for each microservice
3. Configure appsettings.json files with Vault configuration
4. Test and validate Vault integration
5. Follow security best practices for production deployment
