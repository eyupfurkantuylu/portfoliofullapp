using PortfolioFullApp.Core.DTOs.Contact;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IContactRepository
    {
        Task<IEnumerable<ContactDto>> GetAllAsync();
        Task<ContactDto> GetByIdAsync(string id);
        Task<ContactDto> CreateAsync(CreateContactDto createContactDto);
        Task<ContactDto> UpdateAsync(UpdateContactDto contact);
        Task<bool> DeleteAsync(string id);
    }
}