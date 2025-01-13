using Dapper;
using PortfolioFullApp.Core.DTOs.Education;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class EducationRepository : IEducationRepository
    {
        private readonly DapperContext _context;

        public EducationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EducationDto>> GetAllAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                SELECT * FROM Education
                ORDER BY [Order]";
                try
                {
                    var educations = await connection.QueryAsync<EducationDto>(sql);
                    return educations;
                }
                catch
                {
                    throw;
                }

            }

        }

        public async Task<EducationDto> GetByIdAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "SELECT * FROM Education WHERE Id = @Id";
                try
                {
                    var education = await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, new { Id = id });
                    return education ?? throw new KeyNotFoundException($"Education with ID {id} was not found.");
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<EducationDto> CreateAsync(Education education)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                INSERT INTO Education (Id, School, Href, Degree, LogoUrl, Start, [End], [Order], CreatedAt)
                VALUES (@Id, @School, @Href, @Degree, @LogoUrl, @Start, @End, @Order, GETDATE());
                SELECT * FROM Education WHERE Id = @Id";
                try
                {
                    var createdEducation = await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, education);
                    return createdEducation ?? throw new KeyNotFoundException($"Education with ID {education.Id} was not found.");
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<EducationDto> UpdateAsync(Education education)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                UPDATE Education 
                SET School = @School,
                    Href = @Href,
                    Degree = @Degree,
                    LogoUrl = @LogoUrl,
                    Start = @Start,
                    [End] = @End,
                    [Order] = @Order,
                    UpdatedAt = GETDATE() 
                WHERE Id = @Id;
                SELECT * FROM Education WHERE Id = @Id";

                try
                {
                    var updatedEducation = await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, education);
                    return updatedEducation ?? throw new KeyNotFoundException($"Education with ID {education.Id} was not found.");
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "DELETE FROM Education WHERE Id = @Id";
                try
                {
                    var result = await connection.ExecuteAsync(sql, new { Id = id });
                    return result > 0;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}