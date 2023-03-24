# Oasis.MicroService
[![latest version](https://img.shields.io/nuget/v/Oasis.MicroService)](https://www.nuget.org/packages/Oasis.MicroService)
[![downloads](https://img.shields.io/nuget/dt/Oasis.MicroService)](https://www.nuget.org/packages/Oasis.MicroService)
## Introduction
Oasis.MicroService is a simple and supportive library that allows developers to deploy asp.net core web APIs as plugins in web API projects. Or, to be more straight forward, it helps developers to distribute web controller classes in class library projects instead of centralizing them the web API project. So one web API project may contain one or more such microservices, each has it's own folder, configuration files, and run time context. With the support of this library, software engineers will be able to easily deploy microservices dynamically in different web API hosts.

An example use case: I've built a microservice asp.net core web API named *service1*, then have it running in a web host. Later I want to build another microservice asp.net core web API named *service2*, which I also want to deploy to the same web host. But the 2 web services may not work very well in the same host, like they may both consume a lot of CPU power or memory, so if that happens, I may want to shift one of the microservices to a different host. In the mean-time if the 2 microservices are ok to be on the same host, I may want to deploy more microservices to the same host.

Of course a natual way to implement this would be to implement all such microservices as separate web API projects, and run multiple web APIs with different ports on the same host. The concern about this approach is we may end up using 10 or 20 different ports on the same host, which is simply too many to remember or manage. Hence Oasis.MicroService library would help to publish the microservices as class libraries projects instead of web API projects, and allowing the contents of the microservices to be easily deployed under the same web apo project.
## Usage & APIs
### Micro Service Implementation
To implement a microservice, the following steps should be carried out:
1. Create a class library project, add reference to Oasis.MicroService. Define all controllers of the microservice in this project, add the web APIs inside the controllers. Define all necessary interfaces and implementations for features.
2. Define the context builder classes for the microservice, which should inherit from abstract class *MicroServiceContextBuilder*. There are several points to know for implementing the abstract class:
- Note that 1 microservice assembly should only contain 1 non-abstract class implementing *MicroServiceContextBuilder*
- The class has an abstract method named *Initialize*, all dependency injections should be done with its first input parameter of type IServiceCollection (All controllers defined in the microservice assembly will be resolved using the service provider built from this IServiceController interface, but don't inject the controllers manually, it's taken care of automatically).
- The second parameter of *Initialize* is the configuration root read from the micro-service's path with default logic implemented in protected virtual method *GetConfiguration*. The method by default tries to find the the configuration file has the same name *appsettings.json*, under the same path where the assembly is deployed. Environment specific configuration of *MicroServiceContextBuilder* works the same way as normal asp.net core applications, meaning config file *appsettings.Development.json* under the same folder will be taken when ASPNETCORE_ENVIRONMENT = Development, so long and so forth for other environments. This environment variable for micro-services are overridable by configuration at web host side (refer to the web host section). Note that such configuration files are considered optional by *Oasis.MicroService*, which is the same as normal asp.net core web api. If configuration file is compulsory for certain micro-services, some defensive coding can be done in the *Initialize* method. Method *GetConfiguration* is virtual for the possibility of customizing the configuration file reading behavior.
```C#
public sealed class Service1ContextBuilder : MicroServiceContextBuilder
{
	protected override void Initialize(IServiceCollection serviceCollection, IConfigurationRoot configuration)
	{
		serviceCollection.AddSingleton<IService1DemoService>(new Service1DemoService());
		var service1Configuration = configuration.Get<Service1Configuration>();
		if (service1Configuration == null)
		{
			throw new FileLoadException($"Configuration for {typeof(Service1Configuration)} missing", Path.GetFileName(this.GetType().Assembly.Location));
		}

		serviceCollection.AddSingleton<IService1Configuration>(service1Configuration);
	}
}
```
### Web Host Implementation
To implement the web API host, the following steps should be followed:
1. Create a web API project, add reference to Oasis.MicroService.
2. In configuration file, define a "*MicroServices*" section to list paths together with environment names for all micro services to be plugged in. This section contains a list of micro-service configuration items, each configures one micro-service. Each configuration item includes 2 properties: "*Path*" contains path of the micro-service, while "*Environment*" contains the environment name for environment specific configuration (*Environment* parameter is optional, and overrides the value of ASPNETCORE_ENVIRONMENT set for web api host if specified).
```json
{
	"MicroServices":[
		{ "Path": "MicroServices/Service1/Oasis.DemoService1.dll" },
		{ "Path": "MicroServices/Service2/Oasis.DemoService2.dll", "Environment": "Test" }
	]
}
```
3. In Program.cs file, read the "*MicroServices*" section from configuration file, use *AddMicroServices* API to register all micro services.
Then run the web API service, controllers defined in the microservices should be available.
```C#
var builder = WebApplication.CreateBuilder(args);

// Add microservices to the container.
var microServiceConfigurations = new List<MicroServiceConfiguration>();
builder.Configuration.GetSection("MicroServices").Bind(microServiceConfigurations);
builder.AddMicroServices(microServiceConfigurations);
```
## Demo Code
In the demo code:
- *Oasis.DemoService1* is one simple microservice to demonstrate the basics of implementing a microservice
- *Oasis.DemoService2* is a microservice which reads from a sqlite library for demonstrating configuraing database contexts in such microservices
- *Oasis.DemoWebApi* is the web API host for hosting both microservices
To run the demo code, execute "*BuildForDemo.ps1*" file under root folder, it contains the steps to build/publish the projects and copy the binaries to relevant paths written in PowerShell script. Then debug *Oasis.DemoWebApi* project. Controller defined for service 1 is under path Service1/Test, controller defined for service 2 is under path Service2/Test
## Considerations
- To protect the strong naming key, it is not uploaded in the public repository, for source code downloaders to compile Oasis.MicroService project, please generate a key name "Oasis.snk" under Oasis.MicroService folder, or delete the strong naming configuration in the project file.
- Different microservices may depend on the same packages, sometimes with different versions. So it's highly recommended that all dependency dlls are strong named, or else there will be assembly version conflicts among the microservices deployed under the same host.
