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
            using (var connection = _context.CreateConnection())
            {
                var query = "SELECT * FROM Contacts";
                var contacts = (await connection.QueryAsync<ContactDto>(query)).ToList();

                return contacts;
            }
        }

        public async Task<ContactDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * 
                FROM Contacts 
                WHERE Id = @Id";

            var contact = await connection.QueryFirstOrDefaultAsync<ContactDto>(sql, new { Id = id });

            return contact ?? throw new KeyNotFoundException($"Contact with ID {id} waas not found.");
        }


        public async Task<ContactDto> CreateAsync(CreateContactDto createContactDto)
        {
            var contactSql = @"
                    INSERT INTO Contacts (Id, Email, Tel, CreatedAt)
                    OUTPUT INSERTED.Id
                    VALUES (NEWID(), @Email, @Tel, GETDATE())";
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

        public async Task<ContactDto> UpdateAsync(UpdateContactDto contact)
        {
            try
            {
                const string updateContactSql = @"
                    UPDATE Contacts 
                    SET Email = @Email, 
                        Tel = @Tel,
                        UpdatedAt = GETDATE() 
                    WHERE Id = @Id";

                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(updateContactSql, contact);
                }

                return await GetByIdAsync(contact.Id);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    const string deleteContactSql = "DELETE FROM Contacts WHERE Id = @Id";
                    var result = await connection.ExecuteAsync(deleteContactSql, new { Id = id });
                    return result > 0;
                }

            }
            catch
            {
                throw;
            }
        }
    }
}