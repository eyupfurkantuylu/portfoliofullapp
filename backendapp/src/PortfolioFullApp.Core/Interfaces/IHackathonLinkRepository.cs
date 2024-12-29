using PortfolioFullApp.Core.DTOs.HackathonLink;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IHackathonLinkRepository
    {
        Task<IEnumerable<HackathonLinkDto>> GetAllByHackathonIdAsync(string hackathonId);
        Task<HackathonLinkDto> GetByIdAsync(string id);
        Task<IEnumerable<HackathonLinkDto>> CreateManyAsync(string hackathonId, IEnumerable<HackathonLink> links);
        Task<bool> DeleteByHackathonIdAsync(string hackathonId);
    }
}