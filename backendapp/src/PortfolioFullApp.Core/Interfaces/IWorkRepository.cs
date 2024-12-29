using PortfolioFullApp.Core.DTOs.Work;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IWorkRepository
    {
        Task<IEnumerable<WorkDto>> GetAllByProfileIdAsync(string profileId);
        Task<WorkDto> GetByIdAsync(string id);
        Task<WorkDto> CreateAsync(Work work);
        Task<WorkDto> UpdateAsync(Work work);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateOrderAsync(string id, int newOrder);
        Task<int> GetMaxOrderAsync(string profileId);
        Task<bool> UpdateIsCurrentAsync(string id, bool isCurrent);
        Task<IEnumerable<WorkDto>> GetCurrentWorkAsync(string profileId);
    }
}