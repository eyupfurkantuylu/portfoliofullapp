using Dapper;
using PortfolioFullApp.Core.DTOs.Skill;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly DapperContext _context;

        public SkillRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SkillDto>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM Skills
                ORDER BY [Order]";

            var skills = await connection.QueryAsync<SkillDto>(sql);
            return skills;
        }

        public async Task<SkillDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM Skills WHERE Id = @Id";

            var skill = await connection.QueryFirstOrDefaultAsync<SkillDto>(sql, new { Id = id });
            return skill;
        }

        public async Task<SkillDto> CreateAsync(Skill skill)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                INSERT INTO Skills (Id, Name, [Order])
                VALUES (@Id, @Name, @Order);
                SELECT * FROM Skills WHERE Id = @Id";

            var createdSkill = await connection.QueryFirstOrDefaultAsync<SkillDto>(sql, skill);
            return createdSkill;
        }

        public async Task<SkillDto> UpdateAsync(Skill skill)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE Skills 
                SET Name = @Name,
                    [Order] = @Order
                WHERE Id = @Id;
                SELECT * FROM Skills WHERE Id = @Id";

            var updatedSkill = await connection.QueryFirstOrDefaultAsync<SkillDto>(sql, skill);
            return updatedSkill;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM Skills WHERE Id = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
    }
}