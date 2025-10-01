# SQL Server Aspire Samples

A set of samples that show how to integrate SQL Server and Azure SQL with [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview). 

**Updated to use Aspire 9.5**

All the samples can now be easily executed via the [Aspire CLI](https://learn.microsoft.com/dotnet/aspire/cli/install?tabs=windows), using the `aspire run` command from each sample folder.

+ [SQL Server Aspire Samples](#sql-server-aspire-samples)
  + [Base Sample](#base-sample)
  + [Bring-Your-Own-SQL-Server (BYOSS)](#bring-your-own-sql-server-byoss)
  + [Aspire-Hosted SQL Server](#aspire-hosted-sql-server)
  + [Aspire-Hosted SQL Server + DBUp](#aspire-hosted-sql-server--dbup)
  + [Aspire-Hosted SQL Server + Database Project](#aspire-hosted-sql-server--database-project)
  + [Aspire-Hosted SQL Server + EF Core](#aspire-hosted-sql-server--ef-core)
  + [Aspire-Hosted SQL Server + DbUp + DAB](#aspire-hosted-sql-server--dbup--dab)
  + [End-To-End Jamstack "ToDo List" Application](#end-to-end-jamstack-todo-list-application)

## Base Sample

- Folder: [`./base`](./base)

The basic .Net Aspire application with a simple WebAPI project (WebApplication1) that you can use as a starting point to add support for Azure SQL or SQL Server

## Bring-Your-Own-SQL-Server (BYOSS)

- Folder: [`./byoss`](./byoss)

The simplest integration of SQL Server in .NET Aspire. It takes the "Base" example and updates the WebAPI so that now it is calling an existing SQL Server, using a provided connectiong string. If you need a free, on-premise SQL Server, you can use the [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) that is free for development and testing. The easiest way to run it is to use the image that you can get using [`sqlcmd`](https://azuresql.dev/content/sql-server-dev-go-sqlcmd):

```bash
sqlcmd create mssql --accept-eula
```

Get the ADO.NET connection string from the SQL Server container via:

```bash
sqlcmd config cs
```

and then make sure to save it in the user secrets of the `AspireApp1.AppHost` project, so that it can be used by the application. 

```bash
dotnet user-secrets set 'ConnectionStrings:db' '<your-connection-string>'
```

## Aspire-Hosted SQL Server

- Folder: [`./hostedss`](./hostedss)

Change the BYOSS sample so the SQL Server is deployed and managed by Aspire orchestration, providing a basic integration with the Aspire environment. SQL Server is deployed in a OCI container by Aspire. No changes to WebAPI project. If you want to set the password for the created SQL Server, you can do it by setting the `Parameters:sqlsrv-password` user secret:

```bash
dotnet user-secrets set 'Parameters:sqlsrv-password' '<your-password>' 
```

otherwise a random generated password will be used. Read all the details on Azure SQL and SQL Server integration with Aspire here: [.NET Aspire SQL Server integration](https://learn.microsoft.com/en-us/dotnet/aspire/database/sql-server-integration)

The integration with Aspire is quite basic in this sample as it is done using only on the server side, introducting usage the `Aspire.Hosting.SqlServer` library in AppHost project. 

The client WebAPI application (Webapplication1) is still getting the connection string from the Aspire-provided environment variable.

## Aspire-Hosted SQL Server + DBUp

- Folder: [`./hostedss - dbup`](./hostedss%20-%20dbup)

In this sample, which is based on the previous one, deployment of database schema is also added to the solution. 

In addition there is now full integration of SQL Server with the Aspire environment. The client WebAPI application (WebApplication1) is now getting the connection object via Dependency Injection, thanks to the usage of the `Aspire.Microsoft.Data.SqlClient` library. 

The database is deployed using an imperative approach, via the `DbUp` library, that is also orchestrated by Aspire, by a custom application (DatabaseDeploy) that is added to the AppHost project to allow Aspire to orchestrate it.

## Aspire-Hosted SQL Server + Database Project

- Folder: [`./hostedss - dbprj`](./hostedss%20-%20dbprj)

Similar to the previous sample, but the database is deployed using a declarative approach, using a [SDK-Style Database Project](https://techcommunity.microsoft.com/blog/azuresqlblog/the-microsoft-build-sql-project-sdk-is-now-generally-available/4392063), that is also orchestrated by Aspire, via the community extension `CommunityToolkit.Aspire.Hosting.SqlDatabaseProjects`.

## Aspire-Hosted SQL Server + EF Core

- Folder: [`./hostedss - ef`](./hostedss%20-%20ef)

In this sample, the client application (Webapplication1) is now using Entity Framework Core to interact with the SQL Server. The EF Core database context is provided via Dependency Injection, thanks to the usage of the library: `Aspire.Microsoft.EntityFrameworkCore.SqlServer`.

The database is deployed using a EF Core database migrations, that are deployed with the support of a Worker Service, as explained in the Aspire documentation ["Apply Entity Framework Core migrations in .NET Aspire"](https://learn.microsoft.com/en-us/dotnet/aspire/database/ef-core-migrations).

Please note that to keep the sample as simple as possible the EF Core entities and migrations have been defined directly in the WebApplication1 solution, which is not a best practice for a real-world application. A dedicated project should be created for the EF Core entities and migrations.

> [!NOTE]
> This example uses `Aspire.Hosting.Azure.Sql` so that it can be **deployed in Azure** via `azd up` and an Azure SQL DB will be created and the database schema will be deployed.

## Aspire-Hosted SQL Server + DbUp + DAB

- Folder: [`./hostedss - dbup - dab`](./hostedss%20-%20dbup%20-%20dab)

In this sample, taken from the `./hostedss - dbup` sample, add [Data API Builder (DAB)](https://aka.ms/dab) to the solution, so that the database can be exposed as a REST and GraphQL API, so there is no need anymore to manuallly create a CRUD API and therefore the WebAPI project is removed.

DAB is orchestrated by Aspire, via the community extension `CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder`, that has been added to the AppHost project.

## End-To-End Jamstack "ToDo List" Application 

- Folder: [`./todo_app`](./todo_app)

Full end-to-end example of a Jamstack application, with a Vue front-end, a Data API Builder back-end, and SQL Server database, deployed using a [SDK-Style Database Project](https://techcommunity.microsoft.com/blog/azuresqlblog/the-microsoft-build-sql-project-sdk-is-now-generally-available/4392063). Everything is orchestrated by Aspire, including the Node application (TodoApp.Frontend), thanks to the `CommunityToolkit.Aspire.Hosting.NodeJS.Extensions` library.

You can run the application, either via Visual Studio or Visual Studio Code or the command line:

```
aspire run
```

if for some reason you get an error about `.dacpac` file not being available, build the .dacpac manually by runnig the following command to build the database project:

```
cd .\TodoApp.Database\
dotnet build
```

