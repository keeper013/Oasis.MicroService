namespace Oasis.MicroService;

using Microsoft.Extensions.DependencyInjection;

public abstract class MicroServiceContextBuilder
{
	private readonly ServiceCollection _serviceCollection;

	protected MicroServiceContextBuilder()
	{
		_serviceCollection = new ServiceCollection();
	}

	public ServiceProvider Build(IEnumerable<Type> controllerTypes)
	{
		this.Initialize(_serviceCollection);
		foreach (var controllerType in controllerTypes)
		{
			_serviceCollection.AddTransient(controllerType, controllerType);
		}

		return _serviceCollection.BuildServiceProvider();
	}

	protected abstract void Initialize(IServiceCollection serviceCollection);
}