using PortfolioFullApp.Core.DTOs.Education;
using PortfolioFullApp.Core.Entities;

namespace PortfolioFullApp.Core.Interfaces
{
    public interface IEducationRepository
    {
        Task<IEnumerable<EducationDto>> GetAllAsync();
        Task<EducationDto> GetByIdAsync(string id);
        Task<EducationDto> CreateAsync(Education education);
        Task<EducationDto> UpdateAsync(Education education);
        Task<bool> DeleteAsync(string id);
    }
}