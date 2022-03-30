using MercadoPleno.Tools.Application.Abstracao;
using MercadoPleno.Tools.Core.Domains;
using MercadoPleno.Tools.Core.Proxies.CsrGenerator;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;
using MercadoPleno.Tools.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MercadoPleno.Tools.Core.Services
{
	public class CertificadoService : BaseClass
	{
		private readonly CsrGenerator _csrGenerator;
		private readonly ZeroSslProxy _zeroSslProxy;

		public CertificadoService(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_csrGenerator = GetService<CsrGenerator>();
			_zeroSslProxy = GetService<ZeroSslProxy>();
		}

		public async Task<int> RenovarCertificadoExpirando()
		{
			var certificadoRepository = GetService<CertificadoRepository>();
			var usuarios = await certificadoRepository.ObterUsuariosZeroSsl();
			return await Renovar(usuarios);
		}

		private async Task<int> Renovar(IEnumerable<Usuario> usuarios)
		{
			var count = 0;
			foreach (var usuario in usuarios)
				count += await Renovar(usuario);
			return count;
		}

		private async Task<int> Renovar(Usuario usuario)
		{
			var count = 0;
			foreach (var user in usuario.ZeroSslUsers)
				count += await Renovar(user);
			return count;
		}

		private async Task<int> Renovar(ZeroSslUser user)
		{
			var count = 0;
			var certificados = await _zeroSslProxy.ObterCertificados(user.ApiKey);
			foreach (var certificado in certificados.Results)
			{
				if (await Renovar(certificado, user))
					count++;
			}
			return count;
		}

		private async Task<bool> Renovar(ZeroSslCertificado certificado, ZeroSslUser user)
		{
			if (certificado.Status == ZeroSslStatus.Expiring_soon)
			{
				var domain = certificado.Common_name;

				var csrResponse = await _csrGenerator.GerarCertificado(user.Usuario.ZeroSslConfig, domain);
				var zeroSslVerificaRequest = new ZeroSslVerificaRequest { CSR = csrResponse.Certificate };

				var zeroSslResponse = await _zeroSslProxy.VerificarCertificado(user.ApiKey, zeroSslVerificaRequest.CSR);
				if (zeroSslResponse.Valid)
				{
					var zeroSslCreateRequest = new ZeroSslCriaRequest { Domain = domain, CSR = csrResponse.Certificate };
					var zeroSslCertificadoResponse = await _zeroSslProxy.CriarCertificado(user.ApiKey, zeroSslCreateRequest);

					var zeroSslValidaRequest = new ZeroSslValidaRequest { EMail = zeroSslCertificadoResponse.GetValidationEMail() };
					var zeroSslValidaResponse = await _zeroSslProxy.ValidarCertificado(user.ApiKey, zeroSslCertificadoResponse.Id, zeroSslValidaRequest);

					var zeroSslStatusResponse = await _zeroSslProxy.ObterStatusCertificado(user.ApiKey, zeroSslCertificadoResponse.Id);

					return (zeroSslValidaResponse.Status == ZeroSslStatus.Draft) || zeroSslStatusResponse.Validation_completed;
				}
			}

			return false;
		}
	}
}