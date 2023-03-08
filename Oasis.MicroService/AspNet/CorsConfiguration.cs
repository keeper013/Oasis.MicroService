namespace Oasis.MicroService;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

public sealed class CorsConfiguration : IAspNetConfiguration
{
	private const string CorsPolicyName = "CorsPolicy";
	private const string AllowedOriginsName = "AllowedOrigins";
	private const string AllowedMethodsName = "AllowedMethods";
	private const string AllowedHeaders = "AllowedHeaders";

	private readonly CorsConfig corsConfig;

	public CorsConfiguration(CorsConfig corsConfig)
	{
		this.corsConfig = corsConfig ?? throw new ArgumentNullException(nameof(corsConfig));
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		app.UseCors(CorsPolicyName);
	}

	/// <inheritdoc />
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy(CorsPolicyName, builder =>
			{
				builder.WithOrigins(this.corsConfig.AllowedOrigins!).WithMethods(this.corsConfig.AllowedMethods!).WithHeaders(this.corsConfig.AllowedHeaders!);
			});
		});
	}

	public class CorsConfig
	{
		public const string SectionName = "CorsConfig";

		public string[]? AllowedOrigins { get; set; }

		public string[]? AllowedMethods { get; set; }

		public string[]? AllowedHeaders { get; set; }
	}
}