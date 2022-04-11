using Dapper;
using GestaoApolices.Functions.Implantacao.Repositorio.Abstracao;
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
			using var connection = GetService<IDbConnection>();
			using var dataReader = await connection.ExecuteReaderAsync(CertificadoQuery.CmdSqlSelectUsuariosZeroSslPorUserName, param: new { userName });

			return ObterUsuariosZeroSsl(dataReader).SingleOrDefault();
		}

		public async Task<IEnumerable<Usuario>> ObterUsuariosZeroSsl()
		{
			using var connection = GetService<IDbConnection>();
			using var dataReader = await connection.ExecuteReaderAsync(CertificadoQuery.CmdSqlSelectUsuariosZeroSsl);
			return ObterUsuariosZeroSsl(dataReader);
		}

		private IEnumerable<Usuario> ObterUsuariosZeroSsl(IDataReader dataReader)
		{
			var usuarios = dataReader.CreateObjects<Usuario, CsrConfig, ZeroSslUser, ZeroSslCertificate>(
				groupBy1: u => u.UsuarioId,
				groupBy2: u => u.UsuarioId,
				groupBy3: u => u.ZeroSslUserId,
				groupBy4: u => u.Domain,
				mapJoins: mapJoin
			);

			dataReader.Close();

			return usuarios;

			void mapJoin(Usuario usuario, List<CsrConfig> empresas, List<ZeroSslUser> zeroSslUsers, List<ZeroSslCertificate> zeroSslCertificates)
			{
				var empresa = empresas.FirstOrDefault(e => e.UsuarioId == usuario.UsuarioId);
				empresa.Usuario ??= usuario;
				usuario.ZeroSslConfig ??= empresa;

				foreach (var zeroSslUser in zeroSslUsers.Where(e => e.UsuarioId == usuario.UsuarioId))
				{
					zeroSslUser.Usuario = usuario;
					usuario.ZeroSslUsers.Add(zeroSslUser);

					foreach (var zeroSslCertificate in zeroSslCertificates.Where(c=>c.ZeroSslUserId == zeroSslUser.ZeroSslUserId))
					{
						zeroSslCertificate.ZeroSslUser = zeroSslUser;
						zeroSslUser.ZeroSslCertificates.Add(zeroSslCertificate);
					}
				}
			}
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
}