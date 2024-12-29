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
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM NavbarItems
                ORDER BY [Order]";

            var navbarItems = await connection.QueryAsync<NavbarItemDto>(sql);
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
            const string sql = @"
                INSERT INTO NavbarItems (Id, Href, Icon, Label, [Order])
                VALUES (@Id, @Href, @Icon, @Label, @Order);
                SELECT * FROM NavbarItems WHERE Id = @Id";

            var createdNavbarItem = await connection.QueryFirstOrDefaultAsync<NavbarItemDto>(sql, navbarItem);
            return createdNavbarItem;
        }

        public async Task<NavbarItemDto> UpdateAsync(NavbarItem navbarItem)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE NavbarItems 
                SET Href = @Href,
                    Icon = @Icon,
                    Label = @Label,
                    [Order] = @Order
                WHERE Id = @Id;
                SELECT * FROM NavbarItems WHERE Id = @Id";

            var updatedNavbarItem = await connection.QueryFirstOrDefaultAsync<NavbarItemDto>(sql, navbarItem);
            return updatedNavbarItem;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM NavbarItems WHERE Id = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
    }
}