namespace MercadoPleno.Tools.Core.Domains
{
	public interface ICsrRequest
	{
		string Pais { get; }
		string Estado { get; }
		string Cidade { get; }
		string Organizacao { get; }
		string Departamento { get; }
		int KeySize { get; }
	}
}