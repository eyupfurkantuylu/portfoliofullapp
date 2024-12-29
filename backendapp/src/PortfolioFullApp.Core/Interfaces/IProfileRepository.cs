using PortfolioFullApp.Core.DTOs.Profile;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IProfileRepository
    {
        Task<ProfileDto> GetByIdAsync(string id);
        Task<ProfileDto> GetByUserIdAsync(string userId);
        Task<ProfileDto> CreateAsync(Profile profile);
        Task<ProfileDto> UpdateAsync(Profile profile);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string userId);
        Task<bool> UpdateAboutAsync(string id, string about);
        Task<bool> UpdateSummaryAsync(string id, string summary);
    }
}