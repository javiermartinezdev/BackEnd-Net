using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apitienda.Models
{
    /// <summary>
    /// Representa la entidad de usuario en el sistema.
    /// Contiene información personal, de autenticación y estado del usuario.
    /// </summary>
    public class Usuario
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string password { get; set; } = string.Empty;
        public DateTimeOffset? last_login { get; set; }
        public bool is_superuser { get; set; } = false;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public bool is_active { get; set; } = true;
        public DateTimeOffset date_joined { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset created_at { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset modified_at { get; set; } = DateTimeOffset.UtcNow;
        public bool is_deleted { get; set; } = false;
        public DateTimeOffset? deleted_at { get; set; }
        public string username { get; set; } = string.Empty;
        public string? email { get; set; }
        public string? profile_picture { get; set; }
        public bool email_verified { get; set; } = false;
        public DateTimeOffset? email_verified_at { get; set; }
        public string? nationality { get; set; }
        public string? occupation { get; set; }
        public DateTime? date_of_birth { get; set; }
        public string? contact_phone_number { get; set; }
        public string? gender { get; set; }
        public string? address { get; set; }
        public string? address_number { get; set; }
        public string? address_interior_number { get; set; }
        public string? address_complement { get; set; }
        public string? address_neighborhood { get; set; }
        public string? address_zip_code { get; set; }
        public string? address_city { get; set; }
        public string? address_state { get; set; }
        public string? role { get; set; }
        public string? password_reset_token { get; set; }
        public DateTimeOffset? password_reset_token_expiration { get; set; }

    }
}