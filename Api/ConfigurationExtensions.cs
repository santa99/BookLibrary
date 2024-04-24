namespace Attendance;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddConfigOptionsAndBind<TOptions>(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        string section,
        out TOptions instance)
        where TOptions : class, new()
    {
        IConfigurationSection section1 = configuration.GetSection(section);
        TOptions instance1 = new TOptions();
        section1.Bind((object)instance1);
        instance = instance1;
        serviceCollection.Configure<TOptions>((IConfiguration)section1);
        return serviceCollection;
    }
}