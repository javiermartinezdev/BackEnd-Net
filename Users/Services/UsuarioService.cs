using Microsoft.AspNetCore.Mvc;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioDAO _iUsuarioDAO; 
    private readonly UsuarioMapper _usuarioMapper; 
    private readonly CreateUserMapper _createUserMapper; 
    private readonly PasswordResetEmail _emailService; // Inyectar EmailService


    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UsuarioService"/>.
    /// </summary>
    /// <param name="usuarioDAO">El DAO para la interacción con la base de datos.</param>
    /// <param name="usuarioMapper">El mapper para convertir entre entidades y DTOs.</param>
    public UsuarioService(IUsuarioDAO iUsuarioDAO, UsuarioMapper usuarioMapper, CreateUserMapper createUserMapper, PasswordResetEmail emailService)
    {
        _iUsuarioDAO = iUsuarioDAO;
        _usuarioMapper = usuarioMapper;
        _createUserMapper = createUserMapper;
        _emailService = emailService; // Asignar en el constructor
    }

    /// <summary>
    /// Obtiene todos los usuarios de manera asíncrona.
    /// Obtiene una lista de usarios donde tengamos que pasarle parametros 
    /// </summary>
    /// <returns>Una lista de objetos <see cref="UsuarioDTO"/>.</returns>
    public async  Task<IActionResult> GetAllAsync2(int? safePage, int? safeLimit, string? sort, string? safeOrder, bool? safeActive, bool? safeisDeleted, bool? safeIsSuperuser, bool? safeEmailVerified)
    {
        if(!safePage.HasValue && !safeLimit.HasValue && string.IsNullOrEmpty(sort) && string.IsNullOrEmpty(safeOrder) && !safeActive.HasValue && !safeisDeleted.HasValue && !safeIsSuperuser.HasValue && !safeEmailVerified.HasValue)
        {
            safeisDeleted = false;
        }
        
        SentenciaUsuarios Crearsentencia = new SentenciaUsuarios(safePage, safeLimit, sort, safeOrder, safeActive, safeisDeleted, safeIsSuperuser, safeEmailVerified);
        

        var sentencia = Crearsentencia.CrearSenentiaSQLUser();
        var usuarios = await _iUsuarioDAO.GetUserAsync(sentencia.Sentencia, sentencia.Parametros);
        
        if(usuarios == null || !usuarios.Any())
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("getAllUser404")));
        }
        
        var numResultado = usuarios.Count();
        var resultado = usuarios.Select(u => _usuarioMapper.ToDTO(u)).ToList();
        
        /// <summary>
        /// Retornamos un ok 200 y un mensaje de éxito.
        /// Utilizamos ApiResponse para estructurar la respuesta.
        /// La respuesta incluye un objeto ResponseGetUsers que contiene información adicional sobre la paginación y los filtros aplicados.
        /// </summary>
        return new OkObjectResult(new ApiResponse<ResponseGetUsers<List<UsuarioDTO>>>(200,MessageService.Instance.GetMessage("getAlluser200"),
                                  new ResponseGetUsers<List<UsuarioDTO>>(numResultado,         //Total de usuarios
                                                                         sentencia.valores[7], //Valor correspondiente a la paginación
                                                                         sentencia.valores[6], //Valor correspondiente al límite
                                                                         sentencia.valores[4], //Valor correspondiente al sort
                                                                         sentencia.valores[5], //Valor correspondiente al safeOrder
                                                                         sentencia.valores[0], //Valor correspondiente a la actividad
                                                                         sentencia.valores[1], //Valor correspondiente a la eliminación
                                                                         sentencia.valores[2], //Valor correspondiente al superusuario
                                                                         sentencia.valores[3], //Valor correspondiente a la verificación del correo
                                                                         resultado             //Lista de usuarios
                                                                         )));
    
    }

    /// <summary>
    /// Acviva un usuario que este desactivado.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> o <c>null</c> si no se encuentra.</returns>
    
    public async Task<IActionResult> ActiveteUserAsync(Guid id)
    {
        
        var usuario = await _iUsuarioDAO.GetByIdAsync(id);
        
        if(usuario == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("ActivateUserUser404")));
        }

        if(usuario.is_active){
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("ActivateUserUser400")));
        }
    
        usuario.is_active = true;
        
        var usuarioActivado = _usuarioMapper.ToDTO(usuario);
        
        await _iUsuarioDAO.UpdateAsync(usuario);
        
        var resultado = new UsuarioDTOResponce(id,usuarioActivado);

        return new OkObjectResult(new ApiResponse<UsuarioDTOResponce>(200,MessageService.Instance.GetMessage("ActivateUserUser200"),resultado));
    }

    /// <summary>
    /// Obtiene un usuario por su ID de manera asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> o <c>null</c> si no se encuentra.</returns>
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var usuario = await _iUsuarioDAO.GetByIdAsync(id); 
        
        if(usuario == null){
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("GetByIdAsyncUser404")));
        }
        
        var resultado = _usuarioMapper.ToDTO(usuario);
        return new OkObjectResult(new ApiResponse<UsuarioDTO>(200,MessageService.Instance.GetMessage("GetByIdAsyncUser200"),resultado));
    }

    /// <summary>
    /// Agrega un nuevo usuario de manera asíncrona.
    /// </summary>
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Una tarea que representa la operación de inserción.</returns>
    public async Task<IActionResult> AddAsync(UsuarioCreateDTO usuarioCreateDTO)
    {
        
        SentenciaUsuarios Crearsentencia = new SentenciaUsuarios(usuarioCreateDTO.email, usuarioCreateDTO.username);
        var sentencia = Crearsentencia.CrearSentenciaSQLValidarEmailUsername();
        var existeUsuario = await _iUsuarioDAO.GetUserAsync(sentencia.Sentencia, sentencia.Parametros);
        

        if(!(existeUsuario == null || !existeUsuario.Any()))
        {
            return new ObjectResult(new ApiResponse<string>(409,MessageService.Instance.GetMessage("AddAsyncUser409"))){
                StatusCode = StatusCodes.Status409Conflict
            };
        }
        
        var usuario = _createUserMapper.ToEntity(usuarioCreateDTO); 
        var usuarioDTO = _usuarioMapper.ToDTO(usuario);
        
        await _iUsuarioDAO.AddAsync(usuario); 
        
        var resultado = new UsuarioDTOResponceExtends(usuario.id,usuarioDTO); 
        return new CreatedResult("", new ApiResponse<UsuarioDTOResponceExtends>(201, MessageService.Instance.GetMessage("AddAsyncUser201"),resultado));

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
        var usuarioExistente = await _iUsuarioDAO.GetByIdAsync(usuario.id);

        if(usuarioExistente == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("controllerUser404")));
        }

        _iUsuarioDAO.Detach(usuarioExistente);
        await _iUsuarioDAO.UpdateAsync(usuario);

        var resultado = new UsuarioDTOResponceExtends(usuario.id,usuarioDTO);

        return new OkObjectResult(new ApiResponse<UsuarioDTOResponceExtends>(200,MessageService.Instance.GetMessage("UpdateAsyncUser200"),resultado));
    }

    /// <summary>
    /// Elimina un usuario por su ID de manera asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del usuario a eliminar.</param>
    /// <returns>Una tarea que representa la operación de eliminación.</returns>
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existeusuario = await _iUsuarioDAO.GetByIdAsync(id);
        if(existeusuario == null)
        {
           return new NotFoundObjectResult(new ApiResponse<string>(404, MessageService.Instance.GetMessage("controllerUser404")));     
        }
        if(existeusuario.is_deleted)
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("DeleteAsyncUser400")));
        }
        await _iUsuarioDAO.DeleteAsync(id); 
        return new OkObjectResult(new ApiResponse<string>(200,MessageService.Instance.GetMessage("DeleteAsyncUser200")));
    }

    /// <summary>
    /// Actualiza parcialmente un usuario existente de manera asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del usuario a actualizar.</param>
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Una tarea que representa la operación de actualización parcial.</returns>
    public async Task<IActionResult> UpdatePartialAsync(Guid id, UsuarioPatchDTO usuarioDTO)
    {
        
        var existingUser = await _iUsuarioDAO.GetByIdAsync(id);
        
        if (existingUser == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("UpdatePartialAsyncUser404")));
        }

        if(existingUser.is_deleted)
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("UpdatePartialAsyncUser400")));
        }
        
        //Actualiza parcialmente el usuario
        existingUser.first_name = usuarioDTO.first_name ?? existingUser.first_name;
        existingUser.last_name = usuarioDTO.last_name ?? existingUser.last_name;
        existingUser.email = usuarioDTO.email ?? existingUser.email;

        existingUser.modified_at = DateTimeOffset.UtcNow;
        await _iUsuarioDAO.UpdateAsync(existingUser); // Guarda los cambios en la base de datos
        
        var resultado = _usuarioMapper.ToDTO(existingUser); // Convierte la entidad actualizada a DTO
        var resultadoDTO = new UsuarioDTOResponceExtends(id, resultado); // Crea un nuevo DTO de respuesta
        
        return new OkObjectResult(new ApiResponse<UsuarioDTOResponceExtends>(200,MessageService.Instance.GetMessage("UpdatePartialAsyncUser200"),resultadoDTO));
    }

    /// <summary>
    /// Restaura un usuario eliminado lógicamente.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> del usuario restaurado o <c>null</c> si no se encuentra.</returns>
    /// <exception cref="InvalidOperationException">Si el usuario no estaba eliminado.</exception>
    public async Task<IActionResult> RestoreUserAsync(Guid id)
    {
        var restaurarUsuario = await _iUsuarioDAO.GetByIdAsync(id);
        
        if (restaurarUsuario == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("RestoreUserAsyncUser404")));
        }
        
        if (!restaurarUsuario.is_deleted)
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("RestoreUserAsyncUser400")));
        }
        
        restaurarUsuario.is_deleted = false;
        restaurarUsuario.deleted_at = null; 
        restaurarUsuario.modified_at = DateTimeOffset.UtcNow; 
        await _iUsuarioDAO.UpdateAsync(restaurarUsuario);
  
        var resultado = _usuarioMapper.ToDTO(restaurarUsuario);
        return new OkObjectResult(new ApiResponse<UsuarioDTO>(200,MessageService.Instance.GetMessage("RestoreUserAsyncUser200"),resultado));
    }

    /// <summary>
    /// Verifica el correo electrónico de un usuario.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>El DTO del usuario actualizado, o null si el usuario no existe.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si el correo ya estaba verificado.</exception>
    public async Task<IActionResult> VerifyEmailAsync(Guid id)
    {
        var usuario = await _iUsuarioDAO.GetByIdAsync(id); // Busca el usuario en la base de datos

        if (usuario == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("controllerUser404")));
        }

        if (usuario.email_verified)
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("VerifyEmailAsyncUser400"))); 
        }

        usuario.email_verified = true;
        usuario.email_verified_at = DateTime.UtcNow; 

        await _iUsuarioDAO.UpdateAsync(usuario); 
        
        var resultado = new UsuarioDTOVerifiedEmail(id,usuario);

        return new OkObjectResult(new ApiResponse<UsuarioDTOVerifiedEmail>(200,MessageService.Instance.GetMessage("VerifyEmailAsyncUser200"),resultado));
    }
    /// <summary>
    /// Desactiva un usuario estableciendo is_active en false.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>El DTO del usuario desactivado, o null si no se encuentra.</returns>
    /// <exception cref="InvalidOperationException">Si el usuario ya está desactivado.</exception>
    public async Task<IActionResult> DeactivateUserAsync(Guid id)
    {
        var usuario = await _iUsuarioDAO.GetByIdAsync(id);

        if (usuario == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("controllerUser404")));
        }

        if (!usuario.is_active)
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("DeactivateUserAsyncUser400")));
        }

        usuario.is_active = false;
        await _iUsuarioDAO.UpdateAsync(usuario);

        var usuarioActivado = _usuarioMapper.ToDTO(usuario); 

        var resultado = new UsuarioDTOResponce(id,usuarioActivado);
        
        return new OkObjectResult(new ApiResponse<UsuarioDTOResponce>(200,"DeactivateUserAsyncUser200",resultado));
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
    public async Task<IActionResult> ChangePasswordAsync(Guid id, ChangePasswordDTO model)
    {
        var usuario = await _iUsuarioDAO.GetByIdAsync(id);
        
        if (usuario == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404,MessageService.Instance.GetMessage("controllerUser404")));
        }

        // Si la contraseña almacenada NO tiene formato BCrypt, la hasheamos primero
        if (!usuario.password.StartsWith("$2a$") && !usuario.password.StartsWith("$2b$") && !usuario.password.StartsWith("$2y$"))
        {
            usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password, 12);
            await _iUsuarioDAO.UpdateAsync(usuario);
        }

        // Verificar que la contraseña actual sea correcta
        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, usuario.password))
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("ChangePasswordAsyncUser400")));
        }

        // Validar que la nueva contraseña cumple con seguridad
        if (model.NewPassword.Length < 8 || !model.NewPassword.Any(char.IsDigit) || !model.NewPassword.Any(char.IsLetter))
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("ChangePasswordAsyncUser400Size")));
        }

        // Validar que la confirmación coincida con la nueva contraseña
        if (model.NewPassword != model.ConfirmPassword)
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400,MessageService.Instance.GetMessage("ChangePasswordAsyncUser400Confirmation")));
        }

        // Hashear la nueva contraseña antes de guardarla
        usuario.password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, 12);
        await _iUsuarioDAO.UpdateAsync(usuario);

        return new OkObjectResult(new ApiResponse<string>(200,MessageService.Instance.GetMessage("ChangePasswordAsyncUser200")));  
    }

    public async Task<IActionResult> RequestPasswordResetAsync(string email)
    {
        var usuario = await _iUsuarioDAO.GetByEmailAsync(email);

        if (usuario == null)
        {
            return new NotFoundObjectResult(new ApiResponse<string>(404, "RequestPasswordResetAsyncUser404"));
        }

        if (usuario.email_verified==false)
        {
            return new BadRequestObjectResult(new ApiResponse<string>(400, "RequestPasswordResetAsyncUser400"));
        }

        // Generar token seguro para restablecimiento (GUID en Base64 sin caracteres conflictivos)
        var tokenBytes = Guid.NewGuid().ToByteArray();
        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");

        usuario.password_reset_token = token;
        usuario.password_reset_token_expiration = DateTime.UtcNow.AddHours(1); // Expira en 1 hora

        await _iUsuarioDAO.UpdateAsync(usuario);

        try
        {
            if(usuario?.email == null){
                return new NotFoundObjectResult(new ApiResponse<string>(404, "RequestPasswordResetAsyncUser404"));
            }
            
            await _emailService.SendPasswordResetEmail(usuario.email, token);

            return new OkObjectResult(new ApiResponse<string>(200, "RequestPasswordResetAsyncUser200"));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex.Message);
            return new ObjectResult(new ApiResponse<string>(500, "RequestPasswordResetAsyncUser500"));
        }

    }
}
