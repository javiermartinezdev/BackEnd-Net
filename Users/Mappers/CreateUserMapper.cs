using apitienda.Models;

public class CreateUserMapper
{
    public Usuario ToEntity(UsuarioCreateDTO userDTO)
    {
        return new Usuario
        {
            //Lo que se genera automaticamente
            id = Guid.NewGuid(),
            created_at = DateTimeOffset.UtcNow,
            modified_at = DateTimeOffset.UtcNow,
            date_joined = DateTimeOffset.UtcNow,
            is_deleted = false,
            deleted_at = null,
            email_verified = false,
            email_verified_at = null,
            last_login = null,
            password_reset_token = null,
            password_reset_token_expiration = null,
            
            //Los los datos del cliente nos dio
            username = userDTO.username,
            password = userDTO.password,
            email = userDTO.email,
            first_name = userDTO.first_name,
            last_name = userDTO.last_name,
            is_active = userDTO.is_active,
            is_superuser = userDTO.is_superuser,
            profile_picture = userDTO.profile_picture,
            nationality = userDTO.nationality,
            occupation = userDTO.occupation,
            date_of_birth = userDTO.date_of_birth,
            contact_phone_number = userDTO.contact_phone_number,
            gender = userDTO.gender,
            address = userDTO.address,
            address_number = userDTO.address_number,
            address_interior_number = userDTO.address_interior_number,
            address_complement = userDTO.address_complement,
            address_neighborhood = userDTO.address_neighborhood,
            address_zip_code = userDTO.address_zip_code,
            address_city = userDTO.address_city,
            address_state = userDTO.address_state,
            role = userDTO.role
        
        };
    }
    public UsuarioDTOResponceExtends ToDTO(Usuario user)
    {
        UsuarioMapper _mapper = new UsuarioMapper();
        var usuarioDTO = _mapper.ToDTO(user);
        UsuarioDTOResponceExtends respuesta = new UsuarioDTOResponceExtends(user.id, usuarioDTO);
        return respuesta;
    }
}