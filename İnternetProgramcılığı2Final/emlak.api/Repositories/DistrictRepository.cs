using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using emlak.api.Models;

namespace emlak.api.Repositories
{
    public class DistrictRepository : GenericRepository<District>
    {
        public DistrictRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<District>> GetAllAsync()
        {
            return await _dbSet
                .Include(d => d.City)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<District>> GetDistrictsByCityAsync(int cityId)
        {
            return await _dbSet
                .Include(d => d.City)
                .Where(d => d.CityId == cityId)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<District> GetDistrictWithCityAsync(int id)
        {
            return await _context.Districts
                .Include(d => d.City)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<District>> GetDistrictsWithPropertyCountAsync(int cityId)
        {
            return await _dbSet
                .Include(d => d.Properties)
                .Where(d => d.CityId == cityId)
                .Select(d => new District
                {
                    Id = d.Id,
                    Name = d.Name,
                    Code = d.Code,
                    CityId = d.CityId,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    Properties = d.Properties.Where(p => p.IsApproved).ToList()
                })
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<District> GetDistrictByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _dbSet.AnyAsync(d => d.Code == code);
        }

        public async Task<bool> ExistsInCityAsync(int cityId, string code)
        {
            return await _dbSet.AnyAsync(d => d.CityId == cityId && d.Code == code);
        }
    }
}