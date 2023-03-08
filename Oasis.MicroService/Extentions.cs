namespace Oasis.MicroService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public static class Extentions
{
	public static void AddMicroServices(this IServiceCollection services, IList<string> assemblies)
	{
		if (assemblies == null || !assemblies.Any())
		{
			throw new ArgumentNullException(nameof(assemblies));
		}

		var executionAssemblyDirectory = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().Location).Path))!;
		foreach (var assemblyPath in assemblies)
		{
			var fullAssemblyPath = Path.Combine(executionAssemblyDirectory, assemblyPath);
			var assembly = Assembly.LoadFrom(fullAssemblyPath);
			foreach (var type in assembly.GetExportedTypes()
				.Where(t => t.IsClass && IsMicroServiceContextBuilder(t) && !t.IsAbstract))
			{
				var constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[0]);
				var buildContextMethod = type.GetMethod(MicroServiceContextBuilder.BuildContextMethod, BindingFlags.NonPublic | BindingFlags.Instance)!;
				var buildMethod = typeof(MicroServiceContextBuilder<>).MakeGenericType(buildContextMethod.ReturnType)
					.GetMethod(MicroServiceContextBuilder.BuildMethodName, BindingFlags.Public | BindingFlags.Instance);
				if (constructor != null && buildMethod != null)
                {
					var builder = constructor.Invoke(new object[0]);
					services.AddSingleton(buildMethod.ReturnType, buildMethod.Invoke(builder, new object[] { services })!);
                }
				else
				{
					throw new ArgumentException($"Type {type.FullName} doesn't have a parameterless constructor or Build method.", nameof(assemblies));
				}
			}

			services.AddMvc().AddApplicationPart(assembly);
		}
	}

	public static string GetPath(this Assembly assembly)
	{
		UriBuilder uri = new UriBuilder(assembly.Location);
		string path = Uri.UnescapeDataString(uri.Path);
		return Path.GetDirectoryName(path)!;
	}

	private static bool IsMicroServiceContextBuilder(Type type)
	{
		var objectType = typeof(object);
		var microServiceContextBuilderType = typeof(MicroServiceContextBuilder<>);
		var baseType = type.BaseType;
		while (baseType != objectType && baseType != null)
		{
			if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == microServiceContextBuilderType)
			{
				return true;
			}

			baseType = baseType.BaseType;
		}

		return false;
	}
}
