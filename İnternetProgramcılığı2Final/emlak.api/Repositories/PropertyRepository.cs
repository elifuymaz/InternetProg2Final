using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using emlak.api.Models;

namespace emlak.api.Repositories
{
    public class PropertyRepository : GenericRepository<Property>
    {
        public PropertyRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Property>> GetApprovedPropertiesAsync()
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Features)
                .Include(p => p.Images)
                .Where(p => p.IsApproved)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPendingPropertiesAsync()
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Features)
                .Include(p => p.Images)
                .Include(p => p.User)
                .Where(p => !p.IsApproved && p.Status != "Rejected")
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByCityAsync(int cityId)
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Features)
                .Include(p => p.Images)
                .Where(p => p.CityId == cityId && p.IsApproved)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByDistrictAsync(int districtId)
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Features)
                .Include(p => p.Images)
                .Where(p => p.DistrictId == districtId && p.IsApproved)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByUserAsync(string userId)
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Features)
                .Include(p => p.Images)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Property> GetPropertyWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Features)
                .Include(p => p.Images)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task ApprovePropertyAsync(int id, string approvedBy)
        {
            var property = await _dbSet.FindAsync(id);
            if (property != null)
            {
                property.IsApproved = true;
                property.Status = "Approved";
                property.ApprovedAt = DateTime.UtcNow;
                property.ApprovedBy = approvedBy;
                _dbSet.Update(property);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectPropertyAsync(int id, string rejectedBy)
        {
            var property = await _dbSet.FindAsync(id);
            if (property != null)
            {
                property.IsApproved = false;
                property.Status = "Rejected";
                property.RejectedAt = DateTime.UtcNow;
                property.RejectedBy = rejectedBy;
                _dbSet.Update(property);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(
            string searchTerm,
            int? cityId = null,
            int? districtId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minSquareMeters = null,
            int? maxSquareMeters = null,
            bool? isForSale = null,
            bool? isForRent = null)
        {
            var query = _dbSet
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Features)
                .Include(p => p.Images)
                .Where(p => p.IsApproved);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }

            if (cityId.HasValue)
                query = query.Where(p => p.CityId == cityId.Value);

            if (districtId.HasValue)
                query = query.Where(p => p.DistrictId == districtId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (minSquareMeters.HasValue)
                query = query.Where(p => p.SquareMeters >= minSquareMeters.Value);

            if (maxSquareMeters.HasValue)
                query = query.Where(p => p.SquareMeters <= maxSquareMeters.Value);

            if (isForSale.HasValue)
                query = query.Where(p => p.IsForSale == isForSale.Value);

            if (isForRent.HasValue)
                query = query.Where(p => p.IsForRent == isForRent.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetRejectedPropertiesAsync()
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Features)
                .Include(p => p.Images)
                .Where(p => p.Status == "Rejected")
                .ToListAsync();
        }
    }
}