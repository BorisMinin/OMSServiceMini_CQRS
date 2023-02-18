namespace OMSServiceMini.AppHelpers
{
    // все методы кэширования: получение данных, установка данных и удаление кэшированных данных
    public interface ICacheService
    {
        /// <summary>
        /// получение кэшированных данных
        /// </summary>
        /// <typeparam name="T">тип кэшированных данных</typeparam>
        /// <param name="cacheKey">ключ, по которому кэшируются данные</param>
        /// <param name="value">место хранения кэшированных данных</param>
        /// <returns></returns>
        bool TryGet<T>(string cacheKey, out T value);

        /// <summary>
        /// установка кэшированных данных
        /// </summary>
        /// <typeparam name="T">тип кэшированных данных</typeparam>
        /// <param name="cacheKey">ключ, по которому кэшируются данные</param>
        /// <param name="value">данные для кэширования</param>
        /// <returns>кэшированные данные</returns>
        T Set<T>(string cacheKey, T value);

        /// <summary>
        /// удаление кэшированных данных
        /// </summary>
        /// <param name="cacheKey">ключ кэшированных данных для удаления</param>
        void Remove(string cacheKey);
    }
}