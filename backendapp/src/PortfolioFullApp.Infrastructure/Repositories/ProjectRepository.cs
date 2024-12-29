using Dapper;
using PortfolioFullApp.Core.DTOs.Project;
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
                SELECT p.*, t.*, i.*
                FROM Projects p
                LEFT JOIN ProjectTechnologies t ON p.Id = t.ProjectId
                LEFT JOIN ProjectImages i ON p.Id = i.ProjectId
                WHERE p.ProfileId = @ProfileId
                ORDER BY p.[Order]";

            var projectDict = new Dictionary<string, ProjectDto>();

            await connection.QueryAsync<Project, ProjectTechnology, ProjectImage, ProjectDto>(
                sql,
                (project, technology, image) =>
                {
                    if (!projectDict.TryGetValue(project.Id, out var projectDto))
                    {
                        projectDto = new ProjectDto
                        {
                            Id = project.Id,
                            Name = project.Name,
                            Description = project.Description,
                            GithubUrl = project.GithubUrl,
                            LiveUrl = project.LiveUrl,
                            Order = project.Order,
                            IsActive = project.IsActive,
                            Technologies = new List<ProjectTechnologyDto>(),
                            Images = new List<ProjectImageDto>()
                        };
                        projectDict.Add(project.Id, projectDto);
                    }

                    if (technology != null)
                    {
                        var techDto = new ProjectTechnologyDto
                        {
                            Id = technology.Id,
                            Name = technology.Name,
                            Version = technology.Version
                        };
                        if (!projectDto.Technologies.Any(t => t.Id == techDto.Id))
                        {
                            projectDto.Technologies.Add(techDto);
                        }
                    }

                    if (image != null)
                    {
                        var imageDto = new ProjectImageDto
                        {
                            Id = image.Id,
                            Url = image.Url,
                            IsMain = image.IsMain
                        };
                        if (!projectDto.Images.Any(i => i.Id == imageDto.Id))
                        {
                            projectDto.Images.Add(imageDto);
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
                SELECT p.*, t.*, i.*
                FROM Projects p
                LEFT JOIN ProjectTechnologies t ON p.Id = t.ProjectId
                LEFT JOIN ProjectImages i ON p.Id = i.ProjectId
                WHERE p.Id = @Id";

            var projectDict = new Dictionary<string, ProjectDto>();

            await connection.QueryAsync<Project, ProjectTechnology, ProjectImage, ProjectDto>(
                sql,
                (project, technology, image) =>
                {
                    if (!projectDict.TryGetValue(project.Id, out var projectDto))
                    {
                        projectDto = new ProjectDto
                        {
                            Id = project.Id,
                            Name = project.Name,
                            Description = project.Description,
                            GithubUrl = project.GithubUrl,
                            LiveUrl = project.LiveUrl,
                            Order = project.Order,
                            IsActive = project.IsActive,
                            Technologies = new List<ProjectTechnologyDto>(),
                            Images = new List<ProjectImageDto>()
                        };
                        projectDict.Add(project.Id, projectDto);
                    }

                    if (technology != null)
                    {
                        var techDto = new ProjectTechnologyDto
                        {
                            Id = technology.Id,
                            Name = technology.Name,
                            Version = technology.Version
                        };
                        if (!projectDto.Technologies.Any(t => t.Id == techDto.Id))
                        {
                            projectDto.Technologies.Add(techDto);
                        }
                    }

                    if (image != null)
                    {
                        var imageDto = new ProjectImageDto
                        {
                            Id = image.Id,
                            Url = image.Url,
                            IsMain = image.IsMain
                        };
                        if (!projectDto.Images.Any(i => i.Id == imageDto.Id))
                        {
                            projectDto.Images.Add(imageDto);
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
                var maxOrder = await GetMaxOrderAsync(project.ProfileId);
                project.Order = maxOrder + 1;

                const string projectSql = @"
                    INSERT INTO Projects (
                        Id, Name, Description, GithubUrl, LiveUrl,
                        [Order], IsActive, ProfileId
                    ) VALUES (
                        @Id, @Name, @Description, @GithubUrl, @LiveUrl,
                        @Order, @IsActive, @ProfileId
                    )";

                await connection.ExecuteAsync(projectSql, project, transaction);

                if (project.Technologies?.Any() == true)
                {
                    const string techSql = @"
                        INSERT INTO ProjectTechnologies (Id, Name, Version, ProjectId)
                        VALUES (@Id, @Name, @Version, @ProjectId)";

                    foreach (var tech in project.Technologies)
                    {
                        tech.ProjectId = project.Id;
                        await connection.ExecuteAsync(techSql, tech, transaction);
                    }
                }

                if (project.Images?.Any() == true)
                {
                    const string imageSql = @"
                        INSERT INTO ProjectImages (Id, Url, IsMain, ProjectId)
                        VALUES (@Id, @Url, @IsMain, @ProjectId)";

                    foreach (var image in project.Images)
                    {
                        image.ProjectId = project.Id;
                        await connection.ExecuteAsync(imageSql, image, transaction);
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
                    SET Name = @Name,
                        Description = @Description,
                        GithubUrl = @GithubUrl,
                        LiveUrl = @LiveUrl,
                        IsActive = @IsActive
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateProjectSql, project, transaction);

                // Teknolojileri güncelle
                const string deleteTechSql = "DELETE FROM ProjectTechnologies WHERE ProjectId = @ProjectId";
                await connection.ExecuteAsync(deleteTechSql, new { ProjectId = project.Id }, transaction);

                if (project.Technologies?.Any() == true)
                {
                    const string techSql = @"
                        INSERT INTO ProjectTechnologies (Id, Name, Version, ProjectId)
                        VALUES (@Id, @Name, @Version, @ProjectId)";

                    foreach (var tech in project.Technologies)
                    {
                        tech.ProjectId = project.Id;
                        await connection.ExecuteAsync(techSql, tech, transaction);
                    }
                }

                // Resimleri güncelle
                const string deleteImagesSql = "DELETE FROM ProjectImages WHERE ProjectId = @ProjectId";
                await connection.ExecuteAsync(deleteImagesSql, new { ProjectId = project.Id }, transaction);

                if (project.Images?.Any() == true)
                {
                    const string imageSql = @"
                        INSERT INTO ProjectImages (Id, Url, IsMain, ProjectId)
                        VALUES (@Id, @Url, @IsMain, @ProjectId)";

                    foreach (var image in project.Images)
                    {
                        image.ProjectId = project.Id;
                        await connection.ExecuteAsync(imageSql, image, transaction);
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

                // İlişkili kayıtları sil
                const string deleteTechSql = "DELETE FROM ProjectTechnologies WHERE ProjectId = @Id";
                await connection.ExecuteAsync(deleteTechSql, new { Id = id }, transaction);

                const string deleteImagesSql = "DELETE FROM ProjectImages WHERE ProjectId = @Id";
                await connection.ExecuteAsync(deleteImagesSql, new { Id = id }, transaction);

                // Projeyi sil
                const string deleteProjectSql = "DELETE FROM Projects WHERE Id = @Id";
                await connection.ExecuteAsync(deleteProjectSql, new { Id = id }, transaction);

                // Sıralamayı güncelle
                const string updateOrdersSql = @"
                    UPDATE Projects 
                    SET [Order] = [Order] - 1 
                    WHERE ProfileId = @ProfileId 
                    AND [Order] > @Order";

                await connection.ExecuteAsync(updateOrdersSql,
                    new { ProfileId = project.ProfileId, Order = project.Order },
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
                        WHERE ProfileId = @ProfileId 
                        AND [Order] > @CurrentOrder 
                        AND [Order] <= @NewOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { ProfileId = project.ProfileId, CurrentOrder = project.Order, NewOrder = newOrder },
                        transaction);
                }
                else if (newOrder < project.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE Projects 
                        SET [Order] = [Order] + 1 
                        WHERE ProfileId = @ProfileId 
                        AND [Order] >= @NewOrder 
                        AND [Order] < @CurrentOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { ProfileId = project.ProfileId, CurrentOrder = project.Order, NewOrder = newOrder },
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

        public async Task<int> GetMaxOrderAsync(string profileId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT ISNULL(MAX([Order]), 0) 
                FROM Projects 
                WHERE ProfileId = @ProfileId";

            return await connection.ExecuteScalarAsync<int>(sql, new { ProfileId = profileId });
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