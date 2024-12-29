using PortfolioFullApp.Core.DTOs.Project;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<ProjectDto>> GetAllByProfileIdAsync(string profileId);
        Task<ProjectDto> GetByIdAsync(string id);
        Task<ProjectDto> CreateAsync(Project project);
        Task<ProjectDto> UpdateAsync(Project project);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateOrderAsync(string id, int newOrder);
        Task<int> GetMaxOrderAsync(string profileId);
        Task<bool> UpdateStatusAsync(string id, bool isActive);
    }
}