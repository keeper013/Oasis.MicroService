namespace Oasis.DemoService1;

public interface IService1DemoService
{
	string Description { get; }
}

internal class Service1DemoService : IService1DemoService
{
	public string Description => $"This is";
}