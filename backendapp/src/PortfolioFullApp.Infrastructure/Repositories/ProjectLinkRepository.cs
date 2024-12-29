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

        public async Task<IEnumerable<ProjectLinkDto>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM ProjectLinks
                ORDER BY [Order]";

            var projectLinks = await connection.QueryAsync<ProjectLinkDto>(sql);
            return projectLinks;
        }

        public async Task<IEnumerable<ProjectLinkDto>> GetByProjectIdAsync(string projectId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM ProjectLinks 
                WHERE ProjectId = @ProjectId
                ORDER BY [Order]";

            var projectLinks = await connection.QueryAsync<ProjectLinkDto>(sql, new { ProjectId = projectId });
            return projectLinks;
        }

        public async Task<ProjectLinkDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM ProjectLinks WHERE Id = @Id";

            var projectLink = await connection.QueryFirstOrDefaultAsync<ProjectLinkDto>(sql, new { Id = id });
            return projectLink;
        }

        public async Task<ProjectLinkDto> CreateAsync(ProjectLink projectLink)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                INSERT INTO ProjectLinks (Id, Type, Href, Icon, ProjectId, [Order])
                VALUES (@Id, @Type, @Href, @Icon, @ProjectId, @Order);
                SELECT * FROM ProjectLinks WHERE Id = @Id";

            var createdProjectLink = await connection.QueryFirstOrDefaultAsync<ProjectLinkDto>(sql, projectLink);
            return createdProjectLink;
        }

        public async Task<ProjectLinkDto> UpdateAsync(ProjectLink projectLink)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE ProjectLinks 
                SET Type = @Type,
                    Href = @Href,
                    Icon = @Icon,
                    ProjectId = @ProjectId,
                    [Order] = @Order
                WHERE Id = @Id;
                SELECT * FROM ProjectLinks WHERE Id = @Id";

            var updatedProjectLink = await connection.QueryFirstOrDefaultAsync<ProjectLinkDto>(sql, projectLink);
            return updatedProjectLink;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM ProjectLinks WHERE Id = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
    }
}