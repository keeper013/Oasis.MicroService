namespace Oasis.SimpleDemoService;

public interface ISimpleDemoServiceDemoService
{
	string Description { get; }
}

internal class SimpleDemoServiceDemoService : ISimpleDemoServiceDemoService
{
	public string Description => $"This is";
}