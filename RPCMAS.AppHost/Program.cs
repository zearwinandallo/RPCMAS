var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache", 6379);

var apiService = builder.AddProject<Projects.RPCMAS_API>("apiservice")
    .WithReference(cache);

builder.AddProject<Projects.RPCMAS_Blazor>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
