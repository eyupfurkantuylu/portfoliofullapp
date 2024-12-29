// using Dapper;
// using PortfolioFullApp.Core.DTOs.Contact;
// using PortfolioFullApp.Core.DTOs.NavbarItem;
// using PortfolioFullApp.Core.DTOs.Profile;
// using PortfolioFullApp.Core.DTOs.Skill;
// using PortfolioFullApp.Core.DTOs.SocialMedia;
// using PortfolioFullApp.Core.Entities;
// using PortfolioFullApp.Core.Interfaces;
// using PortfolioFullApp.Infrastructure.Data;
//
// namespace PortfolioFullApp.Infrastructure.Repositories
// {
//     public class ProfileRepository : IProfileRepository
//     {
//         private readonly DapperContext _context;
//
//         public ProfileRepository(DapperContext context)
//         {
//             _context = context;
//         }
//
//         public async Task<ProfileDto> GetByIdAsync(string id)
//         {
//             using var connection = _context.CreateConnection();
//             const string sql = @"
//                 SELECT p.*, c.*, sm.*
//                 FROM Profiles p
//                 LEFT JOIN Contacts c ON p.Id = c.ProfileId
//                 LEFT JOIN SocialMedia sm ON c.Id = sm.ContactId
//                 WHERE p.Id = @Id";
//
//             var profileDict = new Dictionary<string, ProfileDto>();
//
//             await connection.QueryAsync<Profile, Contact, SocialMedia, ProfileDto>(
//                 sql,
//                 (profile, contact, social) =>
//                 {
//                     if (!profileDict.TryGetValue(profile.Id, out var profileDto))
//                     {
//                         profileDto = new ProfileDto
//                         {
//                             Id = profile.Id,
//                             Name = profile.Name,
//                             Initials = profile.Initials,
//                             Description = profile.Description,
//                             Summary = profile.Summary,
//                             AvatarUrl = profile.AvatarUrl,
//                             Location = profile.Location,
//                             LocationLink = profile.LocationLink,
//                             Url = profile.Url,
//                             Contact = contact != null ? new ContactDto() : null,
//                             Skills = new List<SkillDto>(),
//                             Navbar = new List<NavbarItemDto>()
//                         };
//                         profileDict.Add(profile.Id, profileDto);
//                     }
//
//                     if (contact != null && profileDto.Contact != null)
//                     {
//                         profileDto.Contact.Id = contact.Id;
//                         profileDto.Contact.Email = contact.Email;
//                         profileDto.Contact.Tel = contact.Tel;
//                         profileDto.Contact.Social ??= new List<SocialMediaDto>();
//
//                         if (social != null)
//                         {
//                             profileDto.Contact.Social.Add(new SocialMediaDto
//                             {
//                                 Id = social.Id,
//                                 Name = social.Name,
//                                 Url = social.Url
//                             });
//                         }
//                     }
//
//                     return profileDto;
//                 },
//                 new { Id = id },
//                 splitOn: "Id"
//             );
//
//             return profileDict.Values.FirstOrDefault();
//         }
//
//         public async Task<ProfileDto> GetByUserIdAsync(string userId)
//         {
//             using var connection = _context.CreateConnection();
//             const string sql = @"
//                 SELECT p.*, c.*, sm.*
//                 FROM Profiles p
//                 LEFT JOIN Contacts c ON p.Id = c.ProfileId
//                 LEFT JOIN SocialMedia sm ON c.Id = sm.ContactId
//                 WHERE p.UserId = @UserId";
//
//             var profileDict = new Dictionary<string, ProfileDto>();
//
//             await connection.QueryAsync<Profile, Contact, SocialMedia, ProfileDto>(
//                 sql,
//                 (profile, contact, social) =>
//                 {
//                     if (!profileDict.TryGetValue(profile.Id, out var profileDto))
//                     {
//                         profileDto = new ProfileDto
//                         {
//                             Id = profile.Id,
//                             Name = profile.Name,
//                             Initials = profile.Initials,
//                             Description = profile.Description,
//                             Summary = profile.Summary,
//                             AvatarUrl = profile.AvatarUrl,
//                             Location = profile.Location,
//                             LocationLink = profile.LocationLink,
//                             Url = profile.Url,
//                             Contact = contact != null ? new ContactDto() : null,
//                             Skills = new List<SkillDto>(),
//                             Navbar = new List<NavbarItemDto>()
//                         };
//                         profileDict.Add(profile.Id, profileDto);
//                     }
//
//                     if (contact != null && profileDto.Contact != null)
//                     {
//                         profileDto.Contact.Id = contact.Id;
//                         profileDto.Contact.Email = contact.Email;
//                         profileDto.Contact.Tel = contact.Tel;
//                         profileDto.Contact.Social ??= new List<SocialMediaDto>();
//
//                         if (social != null)
//                         {
//                             profileDto.Contact.Social.Add(new SocialMediaDto
//                             {
//                                 Id = social.Id,
//                                 Name = social.Name,
//                                 Url = social.Url
//                             });
//                         }
//                     }
//
//                     return profileDto;
//                 },
//                 new { UserId = userId },
//                 splitOn: "Id"
//             );
//
//             return profileDict.Values.FirstOrDefault();
//         }
//
//         public async Task<ProfileDto> CreateAsync(Profile profile)
//         {
//             using var connection = _context.CreateConnection();
//             const string sql = @"
//                 INSERT INTO Profiles (
//                     Id, Name, Initials, Description, 
//                     Summary, AvatarUrl, UserId,
//                     Location, LocationLink, Url
//                 ) VALUES (
//                     @Id, @Name, @Initials, @Description, 
//                     @Summary, @AvatarUrl, @UserId,
//                     @Location, @LocationLink, @Url
//                 );
//                 SELECT * FROM Profiles WHERE Id = @Id";
//
//             var createdProfile = await connection.QueryFirstOrDefaultAsync<ProfileDto>(sql, profile);
//             return createdProfile;
//         }
//
//         public async Task<ProfileDto> UpdateAsync(Profile profile)
//         {
//             using var connection = _context.CreateConnection();
//             const string sql = @"
//                 UPDATE Profiles 
//                 SET Name = @Name,
//                     Initials = @Initials,
//                     Description = @Description,
//                     Summary = @Summary,
//                     AvatarUrl = @AvatarUrl,
//                     Location = @Location,
//                     LocationLink = @LocationLink,
//                     Url = @Url
//                 WHERE Id = @Id;
//                 SELECT * FROM Profiles WHERE Id = @Id";
//
//             var updatedProfile = await connection.QueryFirstOrDefaultAsync<ProfileDto>(sql, profile);
//             return updatedProfile;
//         }
//
//         public async Task<bool> DeleteAsync(string id)
//         {
//             using var connection = _context.CreateConnection();
//             const string sql = "DELETE FROM Profiles WHERE Id = @Id";
//
//             var result = await connection.ExecuteAsync(sql, new { Id = id });
//             return result > 0;
//         }
//
//         public async Task<bool> ExistsAsync(string userId)
//         {
//             using var connection = _context.CreateConnection();
//             const string sql = "SELECT COUNT(1) FROM Profiles WHERE UserId = @UserId";
//
//             var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
//             return count > 0;
//         }
//
//         public async Task<bool> UpdateAboutAsync(string id, string about)
//         {
//             using var connection = _context.CreateConnection();
//             const string sql = @"
//                 UPDATE Profiles 
//                 SET About = @About
//                 WHERE Id = @Id";
//
//             var result = await connection.ExecuteAsync(sql, new { Id = id, About = about });
//             return result > 0;
//         }
//
//         public async Task<bool> UpdateSummaryAsync(string id, string summary)
//         {
//             using var connection = _context.CreateConnection();
//             const string sql = @"
//                 UPDATE Profiles 
//                 SET Summary = @Summary
//                 WHERE Id = @Id";
//
//             var result = await connection.ExecuteAsync(sql, new { Id = id, Summary = summary });
//             return result > 0;
//         }
//
//         private static string GetInitials(string firstName, string lastName)
//         {
//             return string.Empty;
//         }
//     }
// }