var builder = DistributedApplication.CreateBuilder(args);

var sqlSrv = builder.AddSqlServer("sqlsrv").
    WithLifetime(ContainerLifetime.Persistent);

var db = sqlSrv.AddDatabase("db");

builder.AddProject<Projects.WebApplication1>("webapi")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
