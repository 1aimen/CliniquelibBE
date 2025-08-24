using System.Text.Json;

namespace Cliniquelib_BE.Models
{
    public class Clinic
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public JsonDocument Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Timezone { get; set; }
        public JsonDocument Meta { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
