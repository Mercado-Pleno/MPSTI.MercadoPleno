namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos
{
	public class ZeroSslCriaRequest
	{
		public int StrictDomains { get; set; } = 0;

		public int ValidityDays { get; set; } = 90;

		public string Domain { get; set; }

		public string CSR { get; set; }
	}
}