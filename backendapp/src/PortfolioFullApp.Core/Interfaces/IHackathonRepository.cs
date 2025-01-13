using PortfolioFullApp.Core.DTOs.Hackathon;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IHackathonRepository
    {
        Task<IEnumerable<HackathonDto>> GetAllAsync();
        Task<HackathonDto> GetByIdAsync(string id);
        Task<HackathonDto> CreateAsync(CreateHackathonDto hackathon);
        Task<HackathonDto> UpdateAsync(UpdateHackathonDto hackathon);
        Task<bool> DeleteAsync(string id);
    }
}