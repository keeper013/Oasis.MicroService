namespace Oasis.DemoService2;

public interface IService2Configuration
{
	string? Environment { get; }

	string? DatabasePath { get; }
}

public class Service2Configuration : IService2Configuration
{
	public string? Environment { get; set; }

	public string? DatabasePath { get; set; }
}