### Digital Banking and Mobile Services Bounded Context

#### Digital Onboarding Service
- Customer Identity Verification
- Document Upload and Verification
- Account Type Selection (Savings, Checking, etc.)
- Initial Deposit Handling

#### Online Account Access Service
- Login and Authentication
- Account Balance Display
- Transaction History
- Account Settings and Management

#### E-statement and Notification Service
- Monthly E-statement Generation
- Transaction Alerts via Email/SMS
- Custom Notification Settings (Frequency, Channels)

#### Mobile Banking Application Service
- Mobile App Authentication (Biometrics, PIN)
- Real-time Balance Check
- Mobile Check Deposit
- Peer-to-Peer Payments

#### Mobile Payment Processing Service
- NFC Payments Setup and Management
- QR Code Payments Processing
- Mobile Wallet Integration

### Saga Design for Digital Banking and Mobile Services
This saga ensures a seamless flow from verifying the identity of a new customer to setting up their account, subscribing to statements and notifications, and finally enabling mobile banking capabilities. Error handling at each step ensures rollback or alternative flows are activated if issues arise, maintaining data consistency and integrity across the system.

#### Digital Onboarding Saga
1. **Customer Identity Verification**
   - Orchestrates: Customer data collection, document upload, and verification services.
   - Commands: CollectCustomerData, UploadDocuments, VerifyDocuments.

2. **Account Creation and Setup**
   - Orchestrates: Account type selection, initial deposit handling, and online account access setup.
   - Commands: SelectAccountType, HandleInitialDeposit, SetupOnlineAccess.

3. **E-statement and Notification Setup**
   - Orchestrates: E-statement subscription and customization of notification settings.
   - Commands: SubscribeEStatement, CustomizeNotifications.

4. **Mobile Banking Setup**
   - Orchestrates: Mobile app download prompt, mobile banking registration, and mobile payment setup.
   - Commands: PromptMobileAppDownload, RegisterForMobileBanking, SetupMobilePayments.

### Saga Technical Design for Digital Onboarding

#### 1. Saga Orchestration Components
- **Saga Orchestrator:** A service responsible for managing the state of the saga and determining the next step based on the outcomes of previous steps.
- **Command Messages:** These are specific instructions sent to microservices to perform a part of the saga. They are often implemented as events or direct API calls.
- **Compensation Actions:** Defined for each step to undo the action if subsequent steps fail. These are crucial for maintaining data integrity and consistency.

#### 2. Flow and Communication Patterns
- **Asynchronous Messaging:** Most interactions between the saga orchestrator and the microservices are asynchronous, using message queues or event buses, ensuring loose coupling and scalability.
- **Synchronous Calls:** Some steps may require immediate feedback or data retrieval, using synchronous HTTP calls or gRPC for real-time communication.
- **Event Publishing:** Microservices publish events upon completing their tasks, which the saga orchestrator listens to, moving the process to the next step or triggering compensation actions as needed.

#### 3. Step-by-Step Process and Technical Implementation

1. **CollectCustomerData and UploadDocuments**
   - Orchestrator sends a command to the Customer Service to collect data.
   - Customer uploads documents via a secure API endpoint.
   - Verification Service is invoked asynchronously to verify documents, publishing an event upon success/failure.

2. **SelectAccountType and HandleInitialDeposit**
   - Account Management Service receives a command to create a new account with the specified type.
   - Deposit Service processes the initial deposit, ensuring funds are available and securing them in the account, notifying the orchestrator upon success.

3. **SubscribeEStatement and CustomizeNotifications**
   - Notification Service is instructed to set up e-statements and customize alert preferences based on customer input, using customer preferences data.

4. **PromptMobileAppDownload, RegisterForMobileBanking, and SetupMobilePayments**
   - A message is sent to the customer with a link to download the mobile banking app.
   - Mobile Banking Service registers the customer using the data collected and verifies mobile device integrity.
   - Payment Service sets up mobile payments, linking NFC or QR code payment options to the account.

Each step's success prompts the saga to proceed to the next step. 

#### 4. Error Handling and Compensation
Any failure triggers compensation actions to revert changes, such as deleting the created account, refunding the initial deposit, or disabling mobile banking setup if it had been partially completed.
- **Compensation Logic:** Defined for each step, e.g., if mobile payment setup fails, previously enabled services like e-statements or online account access might need adjustments.
- **Transaction Log:** The orchestrator maintains a log of all actions and compensations to enable debugging, auditing, and ensuring the system can recover from failures.