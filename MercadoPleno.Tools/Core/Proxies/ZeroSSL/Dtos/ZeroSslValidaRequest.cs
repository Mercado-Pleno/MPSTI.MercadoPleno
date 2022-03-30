namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos
{
	public class ZeroSslValidaRequest
	{
		public string Method { get; set; } = "EMAIL";

		public string EMail { get; set; }
	}
}