namespace DataObjectProcessor
{
    public interface IDataProcessor
    {
        /// <summary>
        /// Returns an IAsyncEnumberable of a generic type mapped to the underlying data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IAsyncEnumerable<T> GetAsyncEnumerable<T>();

        /// <summary>
        /// Returns an IEnumberable of a generic type mapped to the underlying data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<IEnumerable<T>> GetEnumerableAsync<T>();

        /// <summary>
        /// Returns an Array of a generic type mapped to the underlying data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T[]> GetArrayAsync<T>();
    }
}