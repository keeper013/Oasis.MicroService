# Oasis.MicroService
## Introduction
Oasis.MicroService is a simple and supportive library that allows developers to deploy asp.net core web APIs as plugins in web API projects. Or, to be more straight forward, it helps developers to distribute web controller classes in class library projects instead of centralizing them the web API project. So one web API project may contain one or more such microservices, each has it's own folder, configuration files, and run time context. With the support of this library, software engineers will be able to easily deploy microservices dynamically in different web API hosts.
An example use case: I've built a microservice asp.net core web API named *service1*, then have it running in a web host. Later I want to build another microservice asp.net core web API named *service2*, which I also want to deploy to the same web host. But the 2 web services may not work very well in the same host, like they may both consume a lot of CPU power or memory, so if that happens, I may want to shift one of the microservices to a different host. In the mean-time if the 2 microservices are ok to be on the same host, I may want to deploy more microservices to the same host.
Of course a natual way to implement this would be to implement all such microservices as separate web API projects, and run multiple web APIs with different ports on the same host. The concern about this approach is we may end up using 10 or 20 different ports on the same host, which is simply too many to remember or manage. Hence Oasis.MicroService library would help to publish the microservices as class libraries projects instead of web API projects, and allowing the contents of the microservices to be easily deployed under the same web apo project.
## Usage & APIs
### Micro Service Implementation
To implement a microservice, the following steps should be followed:
1. Create a class library project, add reference to Oasis.MicroService, define all necessary interfaces and implementations for features.
2. Define the context classes of the controllers, which should implement *MicroServiceContext*, if they haven't been defined yet. The context class is the run time context of the controllers, it should be able to expose all necessary configuration and injected dependencies for the controllers.
3. Define the context builder classes of each context, which should implement *MicroServiceContextBuilder<TContext>*. *MicroServiceContextBuilder<TContext>* class has 2 abstract methods to be implemented, *BuildContext* method initializes and returns context of the service, while *Initialize* method does all dependency injections.
4. Define controllers of the microservice, and the web APIs inside the controllers. The controllers should implement class *MicroServiceController<TContext>*, *TContext* is the type of context of this controller.
### Web Host Implementation
To implement the web API host, the following steps should be followed:
1. Create a web API project, add reference to Oasis.MicroService.
2. In configuration file, define a "*MicroServices*" section to list paths to all micro services to be plugged in.
3. In Program.cs file, read that section from configuration file, use *AddMicroServices* API to register all micro services.
Then run the web API service, controllers defined in the microservices should be available.
## Demo Code
In the demo code:
- *Oasis.DemoService1* is one simple microservice to demonstrate the basics of implementing a microservice
- *Oasis.DemoService2* is a microservice which reads from a sqlite library for demonstrating configuraing database contexts in such microservices
- *Oasis.DemoWebApi* is the web API host for hosting both microservices
To run the demo code, execute "*BuildForDemo.ps1*" file under root folder, it contains the steps to build/publish the projects and copy the binaries to relevant paths written in PowerShell script. Then debug *Oasis.DemoWebApi* project. Controller defined for service 1 is under path Service1/Test, controller defined for service 2 is under path Service2/Test
## Extra Web API supports
The library also contains some web API supporting classes to make programming easier
- ByteArrayInputFormatter, this is a formatter to support media type of application/octet-stream, it is useful for controllers receiving byte array html bodies (e.g. output of Google.ProtoBuf). TO use it, simply call builder.Services.AddControllers(options => options.InputFormatters.Add(new ByteArrayInputFormatter())); in Program.cs.
- CorsConfiguration, this class helps to configure cors for the web API, the way to use it is in the demo code and commented out (considering it's not useful for the demonstration itself).
- SwaggerConfiguration, is the supporting class to configure swagger for the web API, it simply wraps a little of swagger related code defaulted generated when createing the web API, to make the code a little neater.
- JwtConfiguration, this class helps to configure the web API to allow Jwt authentication. It's quite troublesome to distribute Jwt and relevant certificates, so the usage of this class will not documented for now. The class may be removed from *Oasis.MicroService* in the future.
