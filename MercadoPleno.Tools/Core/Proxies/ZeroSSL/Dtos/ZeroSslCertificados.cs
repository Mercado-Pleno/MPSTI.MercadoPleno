using System.Collections.Generic;

namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos
{
	public class ZeroSslCertificados : ZeroSslResponse
	{
		public int Total_count { get; set; }
		public int Result_count { get; set; }
		public int Page { get; set; }
		public int Limit { get; set; }
		public List<ZeroSslCertificado> Results { get; set; }
	}
}