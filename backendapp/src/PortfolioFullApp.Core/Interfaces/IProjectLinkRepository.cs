using PortfolioFullApp.Core.DTOs.ProjectLink;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IProjectLinkRepository
    {
        Task<IEnumerable<ProjectLinkDto>> GetAllAsync();
        Task<IEnumerable<ProjectLinkDto>> GetByProjectIdAsync(string projectId);
        Task<ProjectLinkDto> GetByIdAsync(string id);
        Task<ProjectLinkDto> CreateAsync(CreateProjectLinkDto projectLink);
        Task<ProjectLinkDto> UpdateAsync(UpdateProjectLinkDto projectLink);
        Task<bool> DeleteAsync(string id);
    }
}