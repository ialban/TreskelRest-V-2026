using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreskelRest.cls
{
    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> lista) where T : ICloneable
        {
            return lista.Select(item => (T)item.Clone()).ToList();
        }
    }
}
