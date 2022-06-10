using MercadoPleno.Tools.Application.Abstracao;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MercadoPleno.Tools.Core.Util
{
	public sealed class HttpClientManager : BaseClass, IHttpClientFactory
	{
		private readonly Dictionary<string, HttpClient> _httpClients = new();
		private readonly object _access = new();

		public HttpClientManager(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public HttpClient CreateClient(string name) => CreateClientImpl(name, null);

		private HttpClient CreateClientImpl(string name, string baseAddress)
		{
			if (!_httpClients.TryGetValue(name, out var httpClient))
			{
				lock (_access)
				{
					if (!_httpClients.TryGetValue(name, out httpClient))
					{
						httpClient = new HttpClient();
						if (!string.IsNullOrWhiteSpace(baseAddress))
							httpClient.BaseAddress = new Uri(baseAddress);
						_httpClients.Add(name, httpClient);
					}
				}
			}
			return httpClient;
		}
	}
}