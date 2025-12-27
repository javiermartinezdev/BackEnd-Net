using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitienda.Models
{
    /// <summary>
    /// Representa un refresh token revocado para invalidarlo antes de su expiración.
    /// </summary>
    public class TokenBlacklist
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Identificador único del JWT (claim "jti").
        /// </summary>
        [Required]
        public string Jti { get; set; } = null!;

        /// <summary>
        /// Identificador del usuario propietario del token.
        /// </summary>
        [Required]
        public Guid UsuarioId { get; set; }

        /// <summary>
        /// Fecha y hora en que se revocó el token.
        /// </summary>
        public DateTimeOffset? RevokedAt { get; set; }

        /// <summary>
        /// Fecha y hora en que expiraba el token original.
        /// </summary>
        [Required]
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// Motivo de la revocación (logout, refresh, seguridad, etc.).
        /// </summary>
        public string? Reason { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

    }
}
