namespace Oasis.EmptyDemoService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class EmptyDemoServiceContextBuilder : MicroServiceContextBuilder
{
	protected override void Initialize(IServiceCollection serviceCollection, IConfigurationRoot configuration)
	{
	}
}