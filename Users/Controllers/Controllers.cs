using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using apitienda.Models;
using apitienda.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using apitienda.DTOs;
using apitienda.Services;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class usersController : ControllerBase
{
    private readonly IUsuarioService _iUsuarioService;
    private readonly IConfiguration _configuration;
    private readonly DataContext _context;
    private readonly JwtService _jwtService;

    public usersController(
        IUsuarioService iUsuarioService,
        IConfiguration configuration,
        DataContext context,
        JwtService jwtService)
    {
        _iUsuarioService = iUsuarioService;
        _configuration = configuration;
        _context = context;
        _jwtService = jwtService;
    }
[AllowAnonymous]
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsersAsync(int? page, int? limit, string? sort, string? order, bool? is_active, bool? is_deleted, bool? is_superuser, bool? email_veridied)
    {
        try
        {
            if (!(page == null))
            {
                if (page < 1)
                {
                    return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
                }
            }

            if (!(limit == null))
            {
                if (limit < 1)
                {
                    return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
                }
            }

            if (!(order == null))
            {
                if (!(order == "asc" || order == "desc"))
                {
                    return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
                }
            }

            if (!(sort == null))
            {
                if (!(sort == "username" || sort == "email" || sort == "date_joined" || sort == "first_name" || sort == "last_name"
                  || sort == "is_active" || sort == "is_deleted" || sort == "is_superuser" || sort == "email_verified" || sort == "date_of_birth"
                  || sort == "nationality" || sort == "occupation" || sort == "gender" || sort == "role"))
                {
                    return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
                }
            }

            var usuarios = await _iUsuarioService.GetAllAsync2(page, limit, sort, order, is_active, is_deleted, is_superuser, email_veridied);
            return usuarios;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var usuario = await _iUsuarioService.GetByIdAsync(id);
            return usuario;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
[AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UsuarioCreateDTO usuarioCreateDTO)
    {
        try
        {
            if (usuarioCreateDTO == null)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controllerPostUser")));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controllerPostUser")));
            }

            var email = usuarioCreateDTO.email;
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!emailRegex.IsMatch(email))
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controllerPostUser")));
            }
            var usuario = await _iUsuarioService.AddAsync(usuarioCreateDTO);
            return usuario;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UsuarioUpdate usuarioUpdate)
    {
        try
        {
            if (usuarioUpdate == null)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var updatedUser = await _iUsuarioService.UpdateAsync(id, usuarioUpdate);
            return updatedUser;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var existingUser = await _iUsuarioService.DeleteAsync(id);
            return existingUser;

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] UsuarioPatchDTO usuarioDTO)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            if (usuarioDTO == null)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var existingUser = await _iUsuarioService.UpdatePartialAsync(id, usuarioDTO);
            return existingUser;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
    [HttpPatch("{id}/restore")]
    public async Task<IActionResult> RestoreUserAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var usuarioRestaurado = await _iUsuarioService.RestoreUserAsync(id);
            return usuarioRestaurado;

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500:" + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
    [HttpPost("{id}/verify-email")]
    public async Task<IActionResult> VerifyEmail(Guid id)
    {
        try
        {
            var usuarioVerificado = await _iUsuarioService.VerifyEmailAsync(id);
            return usuarioVerificado;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var activarUsuario = await _iUsuarioService.ActiveteUserAsync(id);
            return activarUsuario;

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var usuario = await _iUsuarioService.DeactivateUserAsync(id);
            return usuario;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDTO model)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var result = await _iUsuarioService.ChangePasswordAsync(id, model);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

[AllowAnonymous]
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDTO request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var respuesta = await _iUsuarioService.RequestPasswordResetAsync(request.Email);
            return respuesta;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

[AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new ApiResponse<string>(400, MessageService.Instance.GetMessage("controller400")));
            }

            var respuesta = await _iUsuarioService.ResetPasswordAsync(request.Token, request.NewPassword);
            return respuesta;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500, MessageService.Instance.GetMessage("controllerUser500")));
        }
    }

    /// <summary>
    /// Login: valida usuario y contraseña, devuelve access y refresh token.
    /// </summary>
[AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var validateResult = await _iUsuarioService.ValidateUserAsync(model.Email, model.Password);

        if (validateResult is UnauthorizedObjectResult)
        {
            return validateResult;  // Retorna 401 con mensaje
        }

        // Extraer usuario del resultado OkObjectResult
        var okResult = validateResult as OkObjectResult;
        var user = okResult.Value as Usuario;

        var tokens = _jwtService.GenerateTokens(user);

        _context.TokenBlacklist.Add(new TokenBlacklist
        {
            Id = Guid.NewGuid(),
            Jti = tokens.RefreshTokenJti,
            UsuarioId = user.id,
            ExpiresAt = tokens.RefreshTokenExpiry,
            RevokedAt = null,
            Reason = "issued"
        });
        await _context.SaveChangesAsync();

        return Ok(new
        {
            access_token = tokens.AccessToken,
            refresh_token = tokens.RefreshToken,
            access_token_expires_in_minutes = tokens.AccessTokenExpiryMinutes,
            refresh_token_expires_in_days = Math.Ceiling((tokens.RefreshTokenExpiry - DateTime.UtcNow).TotalDays)
        });
    }


    /// <summary>
    /// Renueva access token usando refresh token.
    /// </summary>
[AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest model)
    {
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(model.RefreshToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(userIdStr))
                return Unauthorized("Token inválido");

            Guid userId = Guid.Parse(userIdStr);

            var storedToken = await _context.TokenBlacklist
                .FirstOrDefaultAsync(t => t.Jti == jti && t.UsuarioId == userId);

            if (storedToken == null || storedToken.RevokedAt != null || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized("Refresh token inválido o revocado.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized();

            var tokens = _jwtService.GenerateTokens(user);

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.Reason = "refreshed";
            _context.TokenBlacklist.Update(storedToken);

            _context.TokenBlacklist.Add(new TokenBlacklist
            {
                Id = Guid.NewGuid(),
                Jti = tokens.RefreshTokenJti,
                UsuarioId = user.id,
                ExpiresAt = tokens.RefreshTokenExpiry,
                RevokedAt = null,
                Reason = "issued"
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                access_token = tokens.AccessToken,
                refresh_token = tokens.RefreshToken,
                access_token_expires_in_minutes = tokens.AccessTokenExpiryMinutes,
                refresh_token_expires_in_days = Math.Ceiling((tokens.RefreshTokenExpiry - DateTime.UtcNow).TotalDays)
            });
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized("Refresh token expirado.");
        }
        catch (Exception)
        {
            return Unauthorized("Token inválido.");
        }
    }

    /// <summary>
    /// Logout: revoca refresh token (elimina o marca como revocado).
    /// </summary>
[AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest model)
    {
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(model.RefreshToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(userIdStr))
                return Unauthorized("Token inválido");

            Guid userId = Guid.Parse(userIdStr);

            var storedToken = await _context.TokenBlacklist
                .FirstOrDefaultAsync(t => t.Jti == jti && t.UsuarioId == userId);

            if (storedToken == null || storedToken.RevokedAt != null)
            {
                return BadRequest("Refresh token ya revocado o inválido.");
            }

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.Reason = "logout";
            _context.TokenBlacklist.Update(storedToken);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Logout exitoso, refresh token revocado." });
        }
        catch (Exception)
        {
            return Unauthorized("Token inválido.");
        }
    }
}
