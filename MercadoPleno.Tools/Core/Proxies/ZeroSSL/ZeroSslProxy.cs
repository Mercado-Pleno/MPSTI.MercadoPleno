using MercadoPleno.Tools.Application.Abstracao;
using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL
{
	public class ZeroSslProxy : BaseClass
	{
		private readonly RestClient _restClient;

		public ZeroSslProxy(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			var httpClientFactory = GetService<IHttpClientFactory>();
			var httpClient = httpClientFactory.CreateClient("ZeroSsl");
			_restClient = new RestClient(httpClient).UseNewtonsoftJson();
		}

		// Post /validation/csr?access_key={apikey}
		public async Task<ZeroSslResponse> VerificarCertificado(string apiKey, string csr)
		{
			var request = new RestRequest("/validation/csr", Method.Post) { AlwaysMultipartFormData = true };
			request.AddParameter("csr", csr);
			var response = await ExecuteAsync<ZeroSslResponse>(request, apiKey);
			return response.Data;
		}

		// Post /certificates?access_key={apikey}
		public async Task<ZeroSslCertificado> CriarCertificado(string apiKey, ZeroSslCriaRequest body)
		{
			var request = new RestRequest("/certificates", Method.Post) { AlwaysMultipartFormData = true };
			request.AddParameter("strict_domains", body.StrictDomains);
			request.AddParameter("certificate_validity_days", body.ValidityDays);
			request.AddParameter("certificate_domains", body.Domain);
			request.AddParameter("certificate_csr", body.CSR);
			var response = await ExecuteAsync<ZeroSslCertificado>(request, apiKey);
			return response.Data;
		}

		// Post /certificates/{id}/challenges?access_key={apikey}
		public async Task<ZeroSslCertificado> ValidarCertificado(string apiKey, string id, ZeroSslValidaRequest body)
		{
			var request = new RestRequest("/certificates/{id}/challenges", Method.Post) { AlwaysMultipartFormData = true };
			request.AddOrUpdateParameter("id", id, ParameterType.UrlSegment);
			request.AddParameter("validation_method", body.Method);
			request.AddParameter("validation_email", body.EMail);
			var response = await ExecuteAsync<ZeroSslCertificado>(request, apiKey);
			return response.Data;
		}

		// Post /certificates/{id}/challenges/email?access_key={apikey}
		public async Task<ZeroSslCertificado> RevalidarCertificado(string apiKey, string id, ZeroSslValidaRequest body)
		{
			var request = new RestRequest("/certificates/{id}/challenges/email", Method.Post) { AlwaysMultipartFormData = true };
			request.AddOrUpdateParameter("id", id, ParameterType.UrlSegment);
			request.AddParameter("validation_method", body.Method);
			request.AddParameter("validation_email", body.EMail);
			var response = await ExecuteAsync<ZeroSslCertificado>(request, apiKey);
			return response.Data;
		}

		// Get /certificates/{id}/status?access_key={apikey}
		public async Task<ZeroSslStatusResponse> ObterStatusCertificado(string apiKey, string id)
		{
			var request = new RestRequest("/certificates/{id}/status", Method.Get);
			request.AddOrUpdateParameter("id", id, ParameterType.UrlSegment);
			var response = await ExecuteAsync<ZeroSslStatusResponse>(request, apiKey);
			return response.Data;
		}

		// Get /certificates/{id}/download/return?access_key={apikey}&include_cross_signed=0
		public async Task<ZeroSslCrtResponse> ObterCertificadoInLine(string apiKey, string id)
		{
			var request = new RestRequest("/certificates/{id}/download/return", Method.Get);
			request.AddOrUpdateParameter("id", id, ParameterType.UrlSegment);
			request.AddQueryParameter("include_cross_signed", 0);
			var response = await ExecuteAsync<ZeroSslCrtResponse>(request, apiKey);
			return response.Data;
		}

		// Get /certificates/{id}/download?access_key={apikey}&include_cross_signed=0
		public async Task<byte[]> ObterCertificadoDownload(string apiKey, string id)
		{
			var request = new RestRequest("/certificates/{id}/download", Method.Get);
			request.AddOrUpdateParameter("id", id, ParameterType.UrlSegment);
			request.AddQueryParameter("include_cross_signed", 0);
			var response = await ExecuteAsync<byte[]>(request, apiKey);
			return response.Data;
		}

		// Post /certificates/{id}/revoke?access_key={apikey}
		public async Task<ZeroSslRemoveResponse> RevogarCertificado(string apiKey, string id)
		{
			var request = new RestRequest("/certificates/{id}/revoke", Method.Post);
			request.AddOrUpdateParameter("id", id, ParameterType.UrlSegment);
			var response = await ExecuteAsync<ZeroSslRemoveResponse>(request, apiKey);
			return response.Data;
		}

		// Post /certificates/{id}/cancel?access_key={apikey}
		public async Task<ZeroSslRemoveResponse> CancelarCertificado(string apiKey, string id)
		{
			var request = new RestRequest("/certificates/{id}/cancel", Method.Post);
			request.AddOrUpdateParameter("id", id, ParameterType.UrlSegment);
			var response = await ExecuteAsync<ZeroSslRemoveResponse>(request, apiKey);
			return response.Data;
		}

		// Delete /certificates/{id}?access_key={apikey}
		public async Task<ZeroSslRemoveResponse> RemoverCertificado(string apiKey, string id)
		{
			var request = new RestRequest("/certificates/{id}", Method.Delete);
			request.AddOrUpdateParameter("id", id, ParameterType.UrlSegment);
			var response = await ExecuteAsync<ZeroSslRemoveResponse>(request, apiKey);
			return response.Data;
		}

		// Get /certificates?access_key={apikey}
		public async Task<ZeroSslCertificados> ObterCertificados(string apiKey, ZeroSslStatus? status = null, string search = null, int? limit = null, int? page = null)
		{
			var request = new RestRequest("/certificates", Method.Get);

			request.AddOrUpdateParameter("certificate_status", status, ParameterType.QueryString);
			request.AddOrUpdateParameter("search", search, ParameterType.QueryString);
			request.AddOrUpdateParameter("limit", limit, ParameterType.QueryString);
			request.AddOrUpdateParameter("page", page, ParameterType.QueryString);

			var response = await ExecuteAsync<ZeroSslCertificados>(request, apiKey);
			return response.Data;
		}

		// Get /certificates/{id}?access_key={apikey}
		public async Task<ZeroSslCertificado> ObterCertificado(string apiKey, string id)
		{
			var request = new RestRequest("/certificates/{id}", Method.Get);
			request.AddOrUpdateParameter("id", id, ParameterType.UrlSegment);
			var response = await ExecuteAsync<ZeroSslCertificado>(request, apiKey);
			return response.Data;
		}

		private async Task<RestResponse<TResponse>> ExecuteAsync<TResponse>(RestRequest request, string apiKey)
		{
			request.AddQueryParameter("access_key", apiKey);
			return await _restClient.ExecuteAsync<TResponse>(request);
		}
	}
}