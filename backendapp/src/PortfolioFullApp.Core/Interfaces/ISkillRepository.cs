using PortfolioFullApp.Core.DTOs.Skill;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface ISkillRepository
    {
        Task<IEnumerable<SkillDto>> GetAllAsync();
        Task<SkillDto> GetByIdAsync(string id);
        Task<SkillDto> CreateAsync(Skill skill);
        Task<SkillDto> UpdateAsync(Skill skill);
        Task<bool> DeleteAsync(string id);
    }
}