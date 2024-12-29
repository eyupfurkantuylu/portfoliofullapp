using Dapper;
using PortfolioFullApp.Core.DTOs.Profile;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly DapperContext _context;

        public ProfileRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProfileDto>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM Profiles";

            var profiles = await connection.QueryAsync<ProfileDto>(sql);
            return profiles;
        }

        public async Task<ProfileDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM Profiles WHERE Id = @Id";

            var profile = await connection.QueryFirstOrDefaultAsync<ProfileDto>(sql, new { Id = id });
            return profile;
        }

        public async Task<ProfileDto> CreateAsync(Profile profile)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                INSERT INTO Profiles (Id, Name, Initials, Url, Location, LocationLink, Description, Summary, AvatarUrl)
                VALUES (@Id, @Name, @Initials, @Url, @Location, @LocationLink, @Description, @Summary, @AvatarUrl);
                SELECT * FROM Profiles WHERE Id = @Id";

            var createdProfile = await connection.QueryFirstOrDefaultAsync<ProfileDto>(sql, profile);
            return createdProfile;
        }

        public async Task<ProfileDto> UpdateAsync(Profile profile)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE Profiles 
                SET Name = @Name,
                    Initials = @Initials,
                    Url = @Url,
                    Location = @Location,
                    LocationLink = @LocationLink,
                    Description = @Description,
                    Summary = @Summary,
                    AvatarUrl = @AvatarUrl
                WHERE Id = @Id;
                SELECT * FROM Profiles WHERE Id = @Id";

            var updatedProfile = await connection.QueryFirstOrDefaultAsync<ProfileDto>(sql, profile);
            return updatedProfile;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM Profiles WHERE Id = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
    }
}