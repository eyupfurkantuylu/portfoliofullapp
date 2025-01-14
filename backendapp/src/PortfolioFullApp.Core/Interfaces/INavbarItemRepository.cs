using PortfolioFullApp.Core.DTOs.NavbarItem;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface INavbarItemRepository
    {
        Task<IEnumerable<NavbarItemDto>> GetAllAsync();
        Task<NavbarItemDto> GetByIdAsync(string id);
        Task<NavbarItemDto> CreateAsync(CreateNavbarItemDto navbarItem);
        Task<NavbarItemDto> UpdateAsync(UpdateNavbarItemDto navbarItem);
        Task<bool> DeleteAsync(string id);
    }
}