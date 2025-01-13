using Dapper;
using PortfolioFullApp.Core.DTOs.Hackathon;
using PortfolioFullApp.Core.DTOs.HackathonLink;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class HackathonRepository : IHackathonRepository
    {
        private readonly DapperContext _context;

        public HackathonRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HackathonDto>> GetAllAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                const string query = @"
                    SELECT h.*, hl.*
                    FROM Hackathons h
                    LEFT JOIN HackathonLinks hl ON h.Id = hl.HackathonId
                    ORDER BY h.[Order]";

                var hackathonDict = new Dictionary<string, HackathonDto>();

                await connection.QueryAsync<Hackathon, HackathonLink, HackathonDto>(
                    query,
                    (hackathon, link) =>
                    {
                        if (!hackathonDict.TryGetValue(hackathon.Id, out var hackathonDto))
                        {
                            hackathonDto = new HackathonDto
                            {
                                Id = hackathon.Id,
                                Title = hackathon.Title,
                                Dates = hackathon.Dates,
                                Location = hackathon.Location,
                                Description = hackathon.Description,
                                Image = hackathon.Image,
                                Mlh = hackathon.Mlh,
                                Win = hackathon.Win,
                                Order = hackathon.Order,
                                Links = new List<HackathonLinkDto>()
                            };
                            hackathonDict.Add(hackathon.Id, hackathonDto);
                        }

                        if (link != null)
                        {
                            hackathonDto.Links.Add(new HackathonLinkDto
                            {
                                Id = link.Id,
                                Title = link.Title,
                                Icon = link.Icon,
                                Href = link.Href,
                                Order = link.Order
                            });
                        }

                        return hackathonDto;
                    },
                    splitOn: "Id"
                );

                return hackathonDict.Values;
            }
        }
        public async Task<HackathonDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT h.*, hl.* 
                FROM Hackathons h
                LEFT JOIN HackathonLinks hl ON h.Id = hl.HackathonId
                WHERE h.Id = @Id";

            var hackathonDict = new Dictionary<string, HackathonDto>();

            await connection.QueryAsync<Hackathon, HackathonLink, HackathonDto>(
                sql,
                (hackathon, link) =>
                {
                    if (!hackathonDict.TryGetValue(hackathon.Id, out var hackathonDto))
                    {
                        hackathonDto = new HackathonDto
                        {
                            Id = hackathon.Id,
                            Title = hackathon.Title,
                            Dates = hackathon.Dates,
                            Location = hackathon.Location,
                            Description = hackathon.Description,
                            Image = hackathon.Image,
                            Mlh = hackathon.Mlh,
                            Win = hackathon.Win,
                            Order = hackathon.Order,
                            Links = new List<HackathonLinkDto>()
                        };
                        hackathonDict.Add(hackathon.Id, hackathonDto);
                    }

                    if (link != null)
                    {
                        hackathonDto.Links.Add(new HackathonLinkDto
                        {
                            Id = link.Id,
                            Title = link.Title,
                            Icon = link.Icon,
                            Href = link.Href,
                            Order = link.Order
                        });
                    }

                    return hackathonDto;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return hackathonDict.Values.FirstOrDefault();
        }

        public async Task<HackathonDto> CreateAsync(CreateHackathonDto hackathon)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var maxOrder = await GetMaxOrderAsync();
                    hackathon.Order = maxOrder + 1;

                    const string hackathonSql = @"
                    INSERT INTO Hackathons (
                        Id, Title, Dates, Location, Description,
                        Image, Mlh, Win, [Order], CreatedAt
                    ) 
                    OUTPUT INSERTED.Id
                    VALUES (
                        NEWID(), @Title, @Dates, @Location, @Description,
                        @Image, @Mlh, @Win, @Order, GETDATE()
                    )";

                    var hackathonId = await connection.ExecuteScalarAsync<string>(hackathonSql, new
                    {
                        hackathon.Title,
                        hackathon.Dates,
                        hackathon.Location,
                        hackathon.Description,
                        hackathon.Image,
                        hackathon.Mlh,
                        hackathon.Win,
                        hackathon.Order
                    });
                    return await GetByIdAsync(hackathonId!);
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<HackathonDto> UpdateAsync(UpdateHackathonDto hackathon)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    const string updateHackathonSql = @"
                    UPDATE Hackathons 
                    SET Title = @Title,
                        Dates = @Dates,
                        Location = @Location,
                        Description = @Description,
                        Image = @Image,
                        Mlh = @Mlh,
                        Win = @Win,
                        [Order] = @Order,
                        UpdatedAt = GETDATE()
                    WHERE Id = @Id";

                    await connection.ExecuteAsync(updateHackathonSql, hackathon);

                    return await GetByIdAsync(hackathon.Id);
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                    try
                    {
                        var hackathon = await GetByIdAsync(id);
                        if (hackathon == null) return false;

                        const string deleteLinksSql = "DELETE FROM HackathonLinks WHERE HackathonId = @Id";
                        await connection.ExecuteAsync(deleteLinksSql, new { Id = id });

                        const string deleteHackathonSql = "DELETE FROM Hackathons WHERE Id = @Id";
                        await connection.ExecuteAsync(deleteHackathonSql, new { Id = id });

                        const string updateOrdersSql = @"
                    UPDATE Hackathons 
                    SET [Order] = [Order] - 1 
                    WHERE [Order] > @Order";

                        await connection.ExecuteAsync(updateOrdersSql,
                            new { Order = hackathon.Order }
                            );

                        return true;
                    }
                    catch
                    {
                        throw;
                    }
                
            }
        }

        public async Task<bool> UpdateOrderAsync(string id, int newOrder)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var hackathon = await GetByIdAsync(id);
                if (hackathon == null) return false;

                if (newOrder > hackathon.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE Hackathons 
                        SET [Order] = [Order] - 1 
                        WHERE [Order] > @CurrentOrder 
                        AND [Order] <= @NewOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { CurrentOrder = hackathon.Order, NewOrder = newOrder },
                        transaction);
                }
                else if (newOrder < hackathon.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE Hackathons 
                        SET [Order] = [Order] + 1 
                        WHERE [Order] >= @NewOrder 
                        AND [Order] < @CurrentOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new { CurrentOrder = hackathon.Order, NewOrder = newOrder },
                        transaction);
                }

                const string updateHackathonSql = @"
                    UPDATE Hackathons 
                    SET [Order] = @NewOrder 
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateHackathonSql,
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

        public async Task<int> GetMaxOrderAsync()
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT ISNULL(MAX([Order]), 0) FROM Hackathons";
            return await connection.ExecuteScalarAsync<int>(sql);
        }
    }
}