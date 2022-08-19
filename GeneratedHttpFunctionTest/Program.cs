using HttpFunction.Extensions;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureGeneratedHttpFunctions()
    .Build();

await host.RunAsync();