using System.Collections.Generic;

namespace MercadoPleno.Tools.Core.Domains
{
	public class ZeroSslUser
	{
		public long UsuarioId { get; set; }
		public long ZeroSslUserId { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string ApiKey { get; set; }
		public Usuario Usuario { get; set; }
		public List<ZeroSslCertificate> ZeroSslCertificates { get; set; } = new List<ZeroSslCertificate>();
	}
}