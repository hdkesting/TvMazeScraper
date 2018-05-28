using System.Collections.Generic;

namespace RtlTvMazeScraper.Models
{
    public class Show
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<CastMember> Cast { get; } = new List<CastMember>();
    }
}