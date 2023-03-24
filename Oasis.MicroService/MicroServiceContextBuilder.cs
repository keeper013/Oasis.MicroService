namespace Oasis.MicroService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public abstract class MicroServiceContextBuilder
{
	private readonly ServiceCollection _serviceCollection;

	protected MicroServiceContextBuilder()
	{
		_serviceCollection = new ServiceCollection();
	}

	public ServiceProvider Build(IEnumerable<Type> controllerTypes, string directory, string? environment)
	{
		var config = GetConfiguration(directory, environment);
		this.Initialize(_serviceCollection, config);
		foreach (var controllerType in controllerTypes)
		{
			_serviceCollection.AddTransient(controllerType, controllerType);
		}

		return _serviceCollection.BuildServiceProvider();
	}

	protected virtual IConfigurationRoot GetConfiguration(string directory, string? environment)
	{
		return new ConfigurationBuilder()
			.AddJsonFile(Path.Combine(directory, "appsettings.json"), true)
			.AddJsonFile(Path.Combine(directory, $"appsettings.{environment}.json"), true)
			.Build();
	}

	protected abstract void Initialize(IServiceCollection serviceCollection, IConfigurationRoot configuration);
}