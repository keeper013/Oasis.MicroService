namespace Oasis.DemoService2;

public interface IService2DemoService
{
	string Description { get; }

	string? Environment { get; }
}

internal class Service2DemoService : IService2DemoService
{
	public Service2DemoService(string? environment)
	{
		Environment = environment;
	}

	public string? Environment { get; init; }

	public string Description => $"This is";
}