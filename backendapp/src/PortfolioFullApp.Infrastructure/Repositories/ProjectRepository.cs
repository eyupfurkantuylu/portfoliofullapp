using Dapper;
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

        public async Task<IEnumerable<ProjectDto>> GetAllByProfileIdAsync(string profileId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT p.*, l.*
                FROM Projects p
                LEFT JOIN ProjectLinks l ON p.Id = l.ProjectId
                WHERE p.ProfileId = @ProfileId
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
                        var linkDto = new ProjectLinkDto
                        {
                            Id = link.Id,
                            Type = link.Type,
                            Href = link.Href,
                            Icon = link.Icon,
                            Order = link.Order
                        };
                        if (!projectDto.Links.Any(l => l.Id == linkDto.Id))
                        {
                            projectDto.Links.Add(linkDto);
                        }
                    }

                    return projectDto;
                },
                new { ProfileId = profileId },
                splitOn: "Id"
            );

            return projectDict.Values;
        }

        public async Task<ProjectDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT p.*, l.*
                FROM Projects p
                LEFT JOIN ProjectLinks l ON p.Id = l.ProjectId
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
                        var linkDto = new ProjectLinkDto
                        {
                            Id = link.Id,
                            Type = link.Type,
                            Href = link.Href,
                            Icon = link.Icon,
                            Order = link.Order
                        };
                        if (!projectDto.Links.Any(l => l.Id == linkDto.Id))
                        {
                            projectDto.Links.Add(linkDto);
                        }
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
                // Önce maksimum sıra numarasını al
                var maxOrder = await connection.ExecuteScalarAsync<int>(
                    "SELECT ISNULL(MAX([Order]), 0) FROM Projects",
                    transaction: transaction
                );
                project.Order = maxOrder + 1;

                const string projectSql = @"
                    INSERT INTO Projects (
                        Id, Title, Href, Dates, Active,
                        Description, Image, Video, [Order], Technologies
                    ) VALUES (
                        @Id, @Title, @Href, @Dates, @Active,
                        @Description, @Image, @Video, @Order, @Technologies
                    )";

                await connection.ExecuteAsync(projectSql, project, transaction);

                if (project.Links?.Any() == true)
                {
                    const string linkSql = @"
                        INSERT INTO ProjectLinks (Id, Type, Href, Icon, [Order], ProjectId)
                        VALUES (@Id, @Type, @Href, @Icon, @Order, @ProjectId)";

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
                        Image = @Image,
                        Video = @Video,
                        Technologies = @Technologies
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateProjectSql, project, transaction);

                // Linkleri güncelle
                const string deleteLinksSql = "DELETE FROM ProjectLinks WHERE ProjectId = @ProjectId";
                await connection.ExecuteAsync(deleteLinksSql, new { ProjectId = project.Id }, transaction);

                if (project.Links?.Any() == true)
                {
                    const string linkSql = @"
                        INSERT INTO ProjectLinks (Id, Type, Href, Icon, [Order], ProjectId)
                        VALUES (@Id, @Type, @Href, @Icon, @Order, @ProjectId)";

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
                // Önce silinecek projenin bilgilerini al
                var project = await GetByIdAsync(id);
                if (project == null) return false;

                // İlişkili linkleri sil
                const string deleteLinksSql = "DELETE FROM ProjectLinks WHERE ProjectId = @Id";
                await connection.ExecuteAsync(deleteLinksSql, new { Id = id }, transaction);

                // Projeyi sil
                const string deleteProjectSql = "DELETE FROM Projects WHERE Id = @Id";
                await connection.ExecuteAsync(deleteProjectSql, new { Id = id }, transaction);

                // Sıralamayı güncelle
                const string updateOrdersSql = @"
                    UPDATE Projects 
                    SET [Order] = [Order] - 1 
                    WHERE [Order] > @Order";

                await connection.ExecuteAsync(updateOrdersSql,
                    new { Order = project.Order },
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
                var project = await GetByIdAsync(id);
                if (project == null) return false;

                if (newOrder > project.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE Projects 
                        SET [Order] = [Order] - 1 
                        WHERE [Order] > @CurrentOrder 
                        AND [Order] <= @NewOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { CurrentOrder = project.Order, NewOrder = newOrder },
                        transaction);
                }
                else if (newOrder < project.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE Projects 
                        SET [Order] = [Order] + 1 
                        WHERE [Order] >= @NewOrder 
                        AND [Order] < @CurrentOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { CurrentOrder = project.Order, NewOrder = newOrder },
                        transaction);
                }

                const string updateProjectSql = @"
                    UPDATE Projects 
                    SET [Order] = @NewOrder 
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateProjectSql,
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


        public async Task<bool> UpdateStatusAsync(string id, bool isActive)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE Projects 
                SET IsActive = @IsActive
                WHERE Id = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id, IsActive = isActive });
            return result > 0;
        }
    }
}