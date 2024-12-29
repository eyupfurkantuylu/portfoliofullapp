using Dapper;
using System.Text.Json;
using PortfolioFullApp.Core.DTOs.Work;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class WorkRepository : IWorkRepository
    {
        private readonly DapperContext _context;

        public WorkRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkDto>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM Works
                ORDER BY [Order]";

            var works = await connection.QueryAsync<WorkDto>(sql);
            return works;
        }

        public async Task<WorkDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM Works WHERE Id = @Id";

            var work = await connection.QueryFirstOrDefaultAsync<WorkDto>(sql, new { Id = id });
            return work;
        }

        public async Task<WorkDto> CreateAsync(Work work)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                INSERT INTO Works (Id, Company, Href, Badges, Location, Title, LogoUrl, Start, [End], Description, [Order])
                VALUES (@Id, @Company, @Href, @Badges, @Location, @Title, @LogoUrl, @Start, @End, @Description, @Order);
                SELECT * FROM Works WHERE Id = @Id";

            var badgesJson = JsonSerializer.Serialize(work.Badges);
            var parameters = new
            {
                work.Id,
                work.Company,
                work.Href,
                Badges = badgesJson,
                work.Location,
                work.Title,
                work.LogoUrl,
                work.Start,
                work.End,
                work.Description,
                work.Order
            };

            var createdWork = await connection.QueryFirstOrDefaultAsync<WorkDto>(sql, parameters);
            return createdWork;
        }

        public async Task<WorkDto> UpdateAsync(Work work)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE Works 
                SET Company = @Company,
                    Href = @Href,
                    Badges = @Badges,
                    Location = @Location,
                    Title = @Title,
                    LogoUrl = @LogoUrl,
                    Start = @Start,
                    [End] = @End,
                    Description = @Description,
                    [Order] = @Order
                WHERE Id = @Id;
                SELECT * FROM Works WHERE Id = @Id";

            var badgesJson = JsonSerializer.Serialize(work.Badges);
            var parameters = new
            {
                work.Id,
                work.Company,
                work.Href,
                Badges = badgesJson,
                work.Location,
                work.Title,
                work.LogoUrl,
                work.Start,
                work.End,
                work.Description,
                work.Order
            };

            var updatedWork = await connection.QueryFirstOrDefaultAsync<WorkDto>(sql, parameters);
            return updatedWork;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM Works WHERE Id = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
    }
}