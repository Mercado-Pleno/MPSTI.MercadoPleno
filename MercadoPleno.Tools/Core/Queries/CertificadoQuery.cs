namespace MercadoPleno.Tools.Core.Queries
{
	public class CertificadoQuery
	{
		public static string CmdSqlObterUsuariosZeroSsl => @"
Select
    U.Id As UsuarioId
    , U.Username
    , U.Senha
    , Cfg.Id As EmpresaId
    , Cfg.Pais
    , Cfg.Estado
    , Cfg.Cidade
    , Cfg.Organizacao
    , Cfg.Departamento
    , Cfg.SufixDNS
    , Cfg.KeySize
    , ZSU.Id As ZeroSslUserId
    , ZSU.Login
    , ZSU.Password
    , ZSU.ApiKey
From Usuario U
Inner Join ZeroSslConfig Cfg On Cfg.UsuarioId = U.Id
Inner Join ZeroSslUser   ZSU On ZSU.UsuarioId = U.Id
";
	}
}