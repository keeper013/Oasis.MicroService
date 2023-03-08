namespace Oasis.MicroService;

using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public interface IJwtConfiguration : IDisposable
{
    public const string SectionName = "JwtConfig";

    string? Issuer { get; set; }

    string? PublicKeyPath { get; set; }

    IList<string>? Audiences { get; set; }

    SecurityKey? VerificationKey { get; }
}

public sealed class JwtConfiguration : IJwtConfiguration
{
    private RsaSecurityKey? _verificationKey;

    public string? Issuer { get; set; }

    public string? PublicKeyPath { get; set; }

    public IList<string>? Audiences { get; set; }

    public SecurityKey? VerificationKey
    {
        get
        {
            if (_verificationKey == null)
            {
                var provider = new RSACryptoServiceProvider();
                provider.ImportRSAPublicKey(new ReadOnlySpan<byte>(Convert.FromBase64String(File.ReadAllText(PublicKeyPath!))), out _);
                _verificationKey = new RsaSecurityKey(provider);
            }

            return _verificationKey;
        }
    }

    public void Dispose()
    {
        if (_verificationKey != null)
        {
            _verificationKey.Rsa.Dispose();
            _verificationKey = null;
        }
    }
}

public static class JwtConfigurationExtention
{
    public static void ConfigureJwtAuthentication(this IServiceCollection collection, IJwtConfiguration configuration, string verificationKeyDirectory)
    {
        collection.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(o =>
		{
			configuration.PublicKeyPath = Path.Combine(verificationKeyDirectory, configuration.PublicKeyPath!);
			o.TokenValidationParameters = new TokenValidationParameters
			{
				ValidIssuer = configuration.Issuer,
				ValidAudiences = configuration.Audiences,
				IssuerSigningKey = configuration.VerificationKey,
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
			};
		});
    }
}