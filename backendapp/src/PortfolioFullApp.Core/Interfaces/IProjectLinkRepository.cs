using PortfolioFullApp.Core.DTOs.ProjectLink;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IProjectLinkRepository
    {
        Task<IEnumerable<ProjectLinkDto>> GetAllAsync();
        Task<IEnumerable<ProjectLinkDto>> GetByProjectIdAsync(string projectId);
        Task<ProjectLinkDto> GetByIdAsync(string id);
        Task<ProjectLinkDto> CreateAsync(ProjectLink projectLink);
        Task<ProjectLinkDto> UpdateAsync(ProjectLink projectLink);
        Task<bool> DeleteAsync(string id);
    }
}