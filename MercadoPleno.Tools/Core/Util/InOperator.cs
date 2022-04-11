using System;
using System.Linq;

namespace MercadoPleno.Tools.Core.Services
{
	public static class InOperator
	{
		public static bool In<T>(this T self, params T[] args) => args.Contains(self);
	}
}