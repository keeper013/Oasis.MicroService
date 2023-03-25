namespace Oasis.MicroService;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class Extentions
{
	public static void AddMicroServices(this WebApplicationBuilder builder, MicroServiceConfiguration configuration)
	{
		if (configuration == null || !configuration.Included.Any())
		{
			throw new ArgumentNullException(nameof(configuration));
		}

		var executionAssemblyDirectory = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().Location).Path))!;
		var excluded = GetExcludedFiles(executionAssemblyDirectory, configuration.Excluded);
		var ignoreAssemblyLoadingErrors = configuration.IgnoreAssemblyLoadingErrors;
		foreach (var config in configuration.Included)
		{
			foreach (var assemblyFilePath in
				Directory.GetFiles(Path.Combine(executionAssemblyDirectory, config.Directory), config.SearchPattern, SearchOption.AllDirectories)
					.Where(p => !excluded.Contains(p)))
			{
				builder.Services.AddMicroServiceFromAssembly(
					assemblyFilePath,
					config.SearchPattern.ContainsWileCardForPath() && ignoreAssemblyLoadingErrors,
					config.Environment ?? builder.Environment.EnvironmentName);
			}
		}
	}

	public static string GetPath(this Assembly assembly)
	{
		UriBuilder uri = new UriBuilder(assembly.Location);
		string path = Uri.UnescapeDataString(uri.Path);
		return Path.GetDirectoryName(path)!;
	}

	private static void AddMicroServiceFromAssembly(this IServiceCollection services, string asssemblyFilePath, bool ignoreAssemblyLoadingErrors, string? environment)
	{
		Assembly assembly = null!;
		ConstructorInfo? constructor = null!;
		try
		{
			assembly = Assembly.LoadFrom(asssemblyFilePath);
			var serviceContextBuilderType = assembly.GetExportedTypes().SingleOrDefault(t => t.IsClass && TypeIsSubType(t, typeof(MicroServiceContextBuilder)) && !t.IsAbstract);
			if (serviceContextBuilderType == null)
			{
				throw new ArgumentException($"Assembly {asssemblyFilePath} doesn't have an implementation of type {typeof(MicroServiceContextBuilder)}.", nameof(asssemblyFilePath));
			}

			constructor = serviceContextBuilderType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[0]);
			if (constructor == null)
			{
				throw new ArgumentException($"Type {serviceContextBuilderType.FullName} in assembly {asssemblyFilePath} doesn't have a default constructor.", nameof(asssemblyFilePath));
			}
		}
		catch
		{
			if (ignoreAssemblyLoadingErrors)
			{
				return;
			}
			else
			{
				throw;
			}
		}

		var serviceContextBuilder = constructor.Invoke(new object[0]);

		var buildMethod = typeof(MicroServiceContextBuilder).GetMethod(nameof(MicroServiceContextBuilder.Build), BindingFlags.Public | BindingFlags.Instance)!;
		var controllerTypes = assembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract && TypeIsSubType(t, typeof(Controller))).ToList();
		ServiceProvider serviceProvider = (ServiceProvider)buildMethod.Invoke(serviceContextBuilder, new object?[] { controllerTypes, Path.GetDirectoryName(assembly.Location), environment })!;

		services.AddMvc().AddApplicationPart(assembly).AddControllersAsServices();
		foreach (var controllerType in controllerTypes)
		{
			services.Replace(new ServiceDescriptor(controllerType, sp => serviceProvider.GetService(controllerType)!, ServiceLifetime.Transient));
		}
	}

	private static bool TypeIsSubType(Type type, Type parentType)
	{
		var objectType = typeof(object);
		var baseType = type;
		while (baseType != parentType)
		{
			if (baseType == objectType || baseType == null)
			{
				return false;
			}

			baseType = baseType.BaseType;
		}

		return true;
	}

	private static ISet<string> GetExcludedFiles(string directory, IList<ExcludedAssemblyConfiguration>? excluded)
	{
		var excludedFiles = new HashSet<string>();
		if (excluded != null && excluded.Any())
		{
			foreach (var item in excluded)
			{
				item.ThrowIfInvalid();
				excludedFiles.UnionWith(Directory.GetFiles(Path.Combine(directory, item.Directory), item.SearchPattern, SearchOption.AllDirectories));
			}
		}

		return excludedFiles;
	}

	private static bool ContainsWileCardForPath(this string path)
	{
		return path.Contains('?') || path.Contains('*');
	}
}
