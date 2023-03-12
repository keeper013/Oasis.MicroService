$configuration=$args[0]
if ([string]::IsNullOrEmpty($configuration)) {
    $configuration = "Debug"
}

$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath

Push-Location $dir

# build demo web api host
dotnet build -c $configuration Oasis.DemoWebApi/Oasis.DemoWebApi.csproj

# publish service 1
dotnet publish -c $configuration Oasis.DemoService1/Oasis.DemoService1.csproj

# copy service 1 to web api
if (Test-Path Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/Service1) {
	Remove-Item Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/Service1 -Recurse -Force
}

Copy-Item -Path Oasis.DemoService1/bin/$configuration/net8.0/publish -Destination Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/Service1 -Recurse

# publish service 2
dotnet publish -c $configuration Oasis.DemoService2/Oasis.DemoService2.csproj

# copy service 2 to web api
if (Test-Path Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/Service2) {
	Remove-Item Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/Service2 -Recurse -Force
}

Copy-Item -Path Oasis.DemoService2/bin/$configuration/net8.0/publish -Destination Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/Service2 -Recurse
Copy-Item -Path Oasis.DemoService2/bin/$configuration/net8.0/publish/runtimes/win-x64/native/e_sqlite3.dll Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/Service2

Pop-Location