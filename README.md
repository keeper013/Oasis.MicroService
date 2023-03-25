# Oasis.MicroService
[![latest version](https://img.shields.io/nuget/v/Oasis.MicroService)](https://www.nuget.org/packages/Oasis.MicroService)
[![downloads](https://img.shields.io/nuget/dt/Oasis.MicroService)](https://www.nuget.org/packages/Oasis.MicroService)
## Introduction
Oasis.MicroService is a simple and supportive library that allows developers to deploy asp.net core web APIs as plugins in web API projects. Or, to be more straight forward, it helps developers to distribute web controller classes in class library projects instead of centralizing them the web API project. So one web API project may contain one or more such microservices, each has it's own folder, configuration files, and run time context. With the support of this library, software engineers will be able to easily deploy microservices dynamically in different web API hosts.

An example use case: I've built a micro-service asp.net core web API named *service1*, then have it running in a web host. Later I want to build another micro-service asp.net core web API named *service2*, which I also want to deploy to the same web host. But the 2 web services may not work very well in the same host, like they may both consume a lot of CPU power or memory, so if that happens, I may want to shift one of the micro-services to a different host. In the mean-time if the 2 micros-ervices are ok to be on the same host, I may want to deploy more microservices to the same host.

Of course a natual way to implement this would be to implement all such micro-services as separate web API projects, and run multiple web APIs with different ports on the same host. The concern about this approach is we may end up using 10 or 20 different ports on the same host, which is simply too many to remember or manage. Hence Oasis.MicroService library would help to publish the micro-services as class libraries projects instead of web API projects, and allowing the contents of the micro-services to be easily deployed under the same web API project like plug-ins.
## Usage & APIs
### Micro Service Implementation
To implement a micro-service, the following steps should be carried out:
1. Create a class library project, add reference to Oasis.MicroService. Define all controllers of the micro-service in this project, add the web APIs inside the controllers. Define all necessary interfaces and implementations for features.
2. Define the context builder classes for the micro-service, which should inherit from abstract class *MicroServiceContextBuilder*. There are several points to know for implementing the abstract class:
- Note that each micro-service assembly should only contain 1 non-abstract class implementing *MicroServiceContextBuilder*
- The class has an abstract method named *Initialize*, all dependency injections should be done with its first input parameter of type IServiceCollection (All controllers defined in the micro-service assembly will be resolved using the service provider built from this IServiceController interface, but don't inject the controllers manually, it's taken care of automatically).
- The second parameter of *Initialize* is the configuration root read from the micro-service's path with default logic implemented in protected virtual method *GetConfiguration*. The method by default tries to find the the configuration file has the same name *appsettings.json*, under the same path where the assembly is deployed. Environment specific configuration of *MicroServiceContextBuilder* works the same way as normal asp.net core applications, meaning config file *appsettings.Development.json* under the same folder will be taken when ASPNETCORE_ENVIRONMENT = Development, so long and so forth for other environments. This environment variable for micro-services are overridable by configuration at web host side (refer to the web host section). Note that such configuration files are considered optional by *Oasis.MicroService*, which is the same as normal asp.net core web api. If configuration file is compulsory for certain micro-services, some defensive coding can be done in the *Initialize* method. Method *GetConfiguration* is virtual for the possibility of customizing the configuration file reading behavior.
```C#
protected override void Initialize(IServiceCollection serviceCollection, IConfigurationRoot configuration)
{
	serviceCollection.AddSingleton<ISimpleDemoServiceDemoService>(new SimpleDemoServiceDemoService());
	var simpleDemoServiceConfiguration = configuration.Get<SimpleDemoServiceConfiguration>();
	if (simpleDemoServiceConfiguration == null)
	{
		throw new FileLoadException($"Configuration for {typeof(SimpleDemoServiceConfiguration)} missing", Path.GetFileName(this.GetType().Assembly.Location));
	}

	serviceCollection.AddSingleton<ISimpleDemoServiceConfiguration>(simpleDemoServiceConfiguration);
}
```
### Web Host Implementation
To implement the web API host, the following steps should be followed:
1. Create a web API project, add reference to Oasis.MicroService.
2. In configuration file, define a "*MicroServices*" section to list assemblies with environment names for all micro services to be plugged in. This section contains a list of included micro-service assemblies, a list of assemblies to be excluded, and a flag to decide whether to ignore micro-service assembly loading errors.
- *Included* section contains a list of items each configures some micro-service assembly. Each configuration item includes 3 properties: *Directory* contains directory to search for the micro-service, *Pattern* is the search pattern to find micro-service assemblies by name, while *Environment* contains the environment name for environment specific configuration (*Environment* parameter is optional, and overrides the value of ASPNETCORE_ENVIRONMENT set for web api host if specified). *Oasis.MicroService* will try to match all assemblies in the specified directory and recursively in all its sub-directories.
- Note that wildcard characters \* and ? are allowed for *SearchPattern* property (not for *Directory* property), so multiple micro-service assemblies can be matched with one such configuration item. This will be convenient if users don't want to add/remove one entry everytime a micro-service is deployed/deleted. 
- Sometimes some assemblies may be wrongfully matched by the wildcard character matching mechanism, like the dependency library "Oasis.CommonLibraryForDemoService.dll" in the micro service directories will be matched by search pattern "*DemoService.dll", hence *Excluded* property is introduced to contain a lit of items each configures some assembly that are not supposed to be considered as micro-service assembly. For the example given in the last section, just list "*Oasis.CommonLibraryForDemoService.dll*" to be exluded. Note that for *Excluded* section, wildcard character matching works the same as *Included* section. This section will only excluded assemblies matched by wildcard characters in *Included* section (e.g. in the sample configuration below, "*Oasis.DemoServiceWithSqlite.dll*" in *Included* section is matched without wildcard characters, so it will be included even if it is listed in *Excluded* section), and any assembly matched in *Excluded* section will definitely not be matched by wildcard characters in *Included* section.
- Note that root folder for included and excluded assemblies are always the directory of the web api assembly.
- The flag *IgnoreAssemblyLoadingErrors* decides whether to ignore exceptions when loading micro-service assemblies (e.g. the assembly file is not found, the assembly file is not in the correct format, the assembly file doesn't have a valid implementation of *MicroServiceContextBuilder*, and so on). If it is set to true, then Oasis.MicroService will simply swallow the exceptions and skip micro-service assemblies that throw them when being loaded. This might be useful if users have too many dependency assemblies that are included by wildcard character matching, and doesn't want to list them all in *Excluded* section. But it is recommended to always leave this flag to be false (which is the default if not configured) to avoid finding out certain expected micro-services are missing only at web API run-time. Note that this flag only works on micro-service assemblies that are matched with wildcard characters, for any item that has no wildcard character in *SearchPattern* property (e.g. SearchPattern is "Oasis.DemoServiceWithSqlite.dll" in the sample below), exceptions happend when loading the micro-service assembly will be thrown regardless of what value this flag is set to be.
```json
{
	"MicroServices":{
		"Included": [
			{ "Directory": "MicroServices", "SearchPattern": "*DemoService.dll" },
			{ "Directory": "MicroServices/DemoServiceWithSqlite", "SearchPattern": "Oasis.DemoServiceWithSqlite.dll", "Environment": "Test" }
		],
		"Excluded": [
			{ "Directory": "MicroServices", "SearchPattern": "Oasis.CommonLibraryForDemoService.dll" }
		],
    	"IgnoreAssemblyLoadingErrors": false
	}
}
```
3. In Program.cs file, read the "*MicroServices*" section from configuration file, use *AddMicroServices* API to register all micro services.
Then run the web API service, controllers defined in the micro-services should be available.
```C#
var builder = WebApplication.CreateBuilder(args);

// Add microservices to the container.
var microServiceConfiguration = new List<MicroServiceConfiguration>();
builder.Configuration.GetSection("MicroServices").Bind(microServiceConfiguration);
builder.AddMicroServices(microServiceConfiguration);
```
## Demo Code
In the demo code:
- *Oasis.EmptyDemoService* is one empty micro-service that has no configuration to be included with wildcard character matching.
- *Oasis.SimpleDemoService* is one simple micro-service to demonstrate basic usage of *Initialize* method and configuration file, and to be matched with wildcard character matching.
- *Oasis.DemoServiceWithSqlite* is a micro-service to demonstrate configuraing database contexts in *Initialize* method and environment specific configuration, and to be matched without wildcard character matching.
- *Oasis.CommonLibraryForDemoService* is a dependency class library referenced by all micro-services to demonstrate the usage of *Excluded* section and *IgnoreAssemblyLoadingErrors* flag in Oasis.MicroService configuration in appsettings.config for web api hosts.
- *Oasis.DemoWebApi* is the web API host for hosting both micro-services
To run the demo code, execute "*BuildForDemo.ps1*" file under root folder, it contains the steps to build/publish the projects and copy the binaries to relevant paths written in PowerShell script. Then debug *Oasis.DemoWebApi* project. Controller defined for service 1 is under path Service1/Test, controller defined for service 2 is under path Service2/Test
## Considerations
- To protect the strong naming key, it is not uploaded in the public repository, for source code downloaders to compile Oasis.MicroService project, please generate a key name "Oasis.snk" under Oasis.MicroService folder, or delete the strong naming configuration in the project file.
- Different micro-services may depend on the same packages, sometimes with different versions. So it's highly recommended that all dependency dlls are strong named, or else there will be assembly version conflicts among the micro-services deployed under the same host (like in the sample code its preferred to strong name *Oasis.CommonLibraryForDemoService* assembly).
