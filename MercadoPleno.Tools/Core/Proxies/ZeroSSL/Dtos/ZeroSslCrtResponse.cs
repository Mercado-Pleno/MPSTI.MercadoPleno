using Newtonsoft.Json;

namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos
{
	public class ZeroSslCrtResponse : ZeroSslResponse
	{
		[JsonProperty("certificate.crt")]
		public string CertificateCrt { get; set; }

		[JsonProperty("ca_bundle.crt")]
		public string CertificateSigned { get; set; }
	}
}