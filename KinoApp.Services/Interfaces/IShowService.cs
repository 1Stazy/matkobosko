using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KinoApp.Core.Models;

namespace KinoApp.Services.Interfaces
{
    public interface IShowService
    {
        /// <summary> Pobierz listę seansów w przedziale dat. </summary>
        Task<IEnumerable<Seans>> GetShowsAsync(DateTime from, DateTime to);

        /// <summary> Pobierz seans po Id (int). </summary>
        Task<Seans?> GetShowByIdAsync(int id);

        /// <summary> Utwórz nowy seans. </summary>
        Task CreateShowAsync(Seans show);

        /// <summary> Zaktualizuj seans. </summary>
        Task UpdateShowAsync(Seans show);

        /// <summary> Usuń seans po Id (int). </summary>
        Task DeleteShowAsync(int id);
    }
}
