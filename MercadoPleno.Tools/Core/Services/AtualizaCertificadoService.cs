using MercadoPleno.Tools.Application.Abstracao;
using MercadoPleno.Tools.Core.Domains;
using MercadoPleno.Tools.Core.Proxies.CsrGenerator;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;
using MercadoPleno.Tools.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace MercadoPleno.Tools.Core.Services
{
	public class AtualizaCertificadoService : BaseClass
	{
		protected readonly CsrGenerator _csrGenerator;
		protected readonly ZeroSslProxy _zeroSslProxy;
		protected readonly CertificadoRepository _certificadoRepository;

		public AtualizaCertificadoService(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_csrGenerator = GetService<CsrGenerator>();
			_zeroSslProxy = GetService<ZeroSslProxy>();
			_certificadoRepository = GetService<CertificadoRepository>();
		}

		public async Task<IEnumerable<ZeroSslCertificate>> AtualizarCertificados()
		{
			var usuarios = await _certificadoRepository.ObterUsuariosZeroSsl();
			var zeroSslCertificates = usuarios.SelectMany(u => u.ZeroSslUsers).SelectMany(u => u.ZeroSslCertificates).Where(c => c.Status == ZeroSslStatus.Pending_validation).ToArray();

			await Processar(zeroSslCertificates);

			using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			foreach (var certificate in zeroSslCertificates.Where(c => c.Status == ZeroSslStatus.Issued))
				await _certificadoRepository.Atualizar(certificate);
			transactionScope.Complete();

			return zeroSslCertificates.Where(c => c.Status == ZeroSslStatus.Issued);
		}

		protected async Task Processar(IEnumerable<ZeroSslCertificate> zeroSslCertificates)
		{
			foreach (var zeroSslCertificate in zeroSslCertificates)
				await Processar(zeroSslCertificate);
		}

		protected async Task Processar(ZeroSslCertificate zeroSslCertificate)
		{
			if (zeroSslCertificate.Status == ZeroSslStatus.Pending_validation)
			{
				var user = zeroSslCertificate.ZeroSslUser;
				var status = await _zeroSslProxy.ObterStatusCertificado(user.ApiKey, zeroSslCertificate.CertificateId);
				if (status.Validation_completed)
				{
					var zeroSslCrtResponse = await _zeroSslProxy.ObterCertificadoInLine(user.ApiKey, zeroSslCertificate.CertificateId);
					zeroSslCertificate.Status = ZeroSslStatus.Issued;
					zeroSslCertificate.CertificateCrt = zeroSslCrtResponse.CertificateCrt;
					zeroSslCertificate.CertificateSigned = zeroSslCrtResponse.CertificateSigned;
				}
			}
		}
	}
}