![MDS](https://www.mameydigitalsolutions.com)

**What is Mamey.ServiceName?**
----------------

Mamey.ServiceName is the microservice being part of [MDS](https://dev.azure.com/mds-pr/MDS/_git/MDS) solution.

**How to Create a Service**
Open a termial window and run:
```dotnet new mamey-srv -n [Company].[Service] -c [Company] -ap [Application] -s [Service] -e [Entity] -p [ServicePortNumber]```

**How to start the application?**
----------------

Service can be started locally via `dotnet run` command (executed in the `/src/Mamey.ServiceName` directory) or by running `./scripts/start.sh` shell script in the root folder of repository.

By default, the service will be available under http://localhost:serviceport.

You can also start the service via Docker, either by building a local Dockerfile: 

`docker build -t Mamey.ServiceName .` 

or using the official one: 

`docker pull mamey.azurecr.io/Mamey.ServiceName`

**What HTTP requests can be sent to the microservice API?**
----------------

You can find the list of all HTTP requests in [Mamey.ServiceName.rest](https://dev.azure.com/mds-pr/MDS/_git/Mamey.ServiceName?path=%2FMamey.ServiceName.rest) file placed in the root folder of the repository.
This file is compatible with [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) plugin for [Visual Studio Code](https://code.visualstudio.com). 

Adding Migrations
from the Infrastructure folder
```bash
dotnet ef migrations add Initial --project ./Mamey.ServiceName.Infrastructure.csproj  -s ../Mamey.ServiceName.Api/Mamey.ServiceName.Api.csproj -o EF/Migrations --context EntityNameDbContext

```