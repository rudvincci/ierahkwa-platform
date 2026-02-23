### Master Trust Accounts Bounded Context

#### Features and Microservices

1. **Master Trust Account Creation Service**
   - Handles the creation of new master trust accounts.
   - Registers a company or entity with the account.
   - Allows for the addition of sub-accounts like savings, loans, credit cards, and trading portfolios.

2. **Sub-Account Management Service**
   - Manages the lifecycle of sub-accounts under a master trust account.
   - Provides functionalities for opening, closing, and modifying sub-accounts.
   - Tracks the allocation and movement of funds between sub-accounts.

3. **Product Linkage Service**
   - Links bank products to the master trust or its sub-accounts.
   - Manages product features based on the type of trust account (revocable or irrevocable).

4. **Trust Account Compliance Service**
   - Ensures all trust account activities comply with legal and regulatory requirements.
   - Manages documentation and reporting for regulatory bodies.

5. **Portfolio Management Service**
   - For trading portfolio sub-accounts, manages investments, trades, and related financial activities.
   - Provides analytics and reporting on investment performance.


### Saga Design for Master Trust Accounts

#### Master Trust Account Setup Saga
This saga orchestrates the setup of a new master trust account along with its initial sub-accounts, ensuring that all steps are successfully completed or appropriately compensated if a failure occurs.

1. **Initiate Account Creation**
   - Orchestrates: Master Trust Account Creation Service to register a new account with associated company details.
   - Commands: CreateMasterTrustAccount, RegisterCompany.

2. **Setup Sub-Accounts**
   - Orchestrates: Sub-Account Management Service to open initial sub-accounts as specified during the setup process.
   - Commands: OpenSubAccount for each required account type (savings, loan, etc.).

3. **Link Bank Products**
   - Orchestrates: Product Linkage Service to attach selected bank products to the master trust account or specific sub-accounts.
   - Commands: LinkBankProduct to each selected product, specifying account linkage.

4. **Ensure Compliance**
   - Orchestrates: Trust Account Compliance Service to review the new account and its sub-accounts for compliance with regulatory requirements.
   - Commands: ReviewForCompliance, GenerateComplianceReport.

5. **Investment Portfolio Setup (if applicable)**
   - Orchestrates: Portfolio Management Service for trading portfolio sub-accounts, setting up investment parameters and preferences.
   - Commands: SetupInvestmentPortfolio, DefineInvestmentPreferences.

#### Compensation Mechanisms
For each step in the saga, specific compensating actions are defined to undo the process in case of failure, such as closing any opened sub-accounts, unlinking products, or deleting the master trust account to maintain system integrity.

### Technical Components of the Saga

#### Saga Orchestrator
- **Role:** Coordinates all steps of the saga, ensuring the execution of each step and managing compensations if necessary.
- **Implementation:** Could be a dedicated service or part of the application layer, utilizing messaging or event sourcing for managing state transitions.

#### Command and Event Handlers
- **Commands:** Direct requests to microservices to perform specific operations (e.g., `CreateMasterTrustAccount`, `OpenSubAccount`).
- **Events:** Notifications published by microservices upon completing their tasks or when an error occurs, which the orchestrator listens to for directing the flow or initiating compensations.

#### Compensation Mechanisms
- Designed to revert the system to its initial state in case of failure at any step.
- Each microservice involved in the saga must implement compensatory actions for its operations (e.g., `CloseSubAccount`, `UnlinkBankProduct`).

### Flow and Communication Patterns

1. **Asynchronous Messaging:** The preferred method for communication between the orchestrator and microservices, using a message broker or event bus to handle commands and events. This approach decouples services, improving fault tolerance and scalability.

2. **Synchronous Calls:** Used sparingly for operations that require immediate confirmation, such as verifying compliance or finalizing account creation. RESTful APIs or gRPC can facilitate these interactions.

3. **State Management:** The orchestrator maintains the saga's state, tracking which steps have been completed and what compensations might be required. This can be achieved using a database or an in-memory store, with persistent storage for reliability.

### Step-by-Step Process with Technical Details

1. **Initiate Account Creation**
   - **Command:** `CreateMasterTrustAccount` sent to the Account Creation Service.
   - **Event on Success:** `MasterTrustAccountCreated`.
   - **Event on Failure:** `AccountCreationFailed`, triggering compensation if necessary.

2. **Setup Sub-Accounts**
   - **Command:** `OpenSubAccount` sent for each required sub-account.
   - **Event on Success:** `SubAccountOpened`.
   - **Event on Failure:** `SubAccountOpeningFailed`, initiating compensation actions such as closing any opened sub-accounts.

3. **Link Bank Products**
   - **Command:** `LinkBankProduct` to attach products to the account or sub-accounts.
   - **Event on Success:** `BankProductLinked`.
   - **Event on Failure:** `ProductLinkingFailed`, requiring compensation like unlinking any connected products.

4. **Ensure Compliance**
   - **Command:** `ReviewForCompliance` issued to the Compliance Service.
   - **Event on Success:** `ComplianceConfirmed`.
   - **Event on Failure:** `ComplianceFailed`, potentially leading to the undoing of the entire account setup process.

5. **Investment Portfolio Setup**
   - **Command:** `SetupInvestmentPortfolio` for trading portfolios.
   - **Event on Success:** `InvestmentPortfolioSetup`.
   - **Event on Failure:** `PortfolioSetupFailed`, with compensation including resetting investment preferences.

#### Error Handling and Compensation
The orchestrator listens for failure events at any step to initiate the appropriate compensatory actions. It also manages timeouts or non-responses, treating them as failures to ensure the system's integrity.