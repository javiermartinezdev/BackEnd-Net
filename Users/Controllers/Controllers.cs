using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using apitienda.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Sockets; // Asegúrate de incluir esta directiva

/// <summary>
/// Controlador para gestionar las operaciones relacionadas con los usuarios.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class usersController : ControllerBase
{
    private readonly UsuarioDAO _usuarioDAO;
    private readonly UsuarioMapper _usuarioMapper;
    private readonly IUsuarioService _usuarioService;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UsuarioController"/>.
    /// </summary>
    public usersController(UsuarioDAO usuarioDAO, UsuarioMapper usuarioMapper, UsuarioService usuarioService)
    {
        _usuarioDAO = usuarioDAO;
        _usuarioMapper = usuarioMapper;
        _usuarioService = usuarioService;
    }
    
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsersAsync(int? page,int? limit,string? sort,string? order,bool? is_active,bool? is_deleted,bool? is_superuser, bool? email_veridied)
    {
        try
        {
            if(!(page==null))
            {
                if(page < 1)
                {
                    return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
                }
            }
            
            if(!(limit==null))
            {
                if(limit < 1)
                {
                    return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
                }
            }
            
            if(!(order == null))
            {
                if(!(order == "asc" || order == "desc"))
                {
                    return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
                }
            }

            if(!(sort == null))
            {
                if(!(sort == "username" || sort == "email" || sort == "date_joined" || sort == "first_name" || sort == "last_name"
                  || sort == "is_active" || sort == "is_deleted" || sort == "is_superuser" || sort == "email_verified" || sort == "date_of_birth"
                  || sort == "nationality" || sort == "occupation" || sort == "gender" || sort == "role"))
                {
                    return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
                }
            }

            /// <summary>
            /// Se determina su el valor es null y si no lo es se le asigna el valor despues de '??' a las variables.
            /// </summary>
            int safePage = page ?? 1;
            int safeLimit = limit ?? 10;
            string safeOrder = order ?? "asc";
            bool safeIsdeleted = is_deleted ?? false;
            bool safeIsActive = is_active ?? true;
            bool safeEmailVeridied = email_veridied ?? true;
            bool safeIsSuperuser = is_superuser ?? false;

            var usuarios = await _usuarioService.GetAllAsync2( safePage,safeLimit, sort,safeOrder,safeIsActive,safeIsdeleted, safeIsSuperuser,safeEmailVeridied);    
            return usuarios;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

    /// <summary>
    /// Obtiene un usuario por su identificador único.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            if(id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }

            var usuario = await _usuarioService.GetByIdAsync(id);
            return usuario;    
        }
        catch(Exception ex){
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500 , new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerUser500")));
        }
        
    }

    /// <summary>
    /// Crea un nuevo usuario.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UsuarioDTO usuarioDTO)
    {
        try
        {
            if (usuarioDTO == null)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controllerPostUser")));
            }
            
            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controllerPostUser")));
            }

            var usuario = await _usuarioService.AddAsync(usuarioDTO);
            return usuario;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerUser500")));
        }
        
    }

    /// <summary>
    /// Actualiza un usuario existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UsuarioDTO usuarioDTO)
    {
        try
        {
            if(usuarioDTO == null)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }
           
            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }

            var updatedUser = await _usuarioService.UpdateAsync(usuarioDTO);
            return updatedUser;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error 500: "+ ex);
            return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

    /// <summary>
    /// Elimina un usuario por su identificador único.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            if(id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }

            var existingUser = await _usuarioService.GetByIdAsync(id);
            return existingUser;
        
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerUser500")));
        }
        
    }

    /// <summary>
    /// Actualiza parcialmente un usuario existente.
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] UsuarioPatchDTO usuarioDTO)
    {
        try{
            if(id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }

            if(usuarioDTO == null)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }
            
             var existingUser = await _usuarioService.UpdatePartialAsync(id,usuarioDTO);
             return existingUser;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

    /// <summary>
    /// Restaura un usuario eliminado.
    /// </summary>
    [HttpPatch("{id}/restore")]
    public async Task<IActionResult> RestoreUserAsync(Guid id)
    {
        try
        {
            if(id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }

            var usuarioRestaurado = await _usuarioService.RestoreUserAsync(id);
            return usuarioRestaurado;

        }catch (Exception ex)
        {
            Console.WriteLine("Error 500:" + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

    /// <summary>
    /// Endpoint HTTP POST para verificar el correo electrónico de un usuario.
    /// </summary>
    [HttpPost("{id}/verify-email")]
    public async Task<IActionResult> VerifyEmail(Guid id)
    {
        try
        {
            var usuarioVerificado = await _usuarioService.VerifyEmailAsync(id); 
            return usuarioVerificado;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

    /// <summary>
    /// Endpoint HTTP PATCH para activar un usuario estableciendo el campo is_active en true.
    /// </summary>
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        try{
            
            if(id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }

            var activarUsuario = await _usuarioService.ActiveteUserAsync(id);
            return activarUsuario;
        
        }catch(Exception ex){
            Console.WriteLine("Error 500: "+ ex);
            return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
    
    /// <summary>
    /// Desactiva un usuario estableciendo is_active en false.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        try
        {
            if(id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }

            var usuario = await _usuarioService.DeactivateUserAsync(id);
            return usuario;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// </summary>
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDTO model)
    {
        
        try{
            
            if(id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(400,MessageService.Instance.GetMessage("controller400")));
            }

            var result = await _usuarioService.ChangePasswordAsync(id, model);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
        
    }

    /// <summary>
    /// Solicita el restablecimiento de contraseña enviando un enlace al correo del usuario.
    /// </summary>
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDTO request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }
            
            var respuesta = await _usuarioService.RequestPasswordResetAsync(request.Email);
            return respuesta;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
}
