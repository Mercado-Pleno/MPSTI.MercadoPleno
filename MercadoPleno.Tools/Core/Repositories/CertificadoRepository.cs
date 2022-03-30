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

		public async Task<IEnumerable<Usuario>> ObterUsuariosZeroSsl()
		{
			using var connection = GetService<IDbConnection>();
			connection.Open();

			using var dataReader = await connection.ExecuteReaderAsync(CertificadoQuery.CmdSqlObterUsuariosZeroSsl);

			var usuarios = dataReader.CreateObjects<Usuario, CsrConfig, ZeroSslUser>(
				groupBy1: u => u.UsuarioId,
				groupBy2: u => u.UsuarioId,
				groupBy3: u => u.ZeroSslUserId,
				mapJoins: mapJoin
			);

			dataReader.Close();
			connection.Close();

			return usuarios;
		}

		private void mapJoin(Usuario usuario, List<CsrConfig> empresas, List<ZeroSslUser> zeroSslUsers)
		{
			var empresa = empresas.FirstOrDefault(e => e.UsuarioId == usuario.UsuarioId);
			empresa.Usuario ??= usuario;
			usuario.ZeroSslConfig ??= empresa;

			foreach (var zeroSslUser in zeroSslUsers.Where(e => e.UsuarioId == usuario.UsuarioId))
			{
				zeroSslUser.Usuario = usuario;
				usuario.ZeroSslUsers.Add(zeroSslUser);
			}
		}
	}
}