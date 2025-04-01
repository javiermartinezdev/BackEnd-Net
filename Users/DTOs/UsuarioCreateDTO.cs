using System.ComponentModel.DataAnnotations;
public class UsuarioCreateDTO
{
    
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public required string username { get; set; }
    
    [Required]
    public required string password { get; set; }
    public string? email { get; set; }
    [Required]
    public required string first_name { get; set; }
    [Required]
    public required string last_name { get; set; }
    public required bool is_active { get; set; }
    public bool is_superuser { get; set; } = false;
    public string? profile_picture { get; set; }
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
}