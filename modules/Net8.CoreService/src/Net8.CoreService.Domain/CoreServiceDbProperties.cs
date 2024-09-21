namespace Net8.CoreService;

public static class CoreServiceDbProperties
{
    public static string DbTablePrefix { get; set; } = "CoreService";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "CoreService";
}
