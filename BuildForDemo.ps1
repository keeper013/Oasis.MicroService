$configuration=$args[0]
if ([string]::IsNullOrEmpty($configuration)) {
    $configuration = "Debug"
}

$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath

Push-Location $dir

# build demo web api host
dotnet build -c $configuration Oasis.DemoWebApi/Oasis.DemoWebApi.csproj

# remove deployed micro-services
if (Test-Path Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices) {
	Remove-Item Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices -Recurse -Force
}

# publish and copy empty demo service
dotnet publish -c $configuration Oasis.EmptyDemoService/Oasis.EmptyDemoService.csproj
Copy-Item -Path Oasis.EmptyDemoService/bin/$configuration/net8.0/publish -Destination Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/EmptyDemoService -Recurse


# publish and copy simple demo service
dotnet publish -c $configuration Oasis.SimpleDemoService/Oasis.SimpleDemoService.csproj
Copy-Item -Path Oasis.SimpleDemoService/bin/$configuration/net8.0/publish -Destination Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/SimpleDemoService -Recurse

# publish and copy demo service with sqlite
dotnet publish -c $configuration Oasis.DemoServiceWithSqlite/Oasis.DemoServiceWithSqlite.csproj
Copy-Item -Path Oasis.DemoServiceWithSqlite/bin/$configuration/net8.0/publish -Destination Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/DemoServiceWithSqlite -Recurse
Copy-Item -Path Oasis.DemoServiceWithSqlite/bin/$configuration/net8.0/publish/runtimes/win-x64/native/e_sqlite3.dll Oasis.DemoWebApi/bin/$configuration/net8.0/MicroServices/DemoServiceWithSqlite

Pop-Location