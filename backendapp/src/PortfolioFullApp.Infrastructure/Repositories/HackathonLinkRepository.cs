using Dapper;
using PortfolioFullApp.Core.DTOs.HackathonLink;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class HackathonLinkRepository : IHackathonLinkRepository
    {
        private readonly DapperContext _context;

        public HackathonLinkRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HackathonLinkDto>> GetAllByHackathonIdAsync(string hackathonId)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                    SELECT * FROM HackathonLinks 
                    WHERE HackathonId = @HackathonId
                    ORDER BY Title";
                try
                {
                    var links = await connection.QueryAsync<HackathonLinkDto>(sql, new { HackathonId = hackathonId });
                    return links;
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<HackathonLinkDto> GetByIdAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "SELECT * FROM HackathonLinks WHERE Id = @Id";

                try
                {
                    var link = await connection.QueryFirstOrDefaultAsync<HackathonLinkDto>(sql, new { Id = id });
                    return link;
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<HackathonLinkDto>> CreateManyAsync(string hackathonId,
            IEnumerable<CreateHackathonLinkDto> links)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    const string sql = @"
                    INSERT INTO HackathonLinks (Id, Title, Icon, Href, HackathonId, [Order], CreatedAt)
                    VALUES (NEWID(), @Title, @Icon, @Href, @HackathonId, @Order, GETDATE())";
                    foreach (var link in links)
                    {
                        link.HackathonId = hackathonId;
                        await connection.ExecuteAsync(sql, link);
                    }
                    return await GetAllByHackathonIdAsync(hackathonId);
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<bool> UpdateAsync(string id, UpdateHackathonLinkDto link)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "UPDATE HackathonLinks SET Title = @Title, Icon = @Icon, Href = @Href, [Order] = @Order, UpdatedAt = GETDATE() WHERE Id = @Id";
                try
                {
                    var result = await connection.ExecuteAsync(sql, link);
                    return result > 0;
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteByHackathonIdAsync(string hackathonId)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "DELETE FROM HackathonLinks WHERE HackathonId = @HackathonId";

                var result = await connection.ExecuteAsync(sql, new { HackathonId = hackathonId });
                return result > 0;
            }
        }

        public async Task<bool> DeleteByIdAsync(string hackathonLinkId)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "DELETE FROM HackathonLinks WHERE Id = @Id";
                var result = await connection.ExecuteAsync(sql, new { Id = hackathonLinkId });
                return result > 0;
            }
        }
    }
}