# Mamey Architecture Overview

Back to [Mamey](https://github.com/Mamey-io/Mamey.Info/tree/master)


The image in the next page shows the overview of the architecture including some microservices to be implemented.

<span style="text-align: center;display:block;">
 <img src="https://learn.microsoft.com/en-us/azure/architecture/reference-architectures/containers/aks-microservices/images/aks.svg" alt="Microsoft Microservice Architecture" width="50%">
</span>

## Technology Stack
Mamey's overall solution makes use of the cloud agnostic tools and most of them can be found under [cncf.io](https://www.cncf.io). To easily plug into the complex infrastructure, [Mamey](https://https//github.com/Mamey-io/Mamey.Info) is being used.

<span style="text-align: center;display:block;">
  <!-- <img src="https://github.com/Mamey-io/Mamey.Info/blob/master/docs/Mamey/mamey-logo@2x.png" width="100%"> -->
  <img src="infrastructure.png" style="text-align:center;" width="50%">
  <!-- ![](infrastructure.png) -->
</span>

Our application is built on a diverse and powerful set of technologies, each chosen for its ability to address specific aspects of a modern, scalable, and resilient microservice architecture. From message queuing and distributed tracing to data storage and secrets management, the components of our technology stack work in concert to ensure that our application meets the highest standards of performance, security, and reliability. Understanding the role and configuration of each technology is crucial for effective development and maintenance of the system. This section provides a concise overview of the technologies employed, including 

### [HashiCorp Consul]()
Consul is a service networking solution that allows you to discover services and secure network traffic in complex, distributed networks.
### [Fabio]()
Fabio is a fast, modern, zero-conf load balancing HTTP(S) router for deploying applications managed by Consul.
### [[Monitoring with Grafana & Prometheus]]
* ### [Grafana]()
  Grafana is an open-source platform for monitoring and observability. It allows you to query, visualize, alert on, and understand your metrics no matter where they are stored.
* ### [Prometheus]()
  Prometheus is an open-source system monitoring and alerting toolkit. It collects and stores its metrics as time series data, i.e., metrics information with timestamps.
### [Jaeger]()
Jaeger is an open-source, end-to-end distributed tracing system.
### [MongoDB]()
MongoDB is a NoSQL database that uses a document-oriented data model. It offers high performance, high availability, and easy scalability.
### [PostgreSQL]()
PostgreSQL is an open-source, object-relational database system. It has more than 15 years of active development and a proven architecture that has earned it a strong reputation for reliability, data integrity, and correctness.
### [RabbitMQ]()
RabbitMQ is an open-source message broker. It accepts and forwards messages, offering a reliable, highly available, scalable, and portable messaging system.
### [Redis]()
Redis is an in-memory data structure store, used as a database, cache, and message broker. It supports data structures such as strings, hashes, lists, sets, and more.
### [Logging and Seq]()
Seq is a log server that uses structured logging for .NET applications. It centralizes and structures your logs for easy analysis.
### [HashiCorp Vault]()
Vault ([https://www.hashicorp.com/products/vault](https://www.hashicorp.com/products/vault) ) secures, stores, and tightly controls access to tokens, passwords, certificates, API keys, and other secrets in modern computing.
Provides encryption as a service with centralized key management.
* [[Hashicorp Vault]]
### [Active Directory]()
Active Directory (AD) is a directory service developed by Microsoft for Windows domain networks. It is included in most Windows Server operating systems as a set of processes and services.
### [SQL Databases]()
SQL databases, also known as relational databases, represent data in a tabular form consisting of rows and columns. They use SQL (Structured Query Language) for defining and manipulating the data.
### [Microsoft Graph]()
Microsoft Graph is the gateway to data and intelligence in Microsoft 365. It provides a unified programmability model that you can use to access the tremendous amount of data in Microsoft 365, Windows 10, and Enterprise Mobility + Security.

----------------

## Microservice Architecture

<span style="text-align: center;display:block;">
  <!-- <img src="https://github.com/Mamey-io/Mamey.Info/blob/master/docs/Mamey/MameyArchitectureOverview/clean_architecture.png" width="100%"> -->
  <img src="clean_architecture.png" style="text-align:center;" width="35%">
  <!-- ![](infrastructure.png) -->
</span>
----------------

Depending on the particular microservice complexity, either the clean architecture + DDD approach (as presented below) is used or another style that is the best fit.![](docs/Mamey/clean_architecture.png)
## Event Driven Applications
Mamey specializes in the development of event-driven applications. These applications are designed to respond to actions or events, such as user interactions, sensor outputs, or messages from other programs. Event-driven programming is widely used in graphical user interfaces, real-time systems, and server applications.
Mamey’s approach can be either microservice-based or monolithic, depending on the specific requirements of the project.

### Microservices and Bounded Context Design Model
In the microservice approach, the application is divided into small, loosely coupled services, each running in its own process and communicating with lightweight mechanisms like HTTP/REST or messaging queues. Each microservice can be developed, deployed, and scaled independently, providing flexibility and resilience.

A key concept in microservices architecture is the bounded context, a central pattern in Domain-Driven Design (DDD). A bounded context defines the boundaries within which a model applies, ensuring that the model remains consistent and that its terms and rules do not leak into other contexts. This allows each microservice to have its own domain model that is not affected by the domain models of other microservices.

### Domain-Driven Design (DDD)
Regardless of the approach, Mamey uses Domain-Driven Design (DDD). DDD is an approach to software development that centers the software around real-world concepts and their interactions, facilitating communication between technical experts and domain experts. DDD can lead to software that is more flexible, easier to understand, and better aligned with the business needs.

### Command Query Responsibility Segregation (CQRS)
CQRS is a software architectural pattern that separates the models for read and update operations. This separation can lead to more performant, scalable, and secure applications.
By implementing CQRS, Mamey ensures that the application can evolve over time without the risk of update commands causing merge conflicts at the domain level. This is particularly beneficial in complex business systems where business rules and workflows can change over time.

The flexibility provided by CQRS allows each part of the system to be optimized independently. For example, read models can be optimized for performance, while write models can be optimized for data consistency. This can lead to a more robust and efficient system overall.
In conclusion, Mamey’s expertise in event-driven applications, combined with their use of DDD, CQRS, and the bounded context design model in microservices, allows them to create robust, efficient, and flexible software systems that meet the evolving needs of businesses.

---