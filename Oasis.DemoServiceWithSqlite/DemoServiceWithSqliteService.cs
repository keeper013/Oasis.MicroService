namespace Oasis.DemoServiceWithSqlite;

public interface IDemoServiceWithSqlite
{
	string Description { get; }

	string? Environment { get; }
}

internal class DemoServiceWithSqlite: IDemoServiceWithSqlite
{
	public DemoServiceWithSqlite(string? environment)
	{
		Environment = environment;
	}

	public string? Environment { get; init; }

	public string Description => $"This is";
}