using Microsoft.AspNetCore.Mvc;

public class UsuarioService
{
    private readonly UsuarioDAO _usuarioDAO; 
    private readonly UsuarioMapper _usuarioMapper; 
    private readonly EmailService _emailService; // Inyectar EmailService


    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UsuarioService"/>.
    /// </summary>
    /// <param name="usuarioDAO">El DAO para la interacción con la base de datos.</param>
    /// <param name="usuarioMapper">El mapper para convertir entre entidades y DTOs.</param>
    public UsuarioService(UsuarioDAO usuarioDAO, UsuarioMapper usuarioMapper, EmailService emailService)
    {
        _usuarioDAO = usuarioDAO;
        _usuarioMapper = usuarioMapper;
        _emailService = emailService; // Asignar en el constructor
    }

    /// <summary>
    /// Obtiene todos los usuarios de manera asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos <see cref="UsuarioDTO"/>.</returns>
    public async Task<IActionResult> GetAllAsync()
    {
        var usuarios = await _usuarioDAO.GetAllAsync(); 
        if(usuarios.Any() || usuarios == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("getAllUser404")));
        } 
        var resultado = usuarios.Select(u => _usuarioMapper.ToDTO(u)).ToList();  
        return new OkObjectResult(new ApiResponse<List<UsuarioDTO>>(200,MessageService.Instance.GetMessage("getAlluser200"),resultado));
    }

    /// <summary>
    /// Acviva un usuario que este desactivado.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> o <c>null</c> si no se encuentra.</returns>
    
    public async Task<IActionResult> ActiveteUserAsync(Guid id){
        var usuario = await _usuarioDAO.GetByIdAsync(id);
        if(usuario == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("ActivateUserUser400")));
        }

        if(usuario.is_active){
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("ActivateUserUser404")));
        }
    
        usuario.is_active = true;
        var usuarioActivado = _usuarioMapper.ToDTO(usuario);
        await this.UpdateAsync(usuarioActivado);
       
        var resultado = new UsuarioDTO
            {
                id = usuario.id,
                username = usuario.username,
                is_active = usuario.is_active
            };

        return new OkObjectResult(new ApiResponse<UsuarioDTO>(200,MessageService.Instance.GetMessage("ActivateUserUser200"),resultado));
    }

    /// <summary>
    /// Obtiene un usuario por su ID de manera asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> o <c>null</c> si no se encuentra.</returns>
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var usuario = await _usuarioDAO.GetByIdAsync(id); 
        
        if(usuario == null){
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("GetByIdAsyncUser404")));
        }
        
        var resultado = _usuarioMapper.ToDTO(usuario);
        return new OkObjectResult(new ApiResponse<UsuarioDTO>(200,MessageService.Instance.GetMessage("GetByIdAsyncUser200")));
    }

    /// <summary>
    /// Agrega un nuevo usuario de manera asíncrona.
    /// </summary>
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Una tarea que representa la operación de inserción.</returns>
    public async Task<IActionResult> AddAsync(UsuarioDTO usuarioDTO)
    {

        var usuario = _usuarioMapper.ToEntity(usuarioDTO); 
        await _usuarioDAO.AddAsync(usuario); 
       
        var resultado = new UsuarioDTO{
            id = usuario.id,
            username = usuario.username,
            email = usuario.email,
            first_name = usuario.first_name,
            last_name = usuario.last_name,
            is_active = usuario.is_active,
            role = usuario.role
        };
        
        return new CreatedResult("", new ApiResponse<UsuarioDTO>(201, MessageService.Instance.GetMessage("AddAsyncUser201"),resultado));
    }

    /// <summary>
    /// Actualiza un usuario existente de manera asíncrona.
    /// </summary>
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Una tarea que representa la operación de actualización.</returns>
    public async Task<IActionResult> UpdateAsync(UsuarioDTO usuarioDTO)
    {
        var usuario = _usuarioMapper.ToEntity(usuarioDTO); // Convierte el DTO en una entidad.

        // Asegúrate de que no se rastree una instancia duplicada de la entidad
        var usuarioExistente = await _usuarioDAO.GetByIdAsync(usuario.id);

        if(usuarioExistente == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("controllerUser404")));
        }

        _usuarioDAO.Detach(usuarioExistente);
        await _usuarioDAO.UpdateAsync(usuario);
        var resultado = new UsuarioDTO{
            id = usuario.id,
            username = usuario.username,
            email = usuario.email,
            first_name = usuario.first_name,
            last_name = usuario.last_name,
            is_active = usuario.is_active,
            role = usuario.role
        };

        return new OkObjectResult(new ApiResponse<UsuarioDTO>(200,MessageService.Instance.GetMessage("UpdateAsyncUser200"),resultado));
    }

    /// <summary>
    /// Elimina un usuario por su ID de manera asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del usuario a eliminar.</param>
    /// <returns>Una tarea que representa la operación de eliminación.</returns>
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existeusuario = await _usuarioDAO.GetByIdAsync(id);
        if(existeusuario == null)
        {
           return new NotFoundObjectResult(new ApiResponse<string>(404, MessageService.Instance.GetMessage("controllerUser404")));     
        }
        if(existeusuario.is_deleted)
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("DeleteAsyncUser400")));
        }
        await _usuarioDAO.DeleteAsync(id); 
        return new OkObjectResult(new ApiResponse<string>(200,MessageService.Instance.GetMessage("DeleteAsyncUser200")));
    }

    /// <summary>
    /// Actualiza parcialmente un usuario existente de manera asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del usuario a actualizar.</param>
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Una tarea que representa la operación de actualización parcial.</returns>
    public async Task UpdatePartialAsync(Guid id, UsuarioDTO usuarioDTO)
    {
        var usuarioParcial = _usuarioMapper.ToEntity(usuarioDTO); // Convierte el DTO en una entidad.
        await _usuarioDAO.UpdatePartialAsync(id, usuarioParcial); // Actualiza parcialmente la entidad en la base de datos.
    }

    /// <summary>
    /// Restaura un usuario eliminado lógicamente.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> del usuario restaurado o <c>null</c> si no se encuentra.</returns>
    /// <exception cref="InvalidOperationException">Si el usuario no estaba eliminado.</exception>
    public async Task<IActionResult> RestoreUserAsync(Guid id)
    {
        
        var restaurado = await _usuarioDAO.RestoreAsync(id); 
        if (!restaurado)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("RestoreUserAsyncUser404"))); // Usuario no encontrado (404)
        }

        var usuario = await _usuarioDAO.GetByIdAsync(id, true); 
        
        if (usuario == null) 
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404, MessageService.Instance.GetMessage("RestoreUserAsyncUser404")));
        }

        var result = _usuarioMapper.ToDTO(usuario);
        return new OkObjectResult(new ApiResponse<UsuarioDTO>(200,MessageService.Instance.GetMessage("RestoreUserAsyncUser200"),result));
    }

    /// <summary>
    /// Verifica el correo electrónico de un usuario.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>El DTO del usuario actualizado, o null si el usuario no existe.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si el correo ya estaba verificado.</exception>
    public async Task<UsuarioDTO?> VerifyEmailAsync(Guid id)
    {
        var usuario = await _usuarioDAO.GetByIdAsync(id); // Busca el usuario en la base de datos

        if (usuario == null)
        {
            return null; // Retorna null si el usuario no existe (404)
        }

        if (usuario.email_verified)
        {
            throw new InvalidOperationException("El email ya estaba verificado."); // Lanza excepción si ya está verificado (400)
        }

        usuario.email_verified = true;
        usuario.email_verified_at = DateTime.UtcNow; // Establece la fecha y hora actual en UTC

        await _usuarioDAO.UpdateAsync(usuario); // Guarda los cambios en la base de datos

        return _usuarioMapper.ToDTO(usuario); // Retorna el usuario actualizado como DTO
    }
    /// <summary>
    /// Desactiva un usuario estableciendo is_active en false.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>El DTO del usuario desactivado, o null si no se encuentra.</returns>
    /// <exception cref="InvalidOperationException">Si el usuario ya está desactivado.</exception>
    public async Task<IActionResult> DeactivateUserAsync(Guid id)
    {
        var usuario = await _usuarioDAO.GetByIdAsync(id);

        if (usuario == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("controllerUser404")));
        }

        if (!usuario.is_active)
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("DeactivateUserAsyncUser400")));
        }

        usuario.is_active = false;
        await _usuarioDAO.UpdateAsync(usuario);

        var usuarioActivado = _usuarioMapper.ToDTO(usuario); 
        var resultado = new UsuarioDTO{
            id = usuarioActivado.id,
            username = usuarioActivado.username,
            is_active = usuarioActivado.is_active
        };
        return new OkObjectResult(new ApiResponse<UsuarioDTO>(200,"DeactivateUserAsyncUser200",resultado));
    }
    
    /// <summary>
    /// Cambia la contraseña de un usuario autenticado.
    /// </summary>
    /// <param name="id">Identificador único del usuario (UUID).</param>
    /// <param name="model">DTO con la contraseña actual, nueva contraseña y confirmación.</param>
    /// <returns>Retorna true si la contraseña fue cambiada exitosamente.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza si el usuario no existe.</exception>
    /// <exception cref="InvalidOperationException">Se lanza si la contraseña actual es incorrecta, 
    /// la nueva contraseña no cumple los requisitos o las contraseñas no coinciden.</exception>
    public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDTO model)
    {
        var usuario = await _usuarioDAO.GetByIdAsync(id);
        
        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado.");
        }

        // Si la contraseña almacenada NO tiene formato BCrypt, la hasheamos primero
        if (!usuario.password.StartsWith("$2a$") && !usuario.password.StartsWith("$2b$") && !usuario.password.StartsWith("$2y$"))
        {
            usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password, 12);
            await _usuarioDAO.UpdateAsync(usuario);
        }

        // Verificar que la contraseña actual sea correcta
        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, usuario.password))
        {
            throw new InvalidOperationException("La contraseña actual no es correcta.");
        }

        // Validar que la nueva contraseña cumple con seguridad
        if (model.NewPassword.Length < 8 || !model.NewPassword.Any(char.IsDigit) || !model.NewPassword.Any(char.IsLetter))
        {
            throw new InvalidOperationException("La nueva contraseña debe tener al menos 8 caracteres y contener letras y números.");
        }

        // Validar que la confirmación coincida con la nueva contraseña
        if (model.NewPassword != model.ConfirmPassword)
        {
            throw new InvalidOperationException("La nueva contraseña y la confirmación no coinciden.");
        }

        // Hashear la nueva contraseña antes de guardarla
        usuario.password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, 12);
        await _usuarioDAO.UpdateAsync(usuario);

        return true;
    }

    public async Task<(int codigo, string mensaje)> RequestPasswordResetAsync(string email)
    {
        var usuario = await _usuarioDAO.GetByEmailAsync(email);

        if (usuario == null)
        {
            return (404, "No se encontró una cuenta con ese email.");
        }

        if (usuario.email_verified==false)
        {
            return (400, "El email no ha sido verificado.");
        }

        // Generar token seguro para restablecimiento (GUID en Base64 sin caracteres conflictivos)
        var tokenBytes = Guid.NewGuid().ToByteArray();
        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");

        usuario.password_reset_token = token;
        usuario.password_reset_token_expiration = DateTime.UtcNow.AddHours(1); // Expira en 1 hora

        await _usuarioDAO.UpdateAsync(usuario);

        try
        {
            // TODO: Implementar envío de correo
            await _emailService.SendPasswordResetEmail(usuario.email, token);
        }
        catch (Exception)
        {
            return (500, "Error al enviar el correo de restablecimiento.");
        }

        return (200, "Se ha enviado un correo con el enlace de restablecimiento.");
    }


}
