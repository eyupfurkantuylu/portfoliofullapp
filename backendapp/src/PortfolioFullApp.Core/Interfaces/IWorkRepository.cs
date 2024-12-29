using PortfolioFullApp.Core.DTOs.Work;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IWorkRepository
    {
        Task<IEnumerable<WorkDto>> GetAllAsync();
        Task<WorkDto> GetByIdAsync(string id);
        Task<WorkDto> CreateAsync(Work work);
        Task<WorkDto> UpdateAsync(Work work);
        Task<bool> DeleteAsync(string id);
    }
}