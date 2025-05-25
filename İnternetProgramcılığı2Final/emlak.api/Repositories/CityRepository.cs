using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using emlak.api.Models;

namespace emlak.api.Repositories
{
    public class CityRepository : GenericRepository<City>
    {
        public CityRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<City>> GetAllCitiesWithDistrictsAsync()
        {
            return await _dbSet
                .Include(c => c.Districts)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<City> GetCityWithDistrictsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Districts)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<City>> GetCitiesWithPropertyCountAsync()
        {
            return await _dbSet
                .Include(c => c.Properties)
                .Select(c => new City
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Properties = c.Properties.Where(p => p.IsApproved).ToList()
                })
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<City> GetCityByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _dbSet.AnyAsync(c => c.Code == code);
        }
    }
}