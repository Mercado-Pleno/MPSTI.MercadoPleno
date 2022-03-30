namespace MercadoPleno.Tools.Core.Proxies.CsrGenerator.Dtos
{
	public class CsrResponse
	{
		private const string endTokenCsr = "-----END CERTIFICATE REQUEST-----";
		private const string startTokenCsr = "-----BEGIN PRIVATE KEY-----";

		private readonly string _content;

		private int indexEndCsr => _content.IndexOf(endTokenCsr) + endTokenCsr.Length + 1;
		private int indexStartPK => _content.IndexOf(startTokenCsr);
		public string Certificate => _content.Substring(0, indexEndCsr);
		public string PrivateKey => _content.Substring(indexStartPK);

		public CsrResponse(string content) => _content = content;
	}
}