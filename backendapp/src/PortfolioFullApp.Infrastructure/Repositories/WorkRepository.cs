using Dapper;
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

        public async Task<IEnumerable<WorkDto>> GetAllByProfileIdAsync(string profileId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM Works 
                WHERE ProfileId = @ProfileId 
                ORDER BY [Order] DESC";

            var works = await connection.QueryAsync<WorkDto>(sql, new { ProfileId = profileId });
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
            using var transaction = connection.BeginTransaction();

            try
            {
                // Önce maksimum sıra numarasını al
                var maxOrder = await GetMaxOrderAsync(work.ProfileId);
                work.Order = maxOrder + 1;

                const string sql = @"
                    INSERT INTO Works (
                        Id, CompanyName, Position, Description,
                        StartDate, EndDate, IsCurrent, [Order],
                        CompanyUrl, CompanyLogoUrl, ProfileId
                    ) VALUES (
                        @Id, @CompanyName, @Position, @Description,
                        @StartDate, @EndDate, @IsCurrent, @Order,
                        @CompanyUrl, @CompanyLogoUrl, @ProfileId
                    );
                    SELECT * FROM Works WHERE Id = @Id";

                var createdWork = await connection.QueryFirstOrDefaultAsync<WorkDto>(sql, work, transaction);

                transaction.Commit();
                return createdWork;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<WorkDto> UpdateAsync(Work work)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE Works 
                SET CompanyName = @CompanyName,
                    Position = @Position,
                    Description = @Description,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    IsCurrent = @IsCurrent,
                    CompanyUrl = @CompanyUrl,
                    CompanyLogoUrl = @CompanyLogoUrl
                WHERE Id = @Id;
                SELECT * FROM Works WHERE Id = @Id";

            var updatedWork = await connection.QueryFirstOrDefaultAsync<WorkDto>(sql, work);
            return updatedWork;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var work = await GetByIdAsync(id);
                if (work == null) return false;

                const string deleteWorkSql = "DELETE FROM Works WHERE Id = @Id";
                await connection.ExecuteAsync(deleteWorkSql, new { Id = id }, transaction);

                const string updateOrdersSql = @"
                    UPDATE Works 
                    SET [Order] = [Order] - 1 
                    WHERE ProfileId = @ProfileId 
                    AND [Order] > @Order";

                await connection.ExecuteAsync(updateOrdersSql,
                    new { ProfileId = work.ProfileId, Order = work.Order },
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

        public async Task<bool> UpdateOrderAsync(string id, int newOrder)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var currentWork = await GetByIdAsync(id);
                if (currentWork == null) return false;

                if (newOrder > currentWork.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE Works 
                        SET [Order] = [Order] - 1 
                        WHERE ProfileId = @ProfileId 
                        AND [Order] > @CurrentOrder 
                        AND [Order] <= @NewOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new
                        {
                            ProfileId = currentWork.ProfileId,
                            CurrentOrder = currentWork.Order,
                            NewOrder = newOrder
                        },
                        transaction);
                }
                else if (newOrder < currentWork.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE Works 
                        SET [Order] = [Order] + 1 
                        WHERE ProfileId = @ProfileId 
                        AND [Order] >= @NewOrder 
                        AND [Order] < @CurrentOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new
                        {
                            ProfileId = currentWork.ProfileId,
                            CurrentOrder = currentWork.Order,
                            NewOrder = newOrder
                        },
                        transaction);
                }

                const string updateWorkSql = @"
                    UPDATE Works 
                    SET [Order] = @NewOrder 
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateWorkSql,
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

        public async Task<int> GetMaxOrderAsync(string profileId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT ISNULL(MAX([Order]), 0) 
                FROM Works 
                WHERE ProfileId = @ProfileId";

            return await connection.ExecuteScalarAsync<int>(sql, new { ProfileId = profileId });
        }

        public async Task<bool> UpdateIsCurrentAsync(string id, bool isCurrent)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var work = await GetByIdAsync(id);
                if (work == null) return false;

                // Eğer yeni iş aktif olarak işaretleniyorsa, diğer aktif işleri pasife çek
                if (isCurrent)
                {
                    const string updateOthersSql = @"
                        UPDATE Works 
                        SET IsCurrent = 0 
                        WHERE ProfileId = @ProfileId 
                        AND Id != @Id";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { ProfileId = work.ProfileId, Id = id },
                        transaction);
                }

                const string updateWorkSql = @"
                    UPDATE Works 
                    SET IsCurrent = @IsCurrent
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateWorkSql,
                    new { Id = id, IsCurrent = isCurrent },
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

        public async Task<IEnumerable<WorkDto>> GetCurrentWorkAsync(string profileId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM Works 
                WHERE ProfileId = @ProfileId 
                AND IsCurrent = 1
                ORDER BY StartDate DESC";

            var works = await connection.QueryAsync<WorkDto>(sql, new { ProfileId = profileId });
            return works;
        }
    }
}