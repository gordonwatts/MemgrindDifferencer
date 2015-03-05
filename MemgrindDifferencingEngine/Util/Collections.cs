using System.Collections.Generic;

namespace MemgrindDifferencingEngine.Util
{
    static class Collections
    {
        /// <summary>
        /// Add everything in a list to a hash.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hash"></param>
        /// <param name="stuff"></param>
        public static void AddRange<T>(this HashSet<T> hash, IEnumerable<T> stuff)
        {
            foreach (var i in stuff)
            {
                hash.Add(i);
            }
        }

        /// <summary>
        /// Create a hash set from an enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            var r = new HashSet<T>();
            r.AddRange(source);
            return r;
        }
    }
}
