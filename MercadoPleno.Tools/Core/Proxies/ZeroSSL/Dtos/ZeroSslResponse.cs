namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos
{
	public class ZeroSslResponse
	{
		public bool Valid { get; set; }
		public Error Error { get; set; }

		public ZeroSslResponse() => Valid = true;
	}
}