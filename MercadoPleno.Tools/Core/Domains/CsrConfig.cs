namespace MercadoPleno.Tools.Core.Domains
{
	public class CsrConfig : ICsrRequest
	{
		public long UsuarioId { get; set; }
		public string Pais { get; set; }
		public string Estado { get; set; }
		public string Cidade { get; set; }
		public string Organizacao { get; set; }
		public string Departamento { get; set; }
		public string SufixDNS { get; set; }
		public int KeySize { get; set; }
		public Usuario Usuario { get; set; }
	}
}