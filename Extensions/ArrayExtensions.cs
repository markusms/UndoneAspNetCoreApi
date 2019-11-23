using System.Collections.Generic;
using System.Linq;

namespace UndoneAspNetCoreApi.Extensions
{
    public static class ArrayExtensions
    {
        public static int NullableCount<T>(this IEnumerable<T> collection)
        {
            return collection == null ? 0 : collection.Count();
        }
    }
}
