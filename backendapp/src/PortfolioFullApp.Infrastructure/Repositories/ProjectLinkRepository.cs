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
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                SELECT * FROM ProjectLinks
                ORDER BY [Order]";
                try
                {
                    var projectLinks = await connection.QueryAsync<ProjectLinkDto>(sql);
                    return projectLinks;
                }
                catch (Exception ex)
                {
                    throw new Exception("Proje linkleri getirilirken bir hata oluştu", ex);
                }
            }
        }

        public async Task<IEnumerable<ProjectLinkDto>> GetByProjectIdAsync(string projectId)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                SELECT * FROM ProjectLinks 
                WHERE ProjectId = @ProjectId
                ORDER BY [Order]";

                try
                {
                    var projectLinks = await connection.QueryAsync<ProjectLinkDto>(sql, new { ProjectId = projectId });
                    return projectLinks;
                }
                catch (Exception ex)
                {
                    throw new Exception("Proje linkleri getirilirken bir hata oluştu", ex);
                }
            }
        }

        public async Task<ProjectLinkDto> GetByIdAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "SELECT * FROM ProjectLinks WHERE Id = @Id";
                try
                {
                    var projectLink = await connection.QueryFirstOrDefaultAsync<ProjectLinkDto>(sql, new { Id = id });
                    return projectLink ?? throw new Exception("Proje linki bulunamadı");
                }
                catch (Exception ex)
                {
                    throw new Exception("Proje linki getirilirken bir hata oluştu", ex);
                }
            }
        }

        public async Task<ProjectLinkDto> CreateAsync(CreateProjectLinkDto projectLink)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                INSERT INTO ProjectLinks (Id, Type, Href, Icon, ProjectId, [Order])
                OUTPUT INSERTED.Id
                VALUES (NEWID(), @Type, @Href, @Icon, @ProjectId, @Order)";

                try
                {
                    var createdProjectLink = await connection.ExecuteScalarAsync<string>(sql, projectLink);
                    return await GetByIdAsync(createdProjectLink!);
                }
                catch (Exception ex)
                {
                    throw new Exception("Proje linki oluşturulurken bir hata oluştu", ex);
                }
            }
        }

        public async Task<ProjectLinkDto> UpdateAsync(UpdateProjectLinkDto projectLink)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                UPDATE ProjectLinks 
                SET Type = @Type,
                    Href = @Href,
                    Icon = @Icon,
                    ProjectId = @ProjectId,
                    [Order] = @Order
                WHERE Id = @Id";

                try
                {
                    await connection.ExecuteAsync(sql, projectLink);
                    return await GetByIdAsync(projectLink.Id);
                }
                catch (Exception ex)
                {
                    throw new Exception("Proje linki güncellenirken bir hata oluştu", ex);
                }
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "DELETE FROM ProjectLinks WHERE Id = @Id";
                try
                {
                    var result = await connection.ExecuteAsync(sql, new { Id = id });
                    return result > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Proje linki silinirken bir hata oluştu", ex);
                }
            }
        }
    }
}
