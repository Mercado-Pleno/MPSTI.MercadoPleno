using System.Collections.Generic;
using System.Linq;

namespace MercadoPleno.Tools.Core.Util
{
	public interface IUnique { long Id { get; } }

	public class UniqueList<T> : List<T> where T : class, IUnique
	{
		public new void Add(T item)
		{
			if ((item != null) && !this.Any(i => i.Id == item.Id))
			{
				base.Add(item);
			}
		}
	}
}