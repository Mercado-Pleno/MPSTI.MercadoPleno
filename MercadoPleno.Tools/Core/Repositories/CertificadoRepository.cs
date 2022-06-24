using Dapper;
using MercadoPleno.Tools.Application.Abstracao;
using MercadoPleno.Tools.Core.Domains;
using MercadoPleno.Tools.Core.Queries;
using MercadoPleno.Tools.Core.Util;
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
		private readonly UniqueList<Usuario> Usuarios = new UniqueList<Usuario>();
		private readonly UniqueList<CsrConfig> CsrConfigs = new UniqueList<CsrConfig>();
		private readonly UniqueList<ZeroSslUser> ZeroSslUsers = new UniqueList<ZeroSslUser>();
		private readonly UniqueList<ZeroSslCertificate> ZeroSslCertificates = new UniqueList<ZeroSslCertificate>();

		public bool Map(Usuario usuario, CsrConfig csrConfig, ZeroSslUser zeroSslUser, ZeroSslCertificate zeroSslCertificate)
		{
			Usuarios.Add(usuario);
			CsrConfigs.Add(csrConfig);
			ZeroSslUsers.Add(zeroSslUser);
			ZeroSslCertificates.Add(zeroSslCertificate);
			return true;
		}

		public IEnumerable<Usuario> Build()
		{
			foreach (var usuario in Usuarios)
				mapJoin(usuario, CsrConfigs, ZeroSslUsers, ZeroSslCertificates);

			return Usuarios;
		}

		private void mapJoin(Usuario usuario, IEnumerable<CsrConfig> empresas, IEnumerable<ZeroSslUser> zeroSslUsers, IEnumerable<ZeroSslCertificate> zeroSslCertificates)
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