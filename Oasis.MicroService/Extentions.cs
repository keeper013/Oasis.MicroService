namespace Oasis.MicroService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class Extentions
{
	public static void AddMicroServices(this IServiceCollection services, IList<MicroServiceConfiguration> configurations)
	{
		if (configurations == null || !configurations.Any())
		{
			throw new ArgumentNullException(nameof(configurations));
		}

		var executionAssemblyDirectory = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().Location).Path))!;
		foreach (var config in configurations)
		{
			var fullAssemblyPath = Path.Combine(executionAssemblyDirectory, config.Path);
			var assembly = Assembly.LoadFrom(fullAssemblyPath);
			var serviceContextBuilderType = assembly.GetExportedTypes().Single(t => t.IsClass && TypeIsSubType(t, typeof(MicroServiceContextBuilder)) && !t.IsAbstract);

			object builder = null!;
			if (string.IsNullOrWhiteSpace(config.Environment))
			{
				var constructor = serviceContextBuilderType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[0]);
				if (constructor == null)
				{
					throw new ArgumentException($"Type {serviceContextBuilderType.FullName} doesn't have a constructor that has no input parameters.", nameof(configurations));
				}

				builder = constructor.Invoke(new object[0]);
			}
			else
			{
				var constructor = serviceContextBuilderType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(string) });
				if (constructor == null)
				{
					throw new ArgumentException($"Type {serviceContextBuilderType.FullName} doesn't have a constructor that takes environment string \"{config.Environment}\" as input parameter.", nameof(configurations));
				}

				builder = constructor.Invoke(new object[] { config.Environment });
			}

			var buildMethod = typeof(MicroServiceContextBuilder).GetMethod(nameof(MicroServiceContextBuilder.Build), BindingFlags.Public | BindingFlags.Instance)!;
			var controllerTypes = assembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract && TypeIsSubType(t, typeof(Controller))).ToList();
			ServiceProvider serviceProvider = (ServiceProvider)buildMethod.Invoke(builder, new object?[] { controllerTypes })!;

			services.AddMvc().AddApplicationPart(assembly).AddControllersAsServices();
			foreach (var controllerType in controllerTypes)
			{
				services.Replace(new ServiceDescriptor(controllerType, sp => serviceProvider.GetService(controllerType)!, ServiceLifetime.Transient));
			}
		}
	}

	public static string GetPath(this Assembly assembly)
	{
		UriBuilder uri = new UriBuilder(assembly.Location);
		string path = Uri.UnescapeDataString(uri.Path);
		return Path.GetDirectoryName(path)!;
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
}
