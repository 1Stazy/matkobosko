using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KinoApp.Core.Models;
using KinoApp.Infrastructure.Data;
using KinoApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KinoApp.Services.Implementations
{
    public class ShowService : IShowService
    {
        private readonly AppDbContext _db;

        public ShowService(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateShowAsync(Seans show)
        {
            await _db.Seanse.AddAsync(show);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteShowAsync(int id)
        {
            var s = await _db.Seanse.FindAsync(id);
            if (s != null)
            {
                _db.Seanse.Remove(s);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Seans?> GetShowByIdAsync(int id)
        {
            return await _db.Seanse
                .Include(s => s.Film)
                .Include(s => s.Sala)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Seans>> GetShowsAsync(DateTime from, DateTime to)
        {
            return await _db.Seanse
                .Include(s => s.Film)
                .Include(s => s.Sala)
                .Where(s => s.DataCzas >= from && s.DataCzas <= to)
                .ToListAsync();
        }

        public async Task UpdateShowAsync(Seans show)
        {
            _db.Seanse.Update(show);
            await _db.SaveChangesAsync();
        }
    }
}
