using Dapper;
using PortfolioFullApp.Core.DTOs.NavbarItem;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class NavbarItemRepository : INavbarItemRepository
    {
        private readonly DapperContext _context;

        public NavbarItemRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NavbarItemDto>> GetAllByProfileIdAsync(string profileId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM NavbarItems 
                WHERE ProfileId = @ProfileId 
                ORDER BY [Order]";

            var navbarItems = await connection.QueryAsync<NavbarItemDto>(sql, new { ProfileId = profileId });
            return navbarItems;
        }

        public async Task<NavbarItemDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM NavbarItems WHERE Id = @Id";

            var navbarItem = await connection.QueryFirstOrDefaultAsync<NavbarItemDto>(sql, new { Id = id });
            return navbarItem;
        }

        public async Task<NavbarItemDto> CreateAsync(NavbarItem navbarItem)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Önce maksimum sıra numarasını al
                var maxOrder = await GetMaxOrderAsync(navbarItem.ProfileId);
                navbarItem.Order = maxOrder + 1;

                const string sql = @"
                    INSERT INTO NavbarItems (Id, Title, Link, [Order], IsActive, ProfileId)
                    VALUES (@Id, @Title, @Link, @Order, @IsActive, @ProfileId);
                    SELECT * FROM NavbarItems WHERE Id = @Id";

                var createdItem = await connection.QueryFirstOrDefaultAsync<NavbarItemDto>(sql, navbarItem, transaction);

                transaction.Commit();
                return createdItem;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<NavbarItemDto> UpdateAsync(NavbarItem navbarItem)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE NavbarItems 
                SET Title = @Title,
                    Link = @Link,
                    IsActive = @IsActive
                WHERE Id = @Id;
                SELECT * FROM NavbarItems WHERE Id = @Id";

            var updatedItem = await connection.QueryFirstOrDefaultAsync<NavbarItemDto>(sql, navbarItem);
            return updatedItem;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Önce silinecek öğenin bilgilerini al
                var item = await GetByIdAsync(id);
                if (item == null) return false;

                // Öğeyi sil
                const string deleteSql = "DELETE FROM NavbarItems WHERE Id = @Id";
                await connection.ExecuteAsync(deleteSql, new { Id = id }, transaction);

                // Sıralamayı güncelle
                const string updateOrdersSql = @"
                    UPDATE NavbarItems 
                    SET [Order] = [Order] - 1 
                    WHERE ProfileId = @ProfileId 
                    AND [Order] > @Order";

                await connection.ExecuteAsync(updateOrdersSql,
                    new { ProfileId = item.ProfileId, Order = item.Order },
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
                // Mevcut öğenin bilgilerini al
                var currentItem = await GetByIdAsync(id);
                if (currentItem == null) return false;

                // Sıralama güncelleme
                if (newOrder > currentItem.Order)
                {
                    // Yukarı taşıma
                    const string updateOthersSql = @"
                        UPDATE NavbarItems 
                        SET [Order] = [Order] - 1 
                        WHERE ProfileId = @ProfileId 
                        AND [Order] > @CurrentOrder 
                        AND [Order] <= @NewOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { ProfileId = currentItem.ProfileId, CurrentOrder = currentItem.Order, NewOrder = newOrder },
                        transaction);
                }
                else if (newOrder < currentItem.Order)
                {
                    // Aşağı taşıma
                    const string updateOthersSql = @"
                        UPDATE NavbarItems 
                        SET [Order] = [Order] + 1 
                        WHERE ProfileId = @ProfileId 
                        AND [Order] >= @NewOrder 
                        AND [Order] < @CurrentOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { ProfileId = currentItem.ProfileId, CurrentOrder = currentItem.Order, NewOrder = newOrder },
                        transaction);
                }

                // Seçilen öğenin sırasını güncelle
                const string updateItemSql = @"
                    UPDATE NavbarItems 
                    SET [Order] = @NewOrder 
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateItemSql,
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
                FROM NavbarItems 
                WHERE ProfileId = @ProfileId";

            return await connection.ExecuteScalarAsync<int>(sql, new { ProfileId = profileId });
        }
    }
}