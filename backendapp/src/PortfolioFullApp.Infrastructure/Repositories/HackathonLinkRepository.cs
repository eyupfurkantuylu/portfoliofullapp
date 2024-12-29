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
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM HackathonLinks 
                WHERE HackathonId = @HackathonId
                ORDER BY Title";

            var links = await connection.QueryAsync<HackathonLinkDto>(sql, new { HackathonId = hackathonId });
            return links;
        }

        public async Task<HackathonLinkDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM HackathonLinks WHERE Id = @Id";

            var link = await connection.QueryFirstOrDefaultAsync<HackathonLinkDto>(sql, new { Id = id });
            return link;
        }

        public async Task<IEnumerable<HackathonLinkDto>> CreateManyAsync(string hackathonId, IEnumerable<HackathonLink> links)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string sql = @"
                    INSERT INTO HackathonLinks (Id, Title, Url, HackathonId)
                    VALUES (@Id, @Title, @Url, @HackathonId)";

                foreach (var link in links)
                {
                    link.HackathonId = hackathonId;
                    await connection.ExecuteAsync(sql, link, transaction);
                }

                transaction.Commit();
                return await GetAllByHackathonIdAsync(hackathonId);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteByHackathonIdAsync(string hackathonId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM HackathonLinks WHERE HackathonId = @HackathonId";

            var result = await connection.ExecuteAsync(sql, new { HackathonId = hackathonId });
            return result > 0;
        }
    }
}