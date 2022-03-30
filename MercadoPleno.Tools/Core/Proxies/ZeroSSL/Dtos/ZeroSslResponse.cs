using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;

namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos
{
	public class ZeroSslResponse
	{
		public bool Valid { get; set; }
		public Error Error { get; set; }
	}
}