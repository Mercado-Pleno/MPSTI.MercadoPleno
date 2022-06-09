using Dapper;
using MercadoPleno.Tools.Application.Abstracao;
using MercadoPleno.Tools.Core.Domains;
using MercadoPleno.Tools.Core.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MercadoPleno.Tools.Core.Repositories
{
	public class CertificadoRepository : BaseClass
	{
		public CertificadoRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public async Task<Usuario> ObterUsuarioZeroSsl(string userName)
		{
			var joinner = new CertificadoJoinner();
			using var connection = GetService<IDbConnection>();
			await connection.QueryAsync<Usuario, CsrConfig, ZeroSslUser, ZeroSslCertificate, bool>(
				sql: CertificadoQuery.CmdSqlSelectUsuariosZeroSslPorUserName,
				param: new { userName },
				map: joinner.Map
			);

			return joinner.Build().FirstOrDefault();
		}

		public async Task<IEnumerable<Usuario>> ObterUsuariosZeroSsl()
		{
			var joinner = new CertificadoJoinner();
			using var connection = GetService<IDbConnection>();
			await connection.QueryAsync<Usuario, CsrConfig, ZeroSslUser, ZeroSslCertificate, bool>(
				sql: CertificadoQuery.CmdSqlSelectUsuariosZeroSsl,
				map: joinner.Map
			);

			return joinner.Build();
		}

		public async Task Substituir(ZeroSslCertificate certificate)
		{
			using var connection = GetService<IDbConnection>();
			await connection.ExecuteAsync(CertificadoQuery.CmdSqlDeleteZeroSslCertificate, param: certificate);
			await connection.ExecuteAsync(CertificadoQuery.CmdSqlInsertZeroSslCertificate, param: certificate);
		}

		public async Task Atualizar(ZeroSslCertificate certificate)
		{
			using var connection = GetService<IDbConnection>();
			await connection.ExecuteAsync(CertificadoQuery.CmdSqlUpdateZeroSslCertificate, param: certificate);
		}
	}

	public class CertificadoJoinner
	{
		private readonly List<Usuario> Usuarios = new List<Usuario>();
		private readonly List<CsrConfig> CsrConfigs = new List<CsrConfig>();
		private readonly List<ZeroSslUser> ZeroSslUsers = new List<ZeroSslUser>();
		private readonly List<ZeroSslCertificate> ZeroSslCertificates = new List<ZeroSslCertificate>();

		public bool Map(Usuario usuario, CsrConfig csrConfig, ZeroSslUser zeroSslUser, ZeroSslCertificate zeroSslCertificate)
		{
			if (usuario != null) Usuarios.Add(usuario);
			if (csrConfig != null) CsrConfigs.Add(csrConfig);
			if (zeroSslUser != null) ZeroSslUsers.Add(zeroSslUser);
			if (zeroSslCertificate != null) ZeroSslCertificates.Add(zeroSslCertificate);
			return true;
		}

		public IEnumerable<Usuario> Build()
		{
			var usuarios = Usuarios.DistinctBy(u => u.Id).ToArray();

			foreach (var usuario in usuarios)
				mapJoin(usuario, CsrConfigs, ZeroSslUsers, ZeroSslCertificates);

			return usuarios;
		}

		private void mapJoin(Usuario usuario, List<CsrConfig> empresas, List<ZeroSslUser> zeroSslUsers, List<ZeroSslCertificate> zeroSslCertificates)
		{
			var empresa = empresas.FirstOrDefault(e => e.UsuarioId == usuario.Id);
			empresa.Usuario ??= usuario;
			usuario.ZeroSslConfig ??= empresa;

			foreach (var zeroSslUser in zeroSslUsers.Where(e => e.UsuarioId == usuario.Id))
			{
				zeroSslUser.Usuario = usuario;
				usuario.ZeroSslUsers.Add(zeroSslUser);

				foreach (var zeroSslCertificate in zeroSslCertificates.Where(c => c.ZeroSslUserId == zeroSslUser.Id && !string.IsNullOrWhiteSpace(c.Domain)))
				{
					zeroSslCertificate.ZeroSslUser = zeroSslUser;
					zeroSslUser.ZeroSslCertificates.Add(zeroSslCertificate);
				}
			}
		}
	}
}