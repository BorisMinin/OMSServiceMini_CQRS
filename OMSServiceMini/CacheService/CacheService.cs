using OMSServiceMini.AppHelpers;
using OMSServiceMini.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OMSServiceMini.CacheService
{
    public static class CacheService
    {
        public static List<T> GetCache<T>(
            this ICacheService _cacheService, 
            List<T> modelList, string cacheKey)
            where T : class
        {
            if (!_cacheService.TryGet(cacheKey, out IReadOnlyList<T> cachedList))
                return _cacheService.Set(cacheKey, modelList);

            return modelList;
        }

        public static async Task RefreshCacheAsync<T>(
            this ICacheService _cacheService, 
            T model, string cacheKey) 
            where T : class
        {
            _cacheService.Remove(cacheKey);
            //var list = await _unitOfWork.Query<T>().ToListAsync();
            _cacheService.Set(cacheKey, model);
        }

        //public static IQueryable<T> GetCache<T>(this ICacheService _cacheService, IUnitOfWork _unitOfWork, string cacheKey) where T : class
        //{
        //    if (!_cacheService.TryGet(cacheKey, out IReadOnlyList<T> cachedList))
        //    {
        //        var query = _unitOfWork.Query<T>().AsNoTracking();

        //        cachedList = query.ToList();
        //        _cacheService.Set(cacheKey, cachedList);
        //        return query;
        //    }
        //    return _unitOfWork.Query<T>().AsNoTracking();
        //}

        //public static async Task RefreshCacheAsync<T>(this ICacheService _cacheService, IUnitOfWork _unitOfWork, string cacheKey) where T : class
        //{
        //    _cacheService.Remove(cacheKey);
        //    var list = await _unitOfWork.Query<T>().ToListAsync();
        //    _cacheService.Set(cacheKey, list);
        //}
    }
}