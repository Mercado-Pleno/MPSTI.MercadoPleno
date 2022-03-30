using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace MercadoPleno.Tools.Application.Abstracao
{
	public abstract class BaseClass
	{
		protected readonly IServiceProvider ServiceProvider;
		protected TService GetService<TService>() => ServiceProvider.GetRequiredService<TService>();
		protected ILogger Logger => GetService<ILogger>();

		public BaseClass(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
	}
}
