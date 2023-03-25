namespace Oasis.DemoServiceWithSqlite;

public interface IDemoServiceWithSqliteConfiguration
{
	string? Environment { get; }

	string? DatabasePath { get; }
}

public class DemoServiceWithSqliteConfiguration : IDemoServiceWithSqliteConfiguration
{
	public string? Environment { get; set; }

	public string? DatabasePath { get; set; }
}