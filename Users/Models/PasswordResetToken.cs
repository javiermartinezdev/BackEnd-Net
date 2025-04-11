using System.ComponentModel.DataAnnotations;

public class PasswordResetToken
{
    [Key]
    public required string Token { get; set; }
    public required string Email { get; set; }
    public  required DateTime Expiration { get; set; }
    public required Guid UserId { get; set; }
}