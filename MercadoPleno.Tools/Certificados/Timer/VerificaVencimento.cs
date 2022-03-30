using MercadoPleno.Tools.Application.Abstracao;
using MercadoPleno.Tools.Core.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MercadoPleno.Tools.Certificados.Timer
{
	public class VerificaVencimento : BaseClass
	{
		public VerificaVencimento(IServiceProvider serviceProvider) : base(serviceProvider) { }

		[FunctionName("VerificaVencimento")]
		public async Task Run([TimerTrigger("0 0 0 * * *", RunOnStartup = true, UseMonitor = true)] TimerInfo myTimer, ILogger log)
		{
			var certificadoService = GetService<CertificadoService>();

			var count = await certificadoService.RenovarCertificadoExpirando();

			log.LogInformation("{count}", count);
		}
	}
}