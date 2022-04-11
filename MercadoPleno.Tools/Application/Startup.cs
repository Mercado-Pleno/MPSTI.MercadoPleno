using MercadoPleno.Tools.Application;
using MercadoPleno.Tools.Core.Proxies.CsrGenerator;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL;
using MercadoPleno.Tools.Core.Repositories;
using MercadoPleno.Tools.Core.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: FunctionsStartup(typeof(Startup))]

namespace MercadoPleno.Tools.Application
{
	public class Startup : FunctionsStartup
	{
		private IConfigurationRoot _configuration;

		public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
		{
			var context = builder.GetContext();

			_configuration = builder.ConfigurationBuilder
				.AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.json"), true, false)
				.AddEnvironmentVariables()
				.Build();
		}

		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.Configure<IConfiguration>(_configuration);
			builder.Services.AddLogging();

			builder.Services.AddHttpClient();
			builder.Services.AddHttpClient("CsrGenerator", http => http.BaseAddress = new Uri("https://csrgenerator.com"));
			builder.Services.AddHttpClient("ZeroSsl", http => http.BaseAddress = new Uri("https://api.zerossl.com"));

			builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(_configuration.GetConnectionString("MercadoPleno")));
			builder.Services.AddTransient<CsrGenerator>();
			builder.Services.AddTransient<ZeroSslProxy>();
			builder.Services.AddTransient<CertificadoRepository>();
			builder.Services.AddTransient<RenovaCertificadoService>();
			builder.Services.AddTransient<RemoveCertificadoService>();
			builder.Services.AddTransient<CriaCertificadoService>();
			builder.Services.AddTransient<AtualizaCertificadoService>();

			builder.Services.Configure<JsonSerializerOptions>(options =>
			{
				options.Converters.Add(new JsonStringEnumConverter());
			});
		}
	}
}