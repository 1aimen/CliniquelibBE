using System.Text.Json;

namespace Cliniquelib_BE.Models
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LegalName { get; set; }
        public string CountryCode { get; set; }
        public string Timezone { get; set; }
        public JsonDocument Meta { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
