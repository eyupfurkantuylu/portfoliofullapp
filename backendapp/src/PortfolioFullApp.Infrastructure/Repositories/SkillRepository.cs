using Dapper;
using PortfolioFullApp.Core.DTOs.Skill;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;
using PortfolioFullApp.Infrastructure.Data;

namespace PortfolioFullApp.Infrastructure.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly DapperContext _context;

        public SkillRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SkillDto>> GetAllByProfileIdAsync(string profileId)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM Skills 
                WHERE ProfileId = @ProfileId 
                ORDER BY Type, [Order]";

            var skills = await connection.QueryAsync<SkillDto>(sql, new { ProfileId = profileId });
            return skills;
        }

        public async Task<SkillDto> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM Skills WHERE Id = @Id";

            var skill = await connection.QueryFirstOrDefaultAsync<SkillDto>(sql, new { Id = id });
            return skill;
        }

        public async Task<SkillDto> CreateAsync(Skill skill)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Önce maksimum sıra numarasını al
                var maxOrder = await GetMaxOrderAsync(skill.ProfileId);
                skill.Order = maxOrder + 1;

                const string sql = @"
                    INSERT INTO Skills (
                        Id, Name, Level, Type, IconUrl, 
                        [Order], ProfileId
                    ) VALUES (
                        @Id, @Name, @Level, @Type, @IconUrl, 
                        @Order, @ProfileId
                    );
                    SELECT * FROM Skills WHERE Id = @Id";

                var createdSkill = await connection.QueryFirstOrDefaultAsync<SkillDto>(sql, skill, transaction);

                transaction.Commit();
                return createdSkill;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<SkillDto> UpdateAsync(Skill skill)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE Skills 
                SET Name = @Name,
                    Level = @Level,
                    Type = @Type,
                    IconUrl = @IconUrl
                WHERE Id = @Id;
                SELECT * FROM Skills WHERE Id = @Id";

            var updatedSkill = await connection.QueryFirstOrDefaultAsync<SkillDto>(sql, skill);
            return updatedSkill;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Önce silinecek skill'in bilgilerini al
                var skill = await GetByIdAsync(id);
                if (skill == null) return false;

                // Skill'i sil
                const string deleteSkillSql = "DELETE FROM Skills WHERE Id = @Id";
                await connection.ExecuteAsync(deleteSkillSql, new { Id = id }, transaction);

                // Sıralamayı güncelle
                const string updateOrdersSql = @"
                    UPDATE Skills 
                    SET [Order] = [Order] - 1 
                    WHERE ProfileId = @ProfileId 
                    AND Type = @Type
                    AND [Order] > @Order";

                await connection.ExecuteAsync(updateOrdersSql,
                    new { ProfileId = skill.ProfileId, Type = skill.Type, Order = skill.Order },
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
                var currentSkill = await GetByIdAsync(id);
                if (currentSkill == null) return false;

                if (newOrder > currentSkill.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE Skills 
                        SET [Order] = [Order] - 1 
                        WHERE ProfileId = @ProfileId 
                        AND Type = @Type
                        AND [Order] > @CurrentOrder 
                        AND [Order] <= @NewOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new
                        {
                            ProfileId = currentSkill.ProfileId,
                            Type = currentSkill.Type,
                            CurrentOrder = currentSkill.Order,
                            NewOrder = newOrder
                        },
                        transaction);
                }
                else if (newOrder < currentSkill.Order)
                {
                    const string updateOthersSql = @"
                        UPDATE Skills 
                        SET [Order] = [Order] + 1 
                        WHERE ProfileId = @ProfileId 
                        AND Type = @Type
                        AND [Order] >= @NewOrder 
                        AND [Order] < @CurrentOrder";

                    await connection.ExecuteAsync(updateOthersSql,
                        new
                        {
                            ProfileId = currentSkill.ProfileId,
                            Type = currentSkill.Type,
                            CurrentOrder = currentSkill.Order,
                            NewOrder = newOrder
                        },
                        transaction);
                }

                const string updateSkillSql = @"
                    UPDATE Skills 
                    SET [Order] = @NewOrder 
                    WHERE Id = @Id";

                await connection.ExecuteAsync(updateSkillSql,
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
                FROM Skills 
                WHERE ProfileId = @ProfileId";

            return await connection.ExecuteScalarAsync<int>(sql, new { ProfileId = profileId });
        }

        public async Task<bool> UpdateLevelAsync(string id, int level)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                UPDATE Skills 
                SET Level = @Level
                WHERE Id = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id, Level = level });
            return result > 0;
        }

        public async Task<IEnumerable<SkillDto>> GetByTypeAsync(string profileId, string type)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT * FROM Skills 
                WHERE ProfileId = @ProfileId 
                AND Type = @Type
                ORDER BY [Order]";

            var skills = await connection.QueryAsync<SkillDto>(sql, new { ProfileId = profileId, Type = type });
            return skills;
        }
    }
}