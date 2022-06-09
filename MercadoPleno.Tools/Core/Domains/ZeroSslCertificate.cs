using MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos;

namespace MercadoPleno.Tools.Core.Domains
{
	public class ZeroSslCertificate
	{
		public long Id { get; set; }
		public long ZeroSslUserId { get; set; }
		public string CertificateId { get; set; }
		public string Domain { get; set; }
		public string CertificateRequest { get; set; }
		public string PrivateKey { get; set; }
		public string CertificateCrt { get; set; }
		public string CertificateSigned { get; set; }
		public ZeroSslStatus Status { get; set; }
		public ZeroSslUser ZeroSslUser { get; set; }

		public ZeroSslCertificate() { }

		public static ZeroSslCertificate Create(string domain, string content)
		{
			return new ZeroSslCertificate
			{
				Domain = domain,
				CertificateRequest = GetCertificateRequest(content),
				PrivateKey = GetPrivateKey(content),
			};
		}

		private static string GetCertificateRequest(string content)
		{
			const string endTokenCsr = "-----END CERTIFICATE REQUEST-----";
			var indexEndCsr = content.IndexOf(endTokenCsr) + endTokenCsr.Length + 1;
			return content.Substring(0, indexEndCsr);
		}

		private static string GetPrivateKey(string content)
		{
			const string startTokenPK = "-----BEGIN PRIVATE KEY-----";
			var indexStartPK = content.IndexOf(startTokenPK);
			return content.Substring(indexStartPK);
		}
	}
}