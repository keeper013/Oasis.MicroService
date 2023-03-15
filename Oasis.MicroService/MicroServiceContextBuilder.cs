namespace Oasis.MicroService;

using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public abstract class MicroServiceContextBuilder
{
	private readonly ServiceCollection _serviceCollection;
	private readonly string? _environment;

	protected MicroServiceContextBuilder()
		: this(null)
	{
	}

	protected MicroServiceContextBuilder(string? environment)
	{
		_serviceCollection = new ServiceCollection();
		_environment = environment;
	}

	public ServiceProvider Build(IEnumerable<Type> controllerTypes)
	{
		this.Initialize(_serviceCollection, _environment);
		foreach (var controllerType in controllerTypes)
		{
			_serviceCollection.AddTransient(controllerType, controllerType);
		}

		return _serviceCollection.BuildServiceProvider();
	}

	protected virtual IConfigurationRoot GetConfiguration(string microServiceAssemblyLocation)
	{
		var mainConfigFilePrefix = microServiceAssemblyLocation.Substring(0, microServiceAssemblyLocation.Length - 4);
		var mainConfigFilePath = $"{mainConfigFilePrefix}.json";
		if (!File.Exists(mainConfigFilePath))
		{
			throw new FileLoadException("Missing configuration file for assembly.", microServiceAssemblyLocation);
		}

		var builder = new ConfigurationBuilder().AddJsonFile(mainConfigFilePath, false);
		if (!string.IsNullOrWhiteSpace(_environment))
		{
			var environmentConfigFilePath = $"{mainConfigFilePrefix}.{_environment}.json";
			builder = builder.AddJsonFile(environmentConfigFilePath, false);
		}

		return builder.Build();
	}

	protected abstract void Initialize(IServiceCollection serviceCollection, string? environment = null);
}