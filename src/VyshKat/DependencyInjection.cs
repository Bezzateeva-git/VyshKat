using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void Configure(IServiceCollection services)
    {
        services.AddSingleton<IServiceCenter, FleetServiceCenter>();
        services.AddSingleton<Fleet>();
        services.AddSingleton<TextReader>(Console.In);
        services.AddSingleton<TextWriter>(Console.Out);
        services.AddSingleton<ConsoleApplication>();
    }
}
