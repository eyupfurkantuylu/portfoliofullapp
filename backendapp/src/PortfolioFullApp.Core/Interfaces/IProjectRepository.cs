using PortfolioFullApp.Core.DTOs.Project;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync();
        Task<ProjectDto> GetByIdAsync(string id);
        Task<ProjectDto> CreateAsync(Project project);
        Task<ProjectDto> UpdateAsync(Project project);
        Task<bool> DeleteAsync(string id);
    }
}