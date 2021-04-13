Stone Assemblies MassAuth
=========================

MassAuth is a _free_, _open source_ distributed, extensible message based authorization framework built on top of [MassTransit](https://masstransit-project.com/).

Build Status
------------

Branch | Status
------ | :------:
master | [![Build Status](https://dev.azure.com/alexfdezsauco/External%20Repositories%20Builds/_apis/build/status/stoneassemblies.StoneAssemblies.MassAuth?branchName=master)](https://dev.azure.com/alexfdezsauco/External%20Repositories%20Builds/_build/latest?definitionId=6&branchName=master)
develop | [![Build Status](https://dev.azure.com/alexfdezsauco/External%20Repositories%20Builds/_apis/build/status/stoneassemblies.StoneAssemblies.MassAuth?branchName=develop)](https://dev.azure.com/alexfdezsauco/External%20Repositories%20Builds/_build/latest?definitionId=6&branchName=develop)

Prequisites 
--------------

1) Docker
2) Tye Tool

Local deployment with with Tye 
------------------------------

1) Clone this repository open a console and run the following command

        > cd deployment/tye
        > tye run


2) With this command you can test how all rules pass.

        >  curl http://localhost:6001/api/balance?PrimaryAccountNumber=12345


Output (Status: 200):

        {
            "balance": 22.729830474001275,
            "dateTime": "2021-04-10T03:15:25.5597771-04:00",
            "primaryAccountNumber": "12345"
        }


3) With the following command you can test how a rule fails. Notice the __unauthorized__ result. In this case the rule checks if the `primaryAccountNumber` length is not equals to 5. 

        > curl http://localhost:6001/api/balance?PrimaryAccountNumber=123456

Output (Status: 401 Unauthorized):

        {
            "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
            "title": "Unauthorized",
            "status": 401,
            "traceId": "00-003a275368f4af46927001570c6ab566-aef497927337e44e-00"
        }



Implementing Rules 
-------------------------

1) The first step to implement a rules you must identity the message first. Take a look into the _StoneAssemblies.MassAuth.Bank.Messages_ demo project to see a message implementation.

2) After defining a message type, you can write a rule for this message. The a look into the _StoneAssemblies.MassAuth.Bank.Rules_ project. Rules must be distributed as NuGet packages.

3) Use the [AuthorizeByRule] attribute from the [StoneAssemblies.MassAuth](https://www.nuget.org/packages/StoneAssemblies.MassAuth) NuGet package the methods of the controllers. Take a look into _StoneAssemblies.MassAuth.Bank.Balance.Services_ demo project.

Host Rules 
-----------

Since rules packages are distribued as NuGet package, rules packages must be specified as part for the extensibility configuration section. 

        {
            "Extensions": {
                "RepositoryUrl": "https://api.nuget.org/v3/index.json",
                "Packages" : ["StoneAssemblies.MassAuth.Bank.Rules"]
            }
        }

Docker Image 
-------------

Stone Assemblies MassAuth Server is distributed as docker image. So, you can use it directly using the following command line.

        > docker run -d --name massauth-server {...} stoneassemblies/massauth-server:latest

The configuration could be passed using environment variables or mounting appsettings.json file.


Interop from not .NET techology
--------------------------------

Stone Assemblies MassAuth Proxy enables interoperability from not .NET techology. There is also a docker image for this proxy service.

	> docker run -d --name massauth-proxy {...} stoneassemblies/massauth-proxy:latest


Message packages must be specified as part for the extensibility configuration section. 


        {
            "Extensions": {
                "RepositoryUrl": "https://api.nuget.org/v3/index.json",
                "Packages" : ["StoneAssemblies.MassAuth.Bank.Messages"]
            }
        }


In order to test the proxy server you can use the following commands:


1) With this command you can request authorization and notice how all tests pass.

	>  curl http://localhost:6004/api/Authorize/AccountBalanceRequestMessage?PrimaryAccountNumber=12345

Ouput (Status: 200):
	1


2) With this command you can request authorization and also noticed how a rule fails.

 	>  curl http://localhost:6004/api/Authorize/AccountBalanceRequestMessage?PrimaryAccountNumber=123456

Ouput (Status: 401 Unauthorized):

	{
	    "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
	    "title": "Unauthorized",
	    "status": 401,
	    "traceId": "00-cb87958e866dac4aad9713c6e143da96-d517606cb9632149-00"
	}