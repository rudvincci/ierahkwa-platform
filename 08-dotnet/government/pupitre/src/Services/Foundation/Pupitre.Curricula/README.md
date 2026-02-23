![MDS](https://www.mameydigitalsolutions.com)

**What is Pupitre.Curricula?**
----------------

Pupitre.Curricula is the microservice being part of [MDS](https://dev.azure.com/mds-pr/MDS/_git/MDS) solution.

**How to Create a Service**
Open a termial window and run:
```dotnet new mamey-srv -n [Company].[Service] -c [Company] -ap [Application] -s [Service] -e [Entity] -p [ServicePortNumber]```

**How to start the application?**
----------------

Service can be started locally via `dotnet run` command (executed in the `/src/Pupitre.Curricula` directory) or by running `./scripts/start.sh` shell script in the root folder of repository.

By default, the service will be available under http://localhost:60003.

You can also start the service via Docker, either by building a local Dockerfile: 

`docker build -t Pupitre.Curricula .` 

or using the official one: 

`docker pull mamey.azurecr.io/Pupitre.Curricula`

**What HTTP requests can be sent to the microservice API?**
----------------

You can find the list of all HTTP requests in [Pupitre.Curricula.rest](https://dev.azure.com/mds-pr/MDS/_git/Pupitre.Curricula?path=%2FPupitre.Curricula.rest) file placed in the root folder of the repository.
This file is compatible with [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) plugin for [Visual Studio Code](https://code.visualstudio.com). 

Adding Migrations
from the Infrastructure folder
```bash
dotnet ef migrations add Initial --project ./Pupitre.Curricula.Infrastructure.csproj  -s ../Pupitre.Curricula.Api/Pupitre.Curricula.Api.csproj -o EF/Migrations --context CurriculumDbContext

```