using PortfolioFullApp.Core.DTOs.SocialMedia;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface ISocialMediaRepository
    {
        Task<IEnumerable<SocialMediaDto>> GetAllByContactIdAsync(string contactId);
        Task<SocialMediaDto> GetByIdAsync(string id);
        Task<IEnumerable<SocialMediaDto>> CreateManyAsync(string contactId, IEnumerable<SocialMedia> socialMedias);
        Task<SocialMediaDto> UpdateAsync(SocialMedia socialMedia);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByContactIdAsync(string contactId);
        Task<bool> UpdateOrderAsync(string id, int newOrder);
        Task<int> GetMaxOrderAsync(string contactId);
    }
}