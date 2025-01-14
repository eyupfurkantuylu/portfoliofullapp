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

        public async Task<IEnumerable<NavbarItemDto>> GetAllAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                SELECT * FROM NavbarItems
                ORDER BY [Order]";
                try
                {
                    var navbarItems = await connection.QueryAsync<NavbarItemDto>(sql);
                    return navbarItems;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<NavbarItemDto> GetByIdAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "SELECT * FROM NavbarItems WHERE Id = @Id";
                try
                {
                    var navbarItem = await connection.QueryFirstOrDefaultAsync<NavbarItemDto>(sql, new { Id = id });
                    return navbarItem ?? throw new Exception("Navbar item not found");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<NavbarItemDto> CreateAsync(CreateNavbarItemDto navbarItem)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                INSERT INTO NavbarItems (Id, Href, Icon, Label, [Order], CreatedAt)
                OUTPUT INSERTED.Id
                VALUES (NEWID(), @Href, @Icon, @Label, CAST(@Order as int), GETDATE())";
                var parameters = new
                {
                    Href = navbarItem.Href,
                    Icon = navbarItem.Icon,
                    Label = navbarItem.Label,
                    Order = navbarItem.Order
                };
                try
                {
                    var createdNavbarItemId = await connection.ExecuteScalarAsync<string>(sql, navbarItem);
                    return await GetByIdAsync(createdNavbarItemId);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<NavbarItemDto> UpdateAsync(UpdateNavbarItemDto navbarItem)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                UPDATE NavbarItems 
                SET Href = @Href,
                    Icon = @Icon,
                    Label = @Label,
                    [Order] = @Order,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id;
                SELECT * FROM NavbarItems WHERE Id = @Id";
                try
                {
                    var updatedNavbarItem = await connection.QueryFirstOrDefaultAsync<NavbarItemDto>(sql, navbarItem);
                    return updatedNavbarItem ?? throw new Exception("Navbar item not found");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "DELETE FROM NavbarItems WHERE Id = @Id";
                try
                {
                    var result = await connection.ExecuteAsync(sql, new { Id = id });
                    return result > 0;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}