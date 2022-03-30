using System.IO;
using System.Reflection;

namespace MercadoPleno.Tools.Application
{
	public class FileInfoUtil
	{
		public static FileInfo GetFileInfoFromRoot(params string[] paths)
		{
			var root = GetRootDirectoryInfo();
			return new FileInfo(Path.Combine(root.FullName, Path.Combine(paths)));
		}

		public static DirectoryInfo GetRootDirectoryInfo()
		{
			var assemblyFileInfo = new FileInfo(Assembly.GetCallingAssembly()?.Location);
			return assemblyFileInfo?.Directory?.Parent ?? new DirectoryInfo("./");
		}
	}

}