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
            using (var connection = _context.CreateConnection())
            {
                const string sql = "SELECT * FROM Profiles";
                try
                {
                    var profiles = await connection.QueryAsync<ProfileDto>(sql);
                    return profiles;
                }
                catch (Exception ex)
                {
                    throw new Exception("Profil bilgileri getirilirken bir hata oluştu", ex);
                }
            }
        }

        public async Task<ProfileDto> GetByIdAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "SELECT * FROM Profiles WHERE Id = @Id";
                try
                {
                    var profile = await connection.QueryFirstOrDefaultAsync<ProfileDto>(sql, new { Id = id });
                    return profile;
                }
                catch (Exception ex)
                {
                    throw new Exception("Profil bilgileri getirilirken bir hata oluştu", ex);
                }
            }
        }

        public async Task<ProfileDto> CreateAsync(CreateProfileDto profile)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                INSERT INTO Profiles (Id, Name, Initials, Url, Location, LocationLink, Description, Summary, AvatarUrl, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES (NEWID(), @Name, @Initials, @Url, @Location, @LocationLink, @Description, @Summary, @AvatarUrl, GETDATE())";
                try
                {
                    var createdProfile = await connection.ExecuteScalarAsync<string>(sql, profile);
                    return await GetByIdAsync(createdProfile!);
                }
                catch (Exception ex)
                {
                    throw new Exception("Profil bilgileri oluşturulurken bir hata oluştu", ex);
                }
            }
        }

        public async Task<ProfileDto> UpdateAsync(UpdateProfileDto profile)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"
                UPDATE Profiles 
                SET Name = @Name,
                    Initials = @Initials,
                    Url = @Url,
                    Location = @Location,
                    LocationLink = @LocationLink,
                    Description = @Description,
                    Summary = @Summary,
                    AvatarUrl = @AvatarUrl,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id";

                try
                {
                    await connection.ExecuteAsync(sql, profile);
                    return await GetByIdAsync(profile.Id);
                }
                catch (Exception ex)
                {
                    throw new Exception("Profil bilgileri güncellenirken bir hata oluştu", ex);
                }
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = "DELETE FROM Profiles WHERE Id = @Id";
                try
                {
                    var result = await connection.ExecuteAsync(sql, new { Id = id });
                    return result > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Profil bilgileri silinirken bir hata oluştu", ex);
                }
            }
        }
    }
}