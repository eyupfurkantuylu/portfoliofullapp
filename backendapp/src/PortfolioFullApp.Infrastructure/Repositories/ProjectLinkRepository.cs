using Dapper;
using PortfolioFullApp.Core.DTOs.ProjectLink;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class ProjectLinkRepository : IProjectLinkRepository
    {
        private readonly DapperContext _context;

        public ProjectLinkRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectLinkDto>> GetAllByProjectIdAsync(string projectId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM ProjectLinks 
                WHERE ProjectId = @ProjectId 
                ORDER BY [Order]";

            var links = await connection.QueryAsync<ProjectLinkDto>(sql, new { ProjectId = projectId });
            return links;
        }

        public async Task<ProjectLinkDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM ProjectLinks WHERE Id = @Id";

            var link = await connection.QueryFirstOrDefaultAsync<ProjectLinkDto>(sql, new { Id = id });
            return link;
        }

        public async Task<IEnumerable<ProjectLinkDto>> CreateManyAsync(string projectId, IEnumerable<ProjectLink> links)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var maxOrder = await GetMaxOrderAsync(projectId);
                var currentOrder = maxOrder;

                const string sql = @"
                    INSERT INTO ProjectLinks (Id, Title, Url, Icon, [Order], ProjectId)
                    VALUES (@Id, @Title, @Url, @Icon, @Order, @ProjectId)";

                foreach (var link in links)
                {
                    currentOrder++;
                    link.ProjectId = projectId;
                    link.Order = currentOrder;
                    await connection.ExecuteAsync(sql, link, transaction);
                }

                transaction.Commit();
                return await GetAllByProjectIdAsync(projectId);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteByProjectIdAsync(string projectId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM ProjectLinks WHERE ProjectId = @ProjectId";

            var result = await connection.ExecuteAsync(sql, new { ProjectId = projectId });
            return result > 0;
        }

        public async Task<bool> UpdateOrderAsync(string id, int newOrder)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var currentLink = await GetByIdAsync(id);
                if (currentLink == null) return false;

                if (newOrder > currentLink.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE ProjectLinks 
                        SET [Order] = [Order] - 1 
                        WHERE ProjectId = @ProjectId 
                        AND [Order] > @CurrentOrder 
                        AND [Order] <= @NewOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { ProjectId = currentLink.ProjectId, CurrentOrder = currentLink.Order, NewOrder = newOrder },
                        transaction);
                }
                else if (newOrder < currentLink.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE ProjectLinks 
                        SET [Order] = [Order] + 1 
                        WHERE ProjectId = @ProjectId 
                        AND [Order] >= @NewOrder 
                        AND [Order] < @CurrentOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { ProjectId = currentLink.ProjectId, CurrentOrder = currentLink.Order, NewOrder = newOrder },
                        transaction);
                }

                const string updateLinkSql = @"
                    UPDATE ProjectLinks 
                    SET [Order] = @NewOrder 
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateLinkSql,
                    new { Id = id, NewOrder = newOrder },
                    transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> GetMaxOrderAsync(string projectId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT ISNULL(MAX([Order]), 0) 
                FROM ProjectLinks 
                WHERE ProjectId = @ProjectId";

            return await connection.ExecuteScalarAsync<int>(sql, new { ProjectId = projectId });
        }
    }
}