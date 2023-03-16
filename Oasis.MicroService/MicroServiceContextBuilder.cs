namespace Oasis.MicroService;

using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public abstract class MicroServiceContextBuilder
{
	private readonly ServiceCollection _serviceCollection;

	protected MicroServiceContextBuilder()
	{
		_serviceCollection = new ServiceCollection();
	}

	public ServiceProvider Build(IEnumerable<Type> controllerTypes, string microServiceAssemblyLocation, string? environment)
	{
		var config = GetConfiguration(microServiceAssemblyLocation, environment);
		this.Initialize(_serviceCollection, config);
		foreach (var controllerType in controllerTypes)
		{
			_serviceCollection.AddTransient(controllerType, controllerType);
		}

		return _serviceCollection.BuildServiceProvider();
	}

	protected virtual IConfigurationRoot? GetConfiguration(string microServiceAssemblyLocation, string? environment)
	{
		var mainConfigFilePrefix = microServiceAssemblyLocation.Substring(0, microServiceAssemblyLocation.Length - 4);
		var mainConfigFilePath = $"{mainConfigFilePrefix}.json";
		if (!File.Exists(mainConfigFilePath))
		{
			return null;
		}

		var builder = new ConfigurationBuilder().AddJsonFile(mainConfigFilePath, false);
		if (!string.IsNullOrWhiteSpace(environment))
		{
			var environmentConfigFilePath = $"{mainConfigFilePrefix}.{environment}.json";
			builder = builder.AddJsonFile(environmentConfigFilePath, false);
		}

		return builder.Build();
	}

	protected abstract void Initialize(IServiceCollection serviceCollection, IConfigurationRoot? configuration);
}