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
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM Education
                ORDER BY [Order]";

            var educations = await connection.QueryAsync<EducationDto>(sql);
            return educations;
        }

        public async Task<EducationDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM Education WHERE Id = @Id";

            var education = await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, new { Id = id });
            return education;
        }

        public async Task<EducationDto> CreateAsync(Education education)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                INSERT INTO Education (Id, School, Href, Degree, LogoUrl, Start, [End], [Order])
                VALUES (@Id, @School, @Href, @Degree, @LogoUrl, @Start, @End, @Order);
                SELECT * FROM Education WHERE Id = @Id";

            var createdEducation = await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, education);
            return createdEducation;
        }

        public async Task<EducationDto> UpdateAsync(Education education)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE Education 
                SET School = @School,
                    Href = @Href,
                    Degree = @Degree,
                    LogoUrl = @LogoUrl,
                    Start = @Start,
                    [End] = @End,
                    [Order] = @Order
                WHERE Id = @Id;
                SELECT * FROM Education WHERE Id = @Id";

            var updatedEducation = await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, education);
            return updatedEducation;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM Education WHERE Id = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
    }
}