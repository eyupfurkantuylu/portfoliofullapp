using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortfolioFullApp.Core.Entities
{
    public class Project : BaseEntity
    {
        public string Title { get; set; }
        public string Href { get; set; }
        public string Dates { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public string Technologies { get; set; }
        [JsonIgnore]
        public List<ProjectLink> Links { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public int Order { get; set; }
    }
}