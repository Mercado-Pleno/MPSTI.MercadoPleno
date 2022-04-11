using MercadoPleno.Tools.Application.Abstracao;
using MercadoPleno.Tools.Core.Domains;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MercadoPleno.Tools.Core.Proxies.CsrGenerator
{
	public class CsrGenerator : BaseClass
	{
		private readonly RestClient _restClient;

		public CsrGenerator(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			var httpClientFactory = GetService<IHttpClientFactory>();
			var httpClient = httpClientFactory.CreateClient("CsrGenerator");
			_restClient = new RestClient(httpClient).UseNewtonsoftJson();
		}

		/// <summary>
		/// CSR = Certificate Signing Request
		/// </summary>
		/// <param name="csrRequest"></param>
		/// <param name="domain"></param>
		/// <returns>CsrResponse</returns>
		public async Task<ZeroSslCertificate> GerarCertificado(ICsrRequest csrRequest, string domain)
		{
			var request = new RestRequest("/generate", Method.Post) { AlwaysMultipartFormData = true };
			request.AddParameter("C", csrRequest.Pais);
			request.AddParameter("ST", csrRequest.Estado);
			request.AddParameter("L", csrRequest.Cidade);
			request.AddParameter("O", csrRequest.Organizacao);
			request.AddParameter("OU", csrRequest.Departamento);
			request.AddParameter("CN", domain);
			request.AddParameter("keySize", csrRequest.KeySize);

			var response = await _restClient.ExecuteAsync<string>(request);
			if (response.IsSuccessful)
				return ZeroSslCertificate.Create(domain, response.Content);

			return null;
		}
	}
}