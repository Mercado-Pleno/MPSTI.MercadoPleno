using MercadoPleno.Tools.Application.Abstracao;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;
using MercadoPleno.Tools.Core.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MercadoPleno.Tools.Certificados.Timer
{
	public class VerificaCertificado : BaseClass
	{
		public VerificaCertificado(IServiceProvider serviceProvider) : base(serviceProvider) { }


		[FunctionName("RunOnStartup")]
		public async Task RunOnStartup([TimerTrigger("0 0 0 1 1 1", RunOnStartup = true)] TimerInfo timer, ILogger log)
		{
			await RemoverCertificados(timer, log);
			await CriarCertificadoPara(timer, log);
			await AtualizarCertificados(timer, log);
		}


		[FunctionName("RemoverCertificados")]
		public async Task RemoverCertificados([TimerTrigger("0 0 0 * * *")] TimerInfo timer, ILogger log)
		{
			var removerCertificadoService = GetService<RemoveCertificadoService>();
			var certificados = await removerCertificadoService.RemoverCertificados(ZeroSslStatus.Undefined);
			log.LogInformation("{count}", certificados.Count);
		}


		[FunctionName("CriarCertificadoPara")]
		public async Task CriarCertificadoPara([TimerTrigger("0 0 0 * * *")] TimerInfo timer, ILogger log)
		{
			var criarCertificadoService = GetService<CriaCertificadoService>();
			var certificado = await criarCertificadoService.CriarCertificadoPara("ssl@mercadopleno.com.br", "xodividas.com.br");
			log.LogInformation("{certificado.Id}", certificado.ZeroSslCertificateId);
		}


		[FunctionName("RenovarCertificadoExpirando")]
		public async Task RenovarCertificadoExpirando([TimerTrigger("0 0 0 * * *")] TimerInfo timer, ILogger log)
		{
			var renovarCertificadoService = GetService<RenovaCertificadoService>();
			var certificados = await renovarCertificadoService.RenovarCertificadoExpirando();

			log.LogInformation("{count}", certificados.Count);
		}


		[FunctionName("AtualizarCertificados")]
		public async Task AtualizarCertificados([TimerTrigger("0 0 0 * * *")] TimerInfo timer, ILogger log)
		{
			var atualizaCertificadoService = GetService<AtualizaCertificadoService>();
			var certificados = await atualizaCertificadoService.AtualizarCertificados();

			log.LogInformation("{count}", certificados.Count);
		}
	}
}