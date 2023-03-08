namespace Oasis.MicroService;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

public interface IAspNetConfiguration
{
	void ConfigureServices(IServiceCollection services);

	void Configure(IApplicationBuilder app, IWebHostEnvironment env);
}