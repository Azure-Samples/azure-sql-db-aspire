var builder = DistributedApplication.CreateBuilder(args);

// This is implictly there already, no need to manually add it
// builder.AddParameter("sqlsrv-password");

// SQL Server
var sqlSrv = builder.AddSqlServer("sqlsrv", port: 1435)
    .WithLifetime(ContainerLifetime.Persistent);

// Azure SQL
// var sqlSrv = builder.AddAzureSqlServer("sql")
//     .RunAsContainer();    

var sqlDb = sqlSrv.AddDatabase("aspiretodo");

var dbPrj = builder.AddSqlProject<Projects.TodoDB>("tododb")    
    .WithReference(sqlDb)
    .WaitFor(sqlDb);    

var dab = builder.AddDataAPIBuilder("dab")
    .WithImageTag("latest")
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Swagger UI";
        url.Url += "/swagger";
    })
    .WithReference(sqlDb)    
    .WaitFor(sqlDb)
    .WaitForCompletion(dbPrj);

builder.AddNpmApp("frontend", "../TodoApp.Frontend")
    .WithReference(dab)
    .WaitFor(dab)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
