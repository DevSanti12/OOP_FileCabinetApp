using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOP_FileCabinetApp.settings;

namespace OOP_FileCabinetApp.src
{
    public class DocumentCache
    {
        private readonly Dictionary<string, (string CardInfo, DateTime Expiration)> _cache = new(); // Cache storage
        private readonly Dictionary<Type, DocumentCacheSettings> _cacheSettings; // Settings per document type

        public DocumentCache(Dictionary<Type, DocumentCacheSettings> cacheSettings)
        {
            _cacheSettings = cacheSettings;
        }

        public bool TryGet(string documentNumber, out string cardInfo)
        {
            if (_cache.TryGetValue(documentNumber, out var cachedItem))
            {
                if (cachedItem.Expiration > DateTime.UtcNow || cachedItem.Expiration == DateTime.MaxValue)
                {
                    cardInfo = cachedItem.CardInfo;
                    return true; // Cache valid
                }

                // Cache expired
                _cache.Remove(documentNumber);
            }

            cardInfo = null;
            return false;
        }

        public void Add(string documentNumber, string cardInfo, Type documentType)
        {
            if (!_cacheSettings.TryGetValue(documentType, out var settings) || settings.DoNotCache)
            {
                return; // Do not cache if settings specify no caching
            }

            var expiration = settings.CacheDuration.HasValue
                ? DateTime.UtcNow.Add(settings.CacheDuration.Value)
                : DateTime.MaxValue;

            _cache[documentNumber] = (cardInfo, expiration);
        }

        public void Clear(string documentNumber)
        {
            _cache.Remove(documentNumber); // Manually clear cache for a specific document
        }
    }
}
