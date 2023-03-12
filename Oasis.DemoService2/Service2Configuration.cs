namespace Oasis.DemoService2;

public interface IService2Configuration
{
	string? ServiceName { get; }

	string? DatabasePath { get; }
}

public class Service2Configuration : IService2Configuration
{
	public string? ServiceName { get; set; }

	public string? DatabasePath { get; set; }
}