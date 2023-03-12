namespace Oasis.DemoService2;

public interface IService2DemoService
{
	string Description { get; }
}

internal class Service2DemoService : IService2DemoService
{
	public string Description => $"This is";
}