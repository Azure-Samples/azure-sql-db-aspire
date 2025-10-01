var builder = DistributedApplication.CreateBuilder(args);

var sqlSrv = builder.AddSqlServer("sqlsrv", port: 1435)
    .WithLifetime(ContainerLifetime.Persistent);

var db = sqlSrv.AddDatabase("db");

var dbPrj = builder.AddSqlProject<Projects.WeatherDatabase>("dbPrj")
    .WithConfigureDacDeployOptions((options) => options.AllowTableRecreation = false)
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.WebApplication1>("webapi")
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Forecast API (HTTP)";
        url.Url += "/weatherforecast";
    })
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "Forecast API (HTTPS)";
        url.Url += "/weatherforecast";
    })
    .WithReference(db)
    .WaitFor(db)
    .WaitForCompletion(dbPrj);

builder.Build().Run();
