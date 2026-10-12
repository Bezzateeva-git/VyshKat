using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new ServiceCollection();
DependencyInjection.Configure(services);

using var provider = services.BuildServiceProvider();
ConsoleApplication application = provider.GetRequiredService<ConsoleApplication>();
application.Run();
