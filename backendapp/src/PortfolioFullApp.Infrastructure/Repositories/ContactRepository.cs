using Dapper;
using PortfolioFullApp.Core.DTOs.Contact;
using PortfolioFullApp.Core.DTOs.SocialMedia;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly DapperContext _context;

        public ContactRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContactDto>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT c.*, sm.* 
                FROM Contacts c
                LEFT JOIN SocialMedias sm ON c.Id = sm.ContactId";

            var contactDict = new Dictionary<string, ContactDto>();

            await connection.QueryAsync<Contact, SocialMedia, ContactDto>(
                sql,
                (contact, social) =>
                {
                    if (!contactDict.TryGetValue(contact.Id, out var contactDto))
                    {
                        contactDto = new ContactDto
                        {
                            Id = contact.Id,
                            Email = contact.Email,
                            Tel = contact.Tel,
                            Social = new List<SocialMediaDto>()
                        };
                        contactDict.Add(contact.Id, contactDto);
                    }

                    if (social != null)
                    {
                        contactDto.Social.Add(new SocialMediaDto
                        {
                            Id = social.Id,
                            Name = social.Name,
                            Url = social.Url
                        });
                    }

                    return contactDto;
                },
                splitOn: "Id"
            );

            return contactDict.Values;
        }

        public async Task<ContactDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT c.*, sm.* 
                FROM Contacts c
                LEFT JOIN SocialMedias sm ON c.Id = sm.ContactId
                WHERE c.Id = @Id";

            var contactDict = new Dictionary<string, ContactDto>();

            await connection.QueryAsync<Contact, SocialMedia, ContactDto>(
                sql,
                (contact, social) =>
                {
                    if (!contactDict.TryGetValue(contact.Id, out var contactDto))
                    {
                        contactDto = new ContactDto
                        {
                            Id = contact.Id,
                            Email = contact.Email,
                            Tel = contact.Tel,
                            Social = new List<SocialMediaDto>()
                        };
                        contactDict.Add(contact.Id, contactDto);
                    }

                    if (social != null)
                    {
                        contactDto.Social.Add(new SocialMediaDto
                        {
                            Id = social.Id,
                            Name = social.Name,
                            Url = social.Url
                        });
                    }

                    return contactDto;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return contactDict.Values.FirstOrDefault();
        }

        public async Task<ContactDto> CreateAsync(CreateContactDto createContactDto)
        {
            var contactSql = @"
                    INSERT INTO Contacts (Id, Email, Tel, CreatedAt)
                    VALUES (NEWID(), @Email, @Tel, GETDATE());
                    SELECT SCOPE_IDENTITY();";
            var parameters = new
            {
                Email = createContactDto.Email,
                Tel = createContactDto.Tel
            };
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var contactId = await connection.ExecuteScalarAsync<string>(contactSql, parameters);
                    return await GetByIdAsync(contactId!);
                }
                catch
                {
                    throw;
                }
            };
        }

        public async Task<ContactDto> UpdateAsync(Contact contact)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string updateContactSql = @"
                    UPDATE Contacts 
                    SET Email = @Email, 
                        Tel = @Tel
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateContactSql, contact, transaction);

                const string deleteSocialSql = "DELETE FROM SocialMedias WHERE ContactId = @ContactId";
                await connection.ExecuteAsync(deleteSocialSql, new { ContactId = contact.Id }, transaction);

           

                transaction.Commit();
                return await GetByIdAsync(contact.Id);
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
                const string deleteSocialSql = "DELETE FROM SocialMedias WHERE ContactId = @Id";
                await connection.ExecuteAsync(deleteSocialSql, new { Id = id }, transaction);

                const string deleteContactSql = "DELETE FROM Contacts WHERE Id = @Id";
                var result = await connection.ExecuteAsync(deleteContactSql, new { Id = id }, transaction);

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