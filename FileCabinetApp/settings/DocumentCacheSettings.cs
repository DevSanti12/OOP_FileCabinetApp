using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_FileCabinetApp.settings
{
    public class DocumentCacheSettings
    {
        public TimeSpan? CacheDuration { get; set; } // Time duration for cache expiration
        public bool DoNotCache { get; set; } = false; // Flag for bypassing cache
    }
}
