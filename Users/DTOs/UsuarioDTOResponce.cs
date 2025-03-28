using apitienda.Models;

public class UsuarioDTOResponce
{
    public Guid Id { get; set; }
    public string username { get; set; }
    public string? email { get; set; }
    
    public UsuarioDTOResponce(Guid id,UsuarioDTO usuario)
    {
        Id = id;
        username = usuario.username;
        email = usuario.email;
    }
    
}