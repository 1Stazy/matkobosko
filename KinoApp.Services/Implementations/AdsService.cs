using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KinoApp.Services.Interfaces;

namespace KinoApp.Services.Implementations
{
    public class AdsService : IAdsService
    {
        private readonly string _adsFolder;

        public AdsService()
        {
            _adsFolder = Path.Combine(System.AppContext.BaseDirectory, "Assets", "Ads");
            if (!Directory.Exists(_adsFolder))
                Directory.CreateDirectory(_adsFolder);
        }

        public Task<IEnumerable<string>> LoadAdsAsync()
        {
            var files = Directory.EnumerateFiles(_adsFolder)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".mp4") || f.EndsWith(".gif"));
            return Task.FromResult(files.AsEnumerable());
        }
    }
}
