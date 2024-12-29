namespace PortfolioFullApp.Core.Entities
{
    public class Skill : BaseEntity
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}