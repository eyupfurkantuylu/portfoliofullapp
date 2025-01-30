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
                        var emptyList = new List<string>();
                        var technologies = project.Technologies != null
                            ? JsonSerializer.Deserialize<List<string>>(project.Technologies.ToString())
                            : new List<string>();

                        projectDto = new ProjectDto
                        {
                            Id = project.Id,
                            Title = project.Title,
                            Href = project.Href,
                            Dates = project.Dates,
                            Active = project.Active,
                            Description = project.Description,
                            Technologies = technologies,
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
                        var technologies = project.Technologies != null
                          ? JsonSerializer.Deserialize<List<string>>(project.Technologies.ToString())
                          : new List<string>();

                        projectDto = new ProjectDto
                        {
                            Id = project.Id,
                            Title = project.Title,
                            Href = project.Href,
                            Dates = project.Dates,
                            Active = project.Active,
                            Description = project.Description,
                            Technologies = technologies,
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

            return projectDict.Values.FirstOrDefault() ?? throw new Exception("Proje bulunamadı");
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto project)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var technologiesJson = JsonSerializer.Serialize(project.Technologies.ToArray());

                    const string projectSql = @"
                    INSERT INTO Projects (Id, Title, Href, Dates, Active, Description, Technologies, Image, Video, [Order], CreatedAt)
                    OUTPUT INSERTED.Id 
                    VALUES (NEWID(), @Title, @Href, @Dates, @Active, @Description, @TechnologiesJson, @Image, @Video, @Order, GETDATE())";

                    var parameters = new
                    {
                        project.Title,
                        project.Href,
                        project.Dates,
                        project.Active,
                        project.Description,
                        TechnologiesJson = technologiesJson,
                        project.Image,
                        project.Video,
                        project.Order
                    };

                    var createdProject = await connection.ExecuteScalarAsync<string>(projectSql, parameters);
                    return await GetByIdAsync(createdProject!);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Proje oluşturulurken bir hata oluştu: {ex.Message}", ex);
                }
            }
        }

        public async Task<ProjectDto> UpdateAsync(UpdateProjectDto project)
        {
            using (var connection = _context.CreateConnection())
            {
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
                        [Order] = @Order,
                        UpdatedAt = GETDATE()
                    WHERE Id = @Id";

                    await connection.ExecuteAsync(updateProjectSql, project);
                    return await GetByIdAsync(project.Id);
                }
                catch (Exception ex)
                {
                    throw new Exception("Proje güncellenirken bir hata oluştu", ex);
                }
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    const string deleteLinksSql = "DELETE FROM ProjectLinks WHERE ProjectId = @Id";

                    await connection.ExecuteAsync(deleteLinksSql, new { Id = id });

                    const string deleteProjectSql = "DELETE FROM Projects WHERE Id = @Id";
                    var result = await connection.ExecuteAsync(deleteProjectSql, new { Id = id });
                    return result > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Proje silinirken bir hata oluştu", ex);
                }
            }
        }
    }
}