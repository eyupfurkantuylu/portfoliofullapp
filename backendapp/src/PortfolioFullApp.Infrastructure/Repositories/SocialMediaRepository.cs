using Dapper;
using PortfolioFullApp.Core.DTOs.SocialMedia;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class SocialMediaRepository : ISocialMediaRepository
    {
        private readonly DapperContext _context;

        public SocialMediaRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SocialMediaDto>> GetAllByContactIdAsync(string contactId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM SocialMedia 
                WHERE ContactId = @ContactId 
                ORDER BY [Order]";

            var socialMedias = await connection.QueryAsync<SocialMediaDto>(sql, new { ContactId = contactId });
            return socialMedias;
        }

        public async Task<SocialMediaDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM SocialMedia WHERE Id = @Id";

            var socialMedia = await connection.QueryFirstOrDefaultAsync<SocialMediaDto>(sql, new { Id = id });
            return socialMedia;
        }

        public async Task<IEnumerable<SocialMediaDto>> CreateManyAsync(string contactId, IEnumerable<SocialMedia> socialMedias)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var maxOrder = await GetMaxOrderAsync(contactId);
                var currentOrder = maxOrder;

                const string sql = @"
                    INSERT INTO SocialMedia (
                        Id, Platform, Url, Icon, [Order], ContactId
                    ) VALUES (
                        @Id, @Platform, @Url, @Icon, @Order, @ContactId
                    )";

                foreach (var socialMedia in socialMedias)
                {
                    currentOrder++;
                    socialMedia.ContactId = contactId;
                    socialMedia.Order = currentOrder;
                    await connection.ExecuteAsync(sql, socialMedia, transaction);
                }

                transaction.Commit();
                return await GetAllByContactIdAsync(contactId);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<SocialMediaDto> UpdateAsync(SocialMedia socialMedia)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE SocialMedia 
                SET Platform = @Platform,
                    Url = @Url,
                    Icon = @Icon
                WHERE Id = @Id;
                SELECT * FROM SocialMedia WHERE Id = @Id";

            var updatedSocialMedia = await connection.QueryFirstOrDefaultAsync<SocialMediaDto>(sql, socialMedia);
            return updatedSocialMedia;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var socialMedia = await GetByIdAsync(id);
                if (socialMedia == null) return false;

                const string deleteSql = "DELETE FROM SocialMedia WHERE Id = @Id";
                await connection.ExecuteAsync(deleteSql, new { Id = id }, transaction);

                const string updateOrdersSql = @"
                    UPDATE SocialMedia 
                    SET [Order] = [Order] - 1 
                    WHERE ContactId = @ContactId 
                    AND [Order] > @Order";

                await connection.ExecuteAsync(updateOrdersSql,
                    new { ContactId = socialMedia.ContactId, Order = socialMedia.Order },
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

        public async Task<bool> DeleteByContactIdAsync(string contactId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM SocialMedia WHERE ContactId = @ContactId";

            var result = await connection.ExecuteAsync(sql, new { ContactId = contactId });
            return result > 0;
        }

        public async Task<bool> UpdateOrderAsync(string id, int newOrder)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var currentSocialMedia = await GetByIdAsync(id);
                if (currentSocialMedia == null) return false;

                if (newOrder > currentSocialMedia.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE SocialMedia 
                        SET [Order] = [Order] - 1 
                        WHERE ContactId = @ContactId 
                        AND [Order] > @CurrentOrder 
                        AND [Order] <= @NewOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new
                        {
                            ContactId = currentSocialMedia.ContactId,
                            CurrentOrder = currentSocialMedia.Order,
                            NewOrder = newOrder
                        },
                        transaction);
                }
                else if (newOrder < currentSocialMedia.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE SocialMedia 
                        SET [Order] = [Order] + 1 
                        WHERE ContactId = @ContactId 
                        AND [Order] >= @NewOrder 
                        AND [Order] < @CurrentOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new
                        {
                            ContactId = currentSocialMedia.ContactId,
                            CurrentOrder = currentSocialMedia.Order,
                            NewOrder = newOrder
                        },
                        transaction);
                }

                const string updateSocialMediaSql = @"
                    UPDATE SocialMedia 
                    SET [Order] = @NewOrder 
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateSocialMediaSql,
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

        public async Task<int> GetMaxOrderAsync(string contactId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT ISNULL(MAX([Order]), 0) 
                FROM SocialMedia 
                WHERE ContactId = @ContactId";

            return await connection.ExecuteScalarAsync<int>(sql, new { ContactId = contactId });
        }
    }
}