namespace MercadoPleno.Tools.Core.Queries
{
	public class CertificadoQuery
	{
		public static string CmdSqlSelectUsuariosZeroSsl => @"
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
    , ZSC.Id As ZeroSslCertificateId
    , ZSC.CertificateId
    , ZSC.Domain
    , ZSC.Status
    , ZSC.CertificateRequest
    , ZSC.PrivateKey
    , ZSC.CertificateCRT
    , ZSC.CertificateSigned
From Usuario U
Inner Join ZeroSslConfig      Cfg On Cfg.UsuarioId = U.Id
Inner Join ZeroSslUser        ZSU On ZSU.UsuarioId = U.Id
Left  Join ZeroSslCertificate ZSC On ZSU.ZeroSslUserId = ZSU.Id
";

        public static string CmdSqlSelectUsuariosZeroSslPorUserName => CmdSqlSelectUsuariosZeroSsl + @"
Where (U.Username = @userName)
";

        public static string CmdSqlSelectZeroSslCertificate => @"
Select
    ZSC.Id As ZeroSslCertificateId
    , ZSC.ZeroSslUserId
    , ZSC.CertificateId
    , ZSC.Domain
    , ZSC.Status
    , ZSC.CertificateRequest
    , ZSC.PrivateKey
    , ZSC.CertificateCRT
    , ZSC.CertificateSigned
From ZeroSslCertificate ZSC
Where (ZSC.Domain = @domain)
";


        public static string CmdSqlInsertZeroSslCertificate => @"
Insert Into ZeroSslCertificate (
      ZeroSslUserId
    , CertificateId
    , Domain
    , Status
    , CertificateRequest
    , PrivateKey
    , CertificateCRT
    , CertificateSigned
) Values (
      @zeroSslUserId
    , @certificateId
    , @domain
    , @status
    , @certificateRequest
    , @privateKey
    , @certificateCRT
    , @certificateSigned
);";


        public static string CmdSqlDeleteZeroSslCertificate => @"
Delete From ZeroSslCertificate Where (Domain = @domain)
";

        public static string CmdSqlUpdateZeroSslCertificate => @"
Update ZSC Set
      ZSC.CertificateCRT = @certificateCRT
    , ZSC.CertificateSigned = @certificateSigned
    , ZSC.Status = @status
From ZeroSslCertificate ZSC
Where (ZSC.ZeroSslUserId = @zeroSslUserId)
And (ZSC.CertificateId = @certificateId)
And (ZSC.Domain = @domain)
";
    }
}