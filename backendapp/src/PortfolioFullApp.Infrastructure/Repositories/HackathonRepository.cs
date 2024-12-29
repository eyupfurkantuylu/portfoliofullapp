using Dapper;
using PortfolioFullApp.Core.DTOs.Hackathon;
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

        public async Task<IEnumerable<HackathonDto>> GetAllByProfileIdAsync(string profileId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT h.*, t.* 
                FROM Hackathons h
                LEFT JOIN HackathonTeamMembers t ON h.Id = t.HackathonId
                WHERE h.ProfileId = @ProfileId
                ORDER BY h.Date DESC";

            var hackathonDict = new Dictionary<string, HackathonDto>();

            await connection.QueryAsync<Hackathon, HackathonTeamMember, HackathonDto>(
                sql,
                (hackathon, teamMember) =>
                {
                    if (!hackathonDict.TryGetValue(hackathon.Id, out var hackathonDto))
                    {
                        hackathonDto = new HackathonDto
                        {
                            Id = hackathon.Id,
                            Name = hackathon.Name,
                            Description = hackathon.Description,
                            Date = hackathon.Date,
                            Place = hackathon.Place,
                            Rank = hackathon.Rank,
                            ProjectName = hackathon.ProjectName,
                            ProjectDescription = hackathon.ProjectDescription,
                            GithubUrl = hackathon.GithubUrl,
                            TeamMembers = new List<HackathonTeamMemberDto>()
                        };
                        hackathonDict.Add(hackathon.Id, hackathonDto);
                    }

                    if (teamMember != null)
                    {
                        hackathonDto.TeamMembers.Add(new HackathonTeamMemberDto
                        {
                            Id = teamMember.Id,
                            Name = teamMember.Name,
                            Role = teamMember.Role
                        });
                    }

                    return hackathonDto;
                },
                new { ProfileId = profileId },
                splitOn: "Id"
            );

            return hackathonDict.Values;
        }

        public async Task<HackathonDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT h.*, t.* 
                FROM Hackathons h
                LEFT JOIN HackathonTeamMembers t ON h.Id = t.HackathonId
                WHERE h.Id = @Id";

            var hackathonDict = new Dictionary<string, HackathonDto>();

            await connection.QueryAsync<Hackathon, HackathonTeamMember, HackathonDto>(
                sql,
                (hackathon, teamMember) =>
                {
                    if (!hackathonDict.TryGetValue(hackathon.Id, out var hackathonDto))
                    {
                        hackathonDto = new HackathonDto
                        {
                            Id = hackathon.Id,
                            Name = hackathon.Name,
                            Description = hackathon.Description,
                            Date = hackathon.Date,
                            Place = hackathon.Place,
                            Rank = hackathon.Rank,
                            ProjectName = hackathon.ProjectName,
                            ProjectDescription = hackathon.ProjectDescription,
                            GithubUrl = hackathon.GithubUrl,
                            TeamMembers = new List<HackathonTeamMemberDto>()
                        };
                        hackathonDict.Add(hackathon.Id, hackathonDto);
                    }

                    if (teamMember != null)
                    {
                        hackathonDto.TeamMembers.Add(new HackathonTeamMemberDto
                        {
                            Id = teamMember.Id,
                            Name = teamMember.Name,
                            Role = teamMember.Role
                        });
                    }

                    return hackathonDto;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return hackathonDict.Values.FirstOrDefault();
        }

        public async Task<HackathonDto> CreateAsync(Hackathon hackathon)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string hackathonSql = @"
                    INSERT INTO Hackathons (
                        Id, Name, Description, Date, Place, Rank, 
                        ProjectName, ProjectDescription, GithubUrl, ProfileId
                    ) VALUES (
                        @Id, @Name, @Description, @Date, @Place, @Rank,
                        @ProjectName, @ProjectDescription, @GithubUrl, @ProfileId
                    )";

                await connection.ExecuteAsync(hackathonSql, hackathon, transaction);

                if (hackathon.TeamMembers != null && hackathon.TeamMembers.Any())
                {
                    const string teamMemberSql = @"
                        INSERT INTO HackathonTeamMembers (Id, Name, Role, HackathonId)
                        VALUES (@Id, @Name, @Role, @HackathonId)";

                    foreach (var member in hackathon.TeamMembers)
                    {
                        member.HackathonId = hackathon.Id;
                        await connection.ExecuteAsync(teamMemberSql, member, transaction);
                    }
                }

                transaction.Commit();
                return await GetByIdAsync(hackathon.Id);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<HackathonDto> UpdateAsync(Hackathon hackathon)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string updateHackathonSql = @"
                    UPDATE Hackathons 
                    SET Name = @Name,
                        Description = @Description,
                        Date = @Date,
                        Place = @Place,
                        Rank = @Rank,
                        ProjectName = @ProjectName,
                        ProjectDescription = @ProjectDescription,
                        GithubUrl = @GithubUrl
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateHackathonSql, hackathon, transaction);

                const string deleteTeamMembersSql = "DELETE FROM HackathonTeamMembers WHERE HackathonId = @HackathonId";
                await connection.ExecuteAsync(deleteTeamMembersSql, new { HackathonId = hackathon.Id }, transaction);

                if (hackathon.TeamMembers != null && hackathon.TeamMembers.Any())
                {
                    const string teamMemberSql = @"
                        INSERT INTO HackathonTeamMembers (Id, Name, Role, HackathonId)
                        VALUES (@Id, @Name, @Role, @HackathonId)";

                    foreach (var member in hackathon.TeamMembers)
                    {
                        member.HackathonId = hackathon.Id;
                        await connection.ExecuteAsync(teamMemberSql, member, transaction);
                    }
                }

                transaction.Commit();
                return await GetByIdAsync(hackathon.Id);
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
                const string deleteTeamMembersSql = "DELETE FROM HackathonTeamMembers WHERE HackathonId = @Id";
                await connection.ExecuteAsync(deleteTeamMembersSql, new { Id = id }, transaction);

                const string deleteHackathonSql = "DELETE FROM Hackathons WHERE Id = @Id";
                var result = await connection.ExecuteAsync(deleteHackathonSql, new { Id = id }, transaction);

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