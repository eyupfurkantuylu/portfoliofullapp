using PortfolioFullApp.Core.DTOs.Contact;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IContactRepository
    {
        Task<IEnumerable<ContactDto>> GetAllAsync();
        Task<ContactDto> GetByIdAsync(string id);
        Task<ContactDto> CreateAsync(Contact contact);
        Task<ContactDto> UpdateAsync(Contact contact);
        Task<bool> DeleteAsync(string id);
    }
}