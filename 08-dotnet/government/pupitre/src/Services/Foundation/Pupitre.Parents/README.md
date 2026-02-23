![MDS](https://www.mameydigitalsolutions.com)

**What is Pupitre.Parents?**
----------------

Pupitre.Parents is the microservice being part of [MDS](https://dev.azure.com/mds-pr/MDS/_git/MDS) solution.

**How to Create a Service**
Open a termial window and run:
```dotnet new mamey-srv -n [Company].[Service] -c [Company] -ap [Application] -s [Service] -e [Entity] -p [ServicePortNumber]```

**How to start the application?**
----------------

Service can be started locally via `dotnet run` command (executed in the `/src/Pupitre.Parents` directory) or by running `./scripts/start.sh` shell script in the root folder of repository.

By default, the service will be available under http://localhost:60009.

You can also start the service via Docker, either by building a local Dockerfile: 

`docker build -t Pupitre.Parents .` 

or using the official one: 

`docker pull mamey.azurecr.io/Pupitre.Parents`

**What HTTP requests can be sent to the microservice API?**
----------------

You can find the list of all HTTP requests in [Pupitre.Parents.rest](https://dev.azure.com/mds-pr/MDS/_git/Pupitre.Parents?path=%2FPupitre.Parents.rest) file placed in the root folder of the repository.
This file is compatible with [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) plugin for [Visual Studio Code](https://code.visualstudio.com). 

Adding Migrations
from the Infrastructure folder
```bash
dotnet ef migrations add Initial --project ./Pupitre.Parents.Infrastructure.csproj  -s ../Pupitre.Parents.Api/Pupitre.Parents.Api.csproj -o EF/Migrations --context ParentDbContext

```