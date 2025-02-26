


namespace AdeAuth.Services.Extensions
{
    internal static class CollectionExtension
    {
      
        /// <summary>
        /// Perform iteration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="action">Action to run</param>
        public static void Foreach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                return;
            foreach (T item in collection)
            {
                action(item);
            }
        }
    }
}
