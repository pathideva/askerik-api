using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//var cache = builder.AddRedis("cache");

var askerikapi = builder.AddProject<Projects.AskErik_Api>("askerik-api")
    .WithExternalHttpEndpoints()
    //.WithReference(cache)
    //.WaitFor(cache)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AskErik_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(askerikapi)
    .WaitFor(askerikapi);

builder.Build().Run();
