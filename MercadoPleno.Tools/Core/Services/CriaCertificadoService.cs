using MercadoPleno.Tools.Core.Domains;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace MercadoPleno.Tools.Core.Services
{
	public class CriaCertificadoService : CertificadoService
	{
		public CriaCertificadoService(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public async Task<ZeroSslCertificate> CriarCertificadoPara(string userName, string domain)
		{
			var usuario = await _certificadoRepository.ObterUsuarioZeroSsl(userName);
			var ZeroSslUser = usuario.ZeroSslUsers.FirstOrDefault(user => _zeroSslProxy.ObterCertificados(user.ApiKey).Result.Total_count < 3);

			using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			var certificate = await CriarCertificado(ZeroSslUser, domain);
			await _certificadoRepository.Substituir(certificate);
			transactionScope.Complete();
			return certificate;
		}

		protected override async Task<ZeroSslCertificate> Processar(ZeroSslUser user, ZeroSslCertificado certificado)
		{
			return await CriarCertificado(user, certificado.Domain);
		}
	}
}