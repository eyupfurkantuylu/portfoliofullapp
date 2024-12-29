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

        public async Task<IEnumerable<EducationDto>> GetAllByProfileIdAsync(string profileId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM Educations 
                WHERE ProfileId = @ProfileId 
                ORDER BY StartDate DESC";

            var educations = await connection.QueryAsync<EducationDto>(sql, new { ProfileId = profileId });
            return educations;
        }

        public async Task<EducationDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM Educations WHERE Id = @Id";

            var education = await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, new { Id = id });
            return education;
        }

        public async Task<EducationDto> CreateAsync(Education education)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                INSERT INTO Educations (
                    Id, 
                    SchoolName, 
                    Department, 
                    Degree, 
                    StartDate, 
                    EndDate, 
                    Description,
                    ProfileId
                ) VALUES (
                    @Id, 
                    @SchoolName, 
                    @Department, 
                    @Degree, 
                    @StartDate, 
                    @EndDate, 
                    @Description,
                    @ProfileId
                );
                SELECT * FROM Educations WHERE Id = @Id";

            var createdEducation = await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, education);
            return createdEducation;
        }

        public async Task<EducationDto> UpdateAsync(Education education)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE Educations 
                SET SchoolName = @SchoolName,
                    Department = @Department,
                    Degree = @Degree,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    Description = @Description
                WHERE Id = @Id;
                SELECT * FROM Educations WHERE Id = @Id";

            var updatedEducation = await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, education);
            return updatedEducation;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM Educations WHERE Id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }
    }
}