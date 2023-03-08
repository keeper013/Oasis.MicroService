namespace Oasis.MicroService;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

public sealed class SwaggerConfiguration : IAspNetConfiguration
{
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddSwaggerGen();
	}
}