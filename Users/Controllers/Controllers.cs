using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using apitienda.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal; // Asegúrate de incluir esta directiva

/// <summary>
/// Controlador para gestionar las operaciones relacionadas con los usuarios.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class usersController : ControllerBase
{
    private readonly UsuarioDAO _usuarioDAO;
    private readonly UsuarioMapper _usuarioMapper;
    private readonly UsuarioService _usuarioService;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UsuarioController"/>.
    /// </summary>
    /// <param name="usuarioDAO">El objeto DAO para acceder a los datos de los usuarios.</param>
    /// <param name="usuarioMapper">El objeto Mapper para convertir entre entidades y DTOs.</param>
    /// <param name="usuarioService">El objeto Service para la lógica de negocio de los usuarios.</param>
    public usersController(UsuarioDAO usuarioDAO, UsuarioMapper usuarioMapper, UsuarioService usuarioService)
    {
        _usuarioDAO = usuarioDAO;
        _usuarioMapper = usuarioMapper;
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Obtiene una lista de todos los usuarios.
    /// </summary>
    /// <returns>Una lista de objetos <see cref="UsuarioDTO"/>.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return usuarios;
        }   
        catch(Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

    /// <summary>
    /// Obtiene un usuario por su identificador único.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/>.</returns>
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
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> creado.</returns>
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
    /// <param name="id">El identificador único del usuario.</param>
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Una respuesta de estado HTTP.</returns>
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
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Una respuesta de estado HTTP.</returns>
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
    /// <param name="id">El identificador único del usuario.</param>
    /// <param name="usuarioDTO">Los datos parciales del usuario en formato DTO.</param>
    /// <returns>Una respuesta de estado HTTP.</returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] UsuarioPatchDTO usuarioDTO)
    {
        if (usuarioDTO == null)
        {
            return BadRequest();
        }

        var existingUser = await _usuarioDAO.GetByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        var usuarioParcial = new Usuario();
        if (!string.IsNullOrEmpty(usuarioDTO.first_name))
            usuarioParcial.first_name = usuarioDTO.first_name;
        if (!string.IsNullOrEmpty(usuarioDTO.last_name))
            usuarioParcial.last_name = usuarioDTO.last_name;
        if (!string.IsNullOrEmpty(usuarioDTO.email))
            usuarioParcial.email = usuarioDTO.email;

        await _usuarioDAO.UpdatePartialAsync(id, usuarioParcial);
        return NoContent();
    }

    /// <summary>
    /// Restaura un usuario eliminado.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Una respuesta de estado HTTP.</returns>
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
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Una acción de resultado que contiene el estado de la solicitud y el usuario verificado.</returns>
    [HttpPost("{id}/verify-email")]
    public async Task<IActionResult> VerifyEmail(Guid id)
    {
        try
        {
            var usuarioVerificado = await _usuarioService.VerifyEmailAsync(id); // Verifica el correo electrónico del usuario
            if (usuarioVerificado == null)
            {
                return NotFound(new
                {
                    codigo = 404,
                    mensaje = "Usuario no encontrado.",
                    resultado = (object?)null
                });
            }

            return Ok(new
            {
                codigo = 200,
                mensaje = "Email verificado exitosamente.",
                resultado = new
                {
                    id = usuarioVerificado.id,
                    email = usuarioVerificado.email,
                    email_verified = usuarioVerificado.email_verified,
                    email_verified_at = usuarioVerificado.email_verified_at
                }
            });
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new
            {
                codigo = 400,
                mensaje = "El email ya estaba verificado.",
                resultado = (object?)null
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                codigo = 500,
                mensaje = "Error interno del servidor.",
                resultado = (object?)null
            });
        }
    }

    /// <summary>
    /// Endpoint HTTP PATCH para activar un usuario estableciendo el campo is_active en true.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Una acción de resultado que devuelve el estado del usuario en activo.</returns>
    
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
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Respuesta HTTP con el estado de la desactivación.</returns>
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
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Respuesta HTTP con el estado del cambio.</returns>
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDTO model)
    {
        try
        {
            var result = await _usuarioService.ChangePasswordAsync(id, model);

            if (!result)
            {
                return BadRequest(new
                {
                    codigo = 400,
                    mensaje = "La contraseña actual no es correcta.",
                    resultado = (object?)null
                });
            }

            return Ok(new
            {
                codigo = 200,
                mensaje = "Contraseña cambiada exitosamente.",
                resultado = (object?)null
            });
        }
        catch (InvalidOperationException ex) when (ex.Message == "La contraseña actual no es correcta.")
        {
            return BadRequest(new
            {
                codigo = 400,
                mensaje = ex.Message,
                resultado = (object?)null
            });
        }
        catch (InvalidOperationException ex) when (ex.Message == "La nueva contraseña y la confirmación no coinciden.")
        {
            return BadRequest(new
            {
                codigo = 400,
                mensaje = ex.Message,
                resultado = (object?)null
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new
            {
                codigo = 404,
                mensaje = "Usuario no encontrado",
                resultado = (object?)null
            });
        }
        /*
        catch (Exception)
        {
            return StatusCode(500, new
            {
                codigo = 500,
                mensaje = "Error interno del servidor.",
                resultado = (object?)null
            });
        }
        */
    }

    /// <summary>
    /// Solicita el restablecimiento de contraseña enviando un enlace al correo del usuario.
    /// </summary>
    /// <param name="request">Objeto con el email del usuario.</param>
    /// <returns>Respuesta HTTP con el estado de la solicitud.</returns>
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDTO request)
    {
        if (request == null || string.IsNullOrEmpty(request.Email))
        {
            return BadRequest(new
            {
                codigo = 400,
                mensaje = "El email es obligatorio.",
                resultado = (object?)null
            });
        }

        try
        {
            await _usuarioService.RequestPasswordResetAsync(request.Email);
            return Ok(new
            {
                codigo = 200,
                mensaje = "Se ha enviado un correo con el enlace de restablecimiento.",
                resultado = (object?)null
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new
            {
                codigo = 404,
                mensaje = "No se encontró una cuenta con ese email.",
                resultado = (object?)null
            });
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new
            {
                codigo = 400,
                mensaje = "El email no ha sido verificado.",
                resultado = (object?)null
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                codigo = 500,
                mensaje = "Error al enviar el correo de restablecimiento.",
                resultado = (object?)null
            });
        }
    }


}
