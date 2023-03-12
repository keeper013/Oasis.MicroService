namespace Oasis.DemoService1;

public interface IService1Configuration
{
	string? ServiceName { get; }
}

public class Service1Configuration : IService1Configuration
{
	public string? ServiceName { get; set; }
}