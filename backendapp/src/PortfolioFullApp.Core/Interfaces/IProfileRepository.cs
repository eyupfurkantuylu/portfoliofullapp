using PortfolioFullApp.Core.DTOs.Profile;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IProfileRepository
    {
        Task<IEnumerable<ProfileDto>> GetAllAsync();
        Task<ProfileDto> GetByIdAsync(string id);
        Task<ProfileDto> CreateAsync(CreateProfileDto profile);
        Task<ProfileDto> UpdateAsync(UpdateProfileDto profile);
        Task<bool> DeleteAsync(string id);
    }
}