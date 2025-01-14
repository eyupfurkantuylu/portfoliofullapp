using PortfolioFullApp.Core.DTOs.HackathonLink;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IHackathonLinkRepository
    {
        Task<IEnumerable<HackathonLinkDto>> GetAllByHackathonIdAsync(string hackathonId);
        Task<HackathonLinkDto> GetByIdAsync(string id);
        Task<IEnumerable<HackathonLinkDto>> CreateManyAsync(string hackathonId, IEnumerable<CreateHackathonLinkDto> links);
        Task<bool> UpdateAsync(string id, UpdateHackathonLinkDto link);
        Task<bool> DeleteByHackathonIdAsync(string hackathonId);
        Task<bool> DeleteByIdAsync(string hackathonLinkId);
    }
}