```json
{
    "$schema": "http://json.schemastore.org/template",
    "author": "Mamey Digital Solutions LLC",
    "name": "Mamey Microservice",
    "shortName": "mamey-micro",
    "defaultName": "Mamey.Entity",

    // --name, 
    // // Unique name for this template
    // This enables all instances of that string to 
    // be re-written by the user-provided value 
    // specified at the command line.
    // This string is also used to subsitute the 
    // namespace name in the .cs file for the 
    // project.
    "sourceName": "Mamey.ServiceName",  
    "identity": "Mamey.ServiceName",
    "groupIdentity": "Mamey.ServiceName",
    "classifications": [ 
        "Mamey", 
        "Microservice", 
        "Multi-Project", 
        "C#9" 
    ],
    "tags": {
      "language": "C#",
      "type": "project"
    },
    "preferNameDirectory": true,
    "guids": [
        "2150E333-8FDC-42A3-9474-1A3956D46DE8",
        "5305A26B-B32B-4E5F-8686-38B841C8CF4C",
        "4ED5B3D7-7D3B-405E-8335-47A6B9DBB791",
        "513BD1BB-4596-4575-917B-7AA932BDBDA3",
        "DDC2663F-1DE3-4FA7-AB18-E46E1A14A971",
        "EC508FDC-8FA4-4175-B237-6046BEB97EBF",
        "61DAEE25-8D31-4200-B8CD-F53B2B322F4D",
        "D22A0851-928A-49F9-80D0-63FFD7928C87",
        "D9DA524A-33E4-45EC-8710-4753B43DF039",
        "B8FFFBDF-A1CF-4964-9DA1-0EC88F42DFA6",
        "DC9E50C6-164B-4457-A215-BA555D0CD5FF",
        "78B721E0-4BCB-4FC5-B464-78AA770D7BD6",
        "ECECFEF0-B521-41F2-8EC5-5CA7A5F5CEAF"
    ],
    "symbols": {
        "service": {
            "type": "parameter",
            "datatype": "text",
            "replaces": "ServiceName",
            "fileRename": "ServiceName",
            "defaultValue": "Service",
            "isRequired": true,
            "description": "Name of the service"
        },
        "entity": {
            "type": "parameter",
            "datatype": "text",
            "replaces": "EntityName",
            "fileRename": "EntityName",
            "defaultValue": "Entity",
            "isRequired": true,
            "description": "Domain AggragateRoot Entity name (singular form)"
        },
        "port": {
            "type": "parameter",
            "datatype": "text",
            "replaces": "serviceport",
            "isRequired": true,
            "defaultValue": "5001",
            "description": "Internal service port number (default: 5001)"
        },
        "serviceLower":{
            "type": "generated",
            "generator": "casing",
            "parameters": {
              "source":"service",
              "toLower": true
            },
            "replaces":"servicename"
        },
        "entityLower":{
            "type": "generated",
            "generator": "casing",
            "parameters": {
              "source":"entity",
              "toLower": true
            },
            "replaces":"entityname"
        },
        "applicationLower":{
            "type": "generated",
            "generator": "casing",
            "fileRename": "mds",
            "parameters": {
              "source":"application",
              "toLower": true
            },
            "replaces":"mds"
        },
        "skipRestore": {
            "type": "parameter",
            "datatype": "bool",
            "description": "If specified, skips the automatic restore of the project on create.",
            "defaultValue": "false"
        }
    },
    "sources": [{
        "modifiers": [
            { "exclude": [ ".vs/**", ".template_config/**" ] }
        ]
    }],
    "primaryOutputs": [
        {
          "path": "Mamey.ServiceName.sln"        
        }
    ],
    "postActions": [
    {
      "condition": "(!skipRestore)",
      "description": "Restore NuGet packages required by this project.",
      "manualInstructions": [
        { "text": "Run 'dotnet restore Mamey.ServiceName/Mamey.ServiceName.sln'" }
      ],
      "actionId": "210D431B-A78B-4D2F-B762-4ED3E3EA9025",
      "continueOnError": true
    }]
  }
```