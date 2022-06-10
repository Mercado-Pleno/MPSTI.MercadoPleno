using MercadoPleno.Tools.Core.Domains;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;
using MercadoPleno.Tools.Core.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MercadoPleno.Tools.Core.Services
{
	public class RemoveCertificadoService : CertificadoService
	{
		private ZeroSslStatus _zeroSslStatus;

		public RemoveCertificadoService(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public async Task<List<ZeroSslCertificate>> RemoverCertificados(ZeroSslStatus zeroSslStatus)
		{
			_zeroSslStatus = zeroSslStatus;
			var usuarios = await _certificadoRepository.ObterUsuariosZeroSsl();
			return await Processar(usuarios);
		}

		protected override async Task<ZeroSslCertificate> Processar(ZeroSslUser user, ZeroSslCertificado certificado)
		{
			if (certificado.Status.In(ZeroSslStatus.Draft, ZeroSslStatus.Expired, ZeroSslStatus.Revoked, ZeroSslStatus.Cancelled, _zeroSslStatus))
				return await RemoverCetificado(user, certificado);
			return null;
		}
	}
}