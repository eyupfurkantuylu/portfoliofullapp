using PortfolioFullApp.Core.DTOs.Profile;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IProfileRepository
    {
        Task<IEnumerable<ProfileDto>> GetAllAsync();
        Task<ProfileDto> GetByIdAsync(string id);
        Task<ProfileDto> CreateAsync(Profile profile);
        Task<ProfileDto> UpdateAsync(Profile profile);
        Task<bool> DeleteAsync(string id);
    }
}