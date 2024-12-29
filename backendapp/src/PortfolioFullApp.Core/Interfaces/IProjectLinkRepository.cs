using PortfolioFullApp.Core.DTOs.ProjectLink;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IProjectLinkRepository
    {
        Task<IEnumerable<ProjectLinkDto>> GetAllByProjectIdAsync(string projectId);
        Task<ProjectLinkDto> GetByIdAsync(string id);
        Task<IEnumerable<ProjectLinkDto>> CreateManyAsync(string projectId, IEnumerable<ProjectLink> links);
        Task<bool> DeleteByProjectIdAsync(string projectId);
        Task<bool> UpdateOrderAsync(string id, int newOrder);
        Task<int> GetMaxOrderAsync(string projectId);
    }
}