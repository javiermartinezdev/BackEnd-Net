using apitienda.Models;

public class UsuarioDTOVerifiedEmail
{   
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public bool Email_verified { get; set; } = false;
    public DateTimeOffset? Email_verified_at { get; set; }

    public UsuarioDTOVerifiedEmail(Guid id, Usuario usuarioVerificadoEmail)
    {
        Id = id;
        Email = usuarioVerificadoEmail.email;
        Email_verified = usuarioVerificadoEmail.email_verified;
        Email_verified_at = usuarioVerificadoEmail.email_verified_at;
    }
    

}