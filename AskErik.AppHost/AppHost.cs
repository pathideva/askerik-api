using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var lessonApi = builder.AddProject<Projects.AskErik_LessonApi>("askerik-lessonapi")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AskErik_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(lessonApi)
    .WaitFor(lessonApi);

builder.Build().Run();
