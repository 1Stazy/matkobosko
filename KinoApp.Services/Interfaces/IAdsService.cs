using System.Collections.Generic;
using System.Threading.Tasks;
using KinoApp.Core.Models; // jeśli chcesz używać modelu Ad w Core, inaczej utwórz model w Services

namespace KinoApp.Services.Interfaces
{
    public interface IAdsService
    {
        /// <summary>Ładuje listę reklam (pliki) z folderu Assets/Ads</summary>
        Task<IEnumerable<string>> LoadAdsAsync();
    }
}
