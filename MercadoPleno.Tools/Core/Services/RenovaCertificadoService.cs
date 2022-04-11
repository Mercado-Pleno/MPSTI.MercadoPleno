using MercadoPleno.Tools.Core.Domains;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace MercadoPleno.Tools.Core.Services
{
	public class RenovaCertificadoService : CertificadoService
	{
		public RenovaCertificadoService(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public async Task<List<ZeroSslCertificate>> RenovarCertificadoExpirando()
		{
			var usuarios = await _certificadoRepository.ObterUsuariosZeroSsl();
			var certificates = await Processar(usuarios);

			using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			foreach (var certificate in certificates)
				await _certificadoRepository.Substituir(certificate);
			transactionScope.Complete();

			return certificates;
		}

		protected override async Task<ZeroSslCertificate> Processar(ZeroSslUser user, ZeroSslCertificado certificado)
		{
			if (certificado.Status == ZeroSslStatus.Expiring_soon)
			{
				await RemoverCetificado(user, certificado);
				return await CriarCertificado(user, certificado.Domain);
			}

			return null;
		}
	}
}