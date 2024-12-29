using PortfolioFullApp.Core.DTOs.Hackathon;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IHackathonRepository
    {
        Task<IEnumerable<HackathonDto>> GetAllByProfileIdAsync(string profileId);
        Task<HackathonDto> GetByIdAsync(string id);
        Task<HackathonDto> CreateAsync(Hackathon hackathon);
        Task<HackathonDto> UpdateAsync(Hackathon hackathon);
        Task<bool> DeleteAsync(string id);
    }
}