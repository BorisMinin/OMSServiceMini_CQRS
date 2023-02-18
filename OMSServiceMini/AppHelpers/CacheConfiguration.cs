namespace OMSServiceMini.AppHelpers
{
    /// <summary>
    /// реализация конфигурационного файла с помощью IOptions Pattern
    /// </summary>
    public class CacheConfiguration
    {
        // время в часах, в течение которого кэш будет очищен, даже если есть запросы от пользователей
        public int AbsoluteExpirationInHours { get; set; }
        // время в минутах, в течении которого кэш будет очищен
        public int SlidingExpirationInMinutes { get; set; }
    }
}