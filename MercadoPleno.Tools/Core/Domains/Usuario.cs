using System.Collections.Generic;

namespace MercadoPleno.Tools.Core.Domains
{
	public class Usuario
	{
		public long Id { get; set; }
		public string Username { get; set; }
		public string Senha { get; set; }
		public CsrConfig ZeroSslConfig { get; set; }
		public List<ZeroSslUser> ZeroSslUsers { get; set; } = new List<ZeroSslUser>();
	}
}