using PortfolioFullApp.Core.DTOs.Skill;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface ISkillRepository
    {
        Task<IEnumerable<SkillDto>> GetAllByProfileIdAsync(string profileId);
        Task<SkillDto> GetByIdAsync(string id);
        Task<SkillDto> CreateAsync(Skill skill);
        Task<SkillDto> UpdateAsync(Skill skill);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateOrderAsync(string id, int newOrder);
        Task<int> GetMaxOrderAsync(string profileId);
        Task<bool> UpdateLevelAsync(string id, int level);
        Task<IEnumerable<SkillDto>> GetByTypeAsync(string profileId, string type);
    }
}