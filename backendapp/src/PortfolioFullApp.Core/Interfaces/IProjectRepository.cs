using PortfolioFullApp.Core.DTOs.Project;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync();
        Task<ProjectDto> GetByIdAsync(string id);
        Task<ProjectDto> CreateAsync(CreateProjectDto project);
        Task<ProjectDto> UpdateAsync(UpdateProjectDto project);
        Task<bool> DeleteAsync(string id);
    }
}