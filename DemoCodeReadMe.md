## Demo Code
In the demo code:
- *Oasis.EmptyDemoService* is one empty micro-service that has no configuration to be included by search pattern *\*DemoService.dll*.
- *Oasis.SimpleDemoService* is one simple micro-service to demonstrate basic usage of *Initialize* method and configuration file, and to be included by search pattern *\*DemoService.dll*.
- *Oasis.DemoServiceWithSqlite* is a micro-service to demonstrate configuraing database contexts in *Initialize* method and environment specific configuration, and to be included by search pattern *Oasis.DemoServiceWithSqlite.dll*.
- *Oasis.CommonLibraryForDemoService* is a dependency class library referenced by all micro-services to demonstrate the usage of *Excluded* section and *IgnoreAssemblyLoadingErrors* flag.
- *Oasis.DemoWebApi* is the web API host for hosting all micro-services
To run the demo code, execute "*BuildForDemo.ps1*" file under root folder, it contains the steps to build/publish the projects and copy the binaries to relevant paths written in PowerShell script. Then debug *Oasis.DemoWebApi* project. Controller actions defined for the micro-service can be accesses under path EmptyDemoService/Test, SimpleDemoService.Test and DemoServiceWithSqlite/Test.