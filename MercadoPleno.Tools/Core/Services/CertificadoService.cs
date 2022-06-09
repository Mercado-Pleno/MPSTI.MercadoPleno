using MercadoPleno.Tools.Application.Abstracao;
using MercadoPleno.Tools.Core.Domains;
using MercadoPleno.Tools.Core.Proxies.CsrGenerator;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;
using MercadoPleno.Tools.Core.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MercadoPleno.Tools.Core.Services
{
	public abstract class CertificadoService : BaseClass
	{
		protected readonly CsrGenerator _csrGenerator;
		protected readonly ZeroSslProxy _zeroSslProxy;
		protected readonly CertificadoRepository _certificadoRepository;

		public CertificadoService(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_csrGenerator = GetService<CsrGenerator>();
			_zeroSslProxy = GetService<ZeroSslProxy>();
			_certificadoRepository = GetService<CertificadoRepository>();
		}

		protected async Task<List<ZeroSslCertificate>> Processar(IEnumerable<Usuario> usuarios)
		{
			var result = new List<ZeroSslCertificate>();
			foreach (var usuario in usuarios)
			{
				var csrResponse = await Processar(usuario);
				result.AddRange(csrResponse);
			}
			return result;
		}

		protected async Task<IEnumerable<ZeroSslCertificate>> Processar(Usuario usuario)
		{
			var result = new List<ZeroSslCertificate>();
			foreach (var user in usuario.ZeroSslUsers)
			{
				var csrResponse = await Processar(user);
				result.AddRange(csrResponse);
			}
			return result;
		}

		protected async Task<IEnumerable<ZeroSslCertificate>> Processar(ZeroSslUser user)
		{
			var result = new List<ZeroSslCertificate>();
			var certificados = await _zeroSslProxy.ObterCertificados(user.ApiKey);
			foreach (var certificado in certificados.Results)
			{
				var csrResponse = await Processar(user, certificado);
				result.Add(csrResponse);
			}
			return result.Where(c => c != null);
		}

		protected abstract Task<ZeroSslCertificate> Processar(ZeroSslUser user, ZeroSslCertificado certificado);

		protected async Task<ZeroSslCertificate> RemoverCetificado(ZeroSslUser user, ZeroSslCertificado certificado)
		{
			await _zeroSslProxy.RevogarCertificado(user.ApiKey, certificado.Id);
			await _zeroSslProxy.CancelarCertificado(user.ApiKey, certificado.Id);
			await _zeroSslProxy.RemoverCertificado(user.ApiKey, certificado.Id);
			return new ZeroSslCertificate { CertificateId = certificado.Id };
		}

		protected async Task<ZeroSslCertificate> CriarCertificado(ZeroSslUser user, string domain)
		{
			var csrResponse = await _csrGenerator.GerarCertificado(user.Usuario.ZeroSslConfig, domain);
			var zeroSslVerificaRequest = new ZeroSslVerificaRequest { CSR = csrResponse.CertificateRequest };

			var zeroSslResponse = await _zeroSslProxy.VerificarCertificado(user.ApiKey, zeroSslVerificaRequest.CSR);
			if (zeroSslResponse.Valid)
			{
				var zeroSslCreateRequest = new ZeroSslCriaRequest { Domain = domain, CSR = csrResponse.CertificateRequest };
				var zeroSslCertificadoResponse = await _zeroSslProxy.CriarCertificado(user.ApiKey, zeroSslCreateRequest);

				var zeroSslValidaRequest = new ZeroSslValidaRequest { EMail = zeroSslCertificadoResponse.GetValidationEMail() };
				var zeroSslValidaResponse = await _zeroSslProxy.ValidarCertificado(user.ApiKey, zeroSslCertificadoResponse.Id, zeroSslValidaRequest);

				csrResponse.ZeroSslUserId = user.Id;
				csrResponse.CertificateId = zeroSslCertificadoResponse.Id;
				csrResponse.Status = zeroSslValidaResponse.Status;
				return csrResponse;
			}

			return null;
		}
	}
}