using PortfolioFullApp.Core.DTOs.NavbarItem;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface INavbarItemRepository
    {
        Task<IEnumerable<NavbarItemDto>> GetAllByProfileIdAsync(string profileId);
        Task<NavbarItemDto> GetByIdAsync(string id);
        Task<NavbarItemDto> CreateAsync(NavbarItem navbarItem);
        Task<NavbarItemDto> UpdateAsync(NavbarItem navbarItem);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateOrderAsync(string id, int newOrder);
        Task<int> GetMaxOrderAsync(string profileId);
    }
}