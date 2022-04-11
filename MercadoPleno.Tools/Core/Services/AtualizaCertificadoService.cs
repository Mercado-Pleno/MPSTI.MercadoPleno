using MercadoPleno.Tools.Core.Domains;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace MercadoPleno.Tools.Core.Services
{
	public class AtualizaCertificadoService : CertificadoService
	{
		public AtualizaCertificadoService(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public async Task<List<ZeroSslCertificate>> AtualizarCertificados()
		{
			var usuarios = await _certificadoRepository.ObterUsuariosZeroSsl();
			var certificates = await Processar(usuarios);

			using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			foreach (var certificate in certificates)
				await _certificadoRepository.Atualizar(certificate);
			transactionScope.Complete();

			return certificates;
		}

		protected override async Task<ZeroSslCertificate> Processar(ZeroSslUser user, ZeroSslCertificado certificado)
		{
			if ((certificado.Status == ZeroSslStatus.Issued) && (certificado.Created >= DateTime.Today.AddDays(-7)))
			{
				var status = await _zeroSslProxy.ObterStatusCertificado(user.ApiKey, certificado.Id);
				if (status.Validation_completed)
				{
					var zeroSslCrtResponse = await _zeroSslProxy.ObterCertificadoInLine(user.ApiKey, certificado.Id);
					return new ZeroSslCertificate
					{
						ZeroSslUserId = user.ZeroSslUserId,
						CertificateId = certificado.Id,
						Status = certificado.Status,
						Domain = certificado.Domain,
						CertificateCrt = zeroSslCrtResponse.CertificateCrt,
						CertificateSigned = zeroSslCrtResponse.CertificateSigned,
					};
				}
			}

			return null;
		}
	}
}