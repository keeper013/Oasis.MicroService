namespace Oasis.SimpleDemoService;

public interface ISimpleDemoServiceConfiguration
{
	string? ServiceName { get; }
}

public class SimpleDemoServiceConfiguration : ISimpleDemoServiceConfiguration
{
	public string? ServiceName { get; set; }
}