namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos
{
	public class ZeroSslStatusResponse : ZeroSslResponse
	{
		public bool Validation_completed { get; set; }
		public dynamic Details { get; set; }
	}
}