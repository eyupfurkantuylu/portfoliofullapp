using Dapper;
using System.Text.Json;
using PortfolioFullApp.Core.DTOs.Project;
using PortfolioFullApp.Core.DTOs.ProjectLink;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DapperContext _context;

        public ProjectRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT p.*, pl.*
                FROM Projects p
                LEFT JOIN ProjectLinks pl ON p.Id = pl.ProjectId
                ORDER BY p.[Order]";

            var projectDict = new Dictionary<string, ProjectDto>();

            await connection.QueryAsync<Project, ProjectLink, ProjectDto>(
                sql,
                (project, link) =>
                {
                    if (!projectDict.TryGetValue(project.Id, out var projectDto))
                    {
                        projectDto = new ProjectDto
                        {
                            Id = project.Id,
                            Title = project.Title,
                            Href = project.Href,
                            Dates = project.Dates,
                            Active = project.Active,
                            Description = project.Description,
                            Technologies = project.Technologies,
                            Links = new List<ProjectLinkDto>(),
                            Image = project.Image,
                            Video = project.Video,
                            Order = project.Order
                        };
                        projectDict.Add(project.Id, projectDto);
                    }

                    if (link != null)
                    {
                        projectDto.Links.Add(new ProjectLinkDto
                        {
                            Id = link.Id,
                            Type = link.Type,
                            Href = link.Href,
                            Icon = link.Icon,
                            Order = link.Order
                        });
                    }

                    return projectDto;
                },
                splitOn: "Id"
            );

            return projectDict.Values;
        }

        public async Task<ProjectDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT p.*, pl.*
                FROM Projects p
                LEFT JOIN ProjectLinks pl ON p.Id = pl.ProjectId
                WHERE p.Id = @Id";

            var projectDict = new Dictionary<string, ProjectDto>();

            await connection.QueryAsync<Project, ProjectLink, ProjectDto>(
                sql,
                (project, link) =>
                {
                    if (!projectDict.TryGetValue(project.Id, out var projectDto))
                    {
                        projectDto = new ProjectDto
                        {
                            Id = project.Id,
                            Title = project.Title,
                            Href = project.Href,
                            Dates = project.Dates,
                            Active = project.Active,
                            Description = project.Description,
                            Technologies = project.Technologies,
                            Links = new List<ProjectLinkDto>(),
                            Image = project.Image,
                            Video = project.Video,
                            Order = project.Order
                        };
                        projectDict.Add(project.Id, projectDto);
                    }

                    if (link != null)
                    {
                        projectDto.Links.Add(new ProjectLinkDto
                        {
                            Id = link.Id,
                            Type = link.Type,
                            Href = link.Href,
                            Icon = link.Icon,
                            Order = link.Order
                        });
                    }

                    return projectDto;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return projectDict.Values.FirstOrDefault();
        }

        public async Task<ProjectDto> CreateAsync(Project project)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string projectSql = @"
                    INSERT INTO Projects (Id, Title, Href, Dates, Active, Description, Technologies, Image, Video, [Order])
                    VALUES (@Id, @Title, @Href, @Dates, @Active, @Description, @Technologies, @Image, @Video, @Order);";

                var technologiesJson = JsonSerializer.Serialize(project.Technologies);
                await connection.ExecuteAsync(projectSql, new
                {
                    project.Id,
                    project.Title,
                    project.Href,
                    project.Dates,
                    project.Active,
                    project.Description,
                    Technologies = technologiesJson,
                    project.Image,
                    project.Video,
                    project.Order
                }, transaction);

                if (project.Links != null && project.Links.Any())
                {
                    const string linkSql = @"
                        INSERT INTO ProjectLinks (Id, Type, Href, Icon, ProjectId, [Order])
                        VALUES (@Id, @Type, @Href, @Icon, @ProjectId, @Order)";

                    foreach (var link in project.Links)
                    {
                        link.ProjectId = project.Id;
                        await connection.ExecuteAsync(linkSql, link, transaction);
                    }
                }

                transaction.Commit();
                return await GetByIdAsync(project.Id);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<ProjectDto> UpdateAsync(Project project)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string updateProjectSql = @"
                    UPDATE Projects 
                    SET Title = @Title,
                        Href = @Href,
                        Dates = @Dates,
                        Active = @Active,
                        Description = @Description,
                        Technologies = @Technologies,
                        Image = @Image,
                        Video = @Video,
                        [Order] = @Order
                    WHERE Id = @Id";

                var technologiesJson = JsonSerializer.Serialize(project.Technologies);
                await connection.ExecuteAsync(updateProjectSql, new
                {
                    project.Id,
                    project.Title,
                    project.Href,
                    project.Dates,
                    project.Active,
                    project.Description,
                    Technologies = technologiesJson,
                    project.Image,
                    project.Video,
                    project.Order
                }, transaction);

                const string deleteLinksSql = "DELETE FROM ProjectLinks WHERE ProjectId = @ProjectId";
                await connection.ExecuteAsync(deleteLinksSql, new { ProjectId = project.Id }, transaction);

                if (project.Links != null && project.Links.Any())
                {
                    const string linkSql = @"
                        INSERT INTO ProjectLinks (Id, Type, Href, Icon, ProjectId, [Order])
                        VALUES (@Id, @Type, @Href, @Icon, @ProjectId, @Order)";

                    foreach (var link in project.Links)
                    {
                        link.ProjectId = project.Id;
                        await connection.ExecuteAsync(linkSql, link, transaction);
                    }
                }

                transaction.Commit();
                return await GetByIdAsync(project.Id);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string deleteLinksSql = "DELETE FROM ProjectLinks WHERE ProjectId = @Id";
                await connection.ExecuteAsync(deleteLinksSql, new { Id = id }, transaction);

                const string deleteProjectSql = "DELETE FROM Projects WHERE Id = @Id";
                var result = await connection.ExecuteAsync(deleteProjectSql, new { Id = id }, transaction);

                transaction.Commit();
                return result > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}