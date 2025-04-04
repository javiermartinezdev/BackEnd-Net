using apitienda.Models;

public class CreateUserMapper
{
    public Usuario ToEntity(UsuarioCreateDTO userCreateDTO)
    {
        return new Usuario
        {
            // Generados automáticamente
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

            // Del DTO
            username = userCreateDTO.username,
            password = userCreateDTO.password,
            email = userCreateDTO.email,
            first_name = userCreateDTO.first_name,
            last_name = userCreateDTO.last_name,
            is_active = userCreateDTO.is_active,
            is_superuser = userCreateDTO.is_superuser,
            profile_picture = userCreateDTO.profile_picture,
            nationality = userCreateDTO.nationality,
            occupation = userCreateDTO.occupation,
            date_of_birth = userCreateDTO.date_of_birth,
            contact_phone_number = userCreateDTO.contact_phone_number,
            gender = userCreateDTO.gender,
            address = userCreateDTO.address,
            address_number = userCreateDTO.address_number,
            address_interior_number = userCreateDTO.address_interior_number,
            address_complement = userCreateDTO.address_complement,
            address_neighborhood = userCreateDTO.address_neighborhood,
            address_zip_code = userCreateDTO.address_zip_code,
            address_city = userCreateDTO.address_city,
            address_state = userCreateDTO.address_state,
            role = userCreateDTO.role,

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