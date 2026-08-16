using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlataformaEducacional.Auth.Api.Models;
using PlataformaEducacional.Auth.Api.Security;
using PlataformaEducacional.Core.Enumerators;
using PlataformaEducacional.Core.Extensions;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.MessageBus;
using PlataformaEducacional.WebApi.Core.Controllers;
using PlataformaEducacional.WebApi.Core.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PlataformaEducacional.Auth.Api.Controllers;

[Route("api/identidade")]
public class AuthController : MainController
{
    private readonly ILogger<AuthController> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtSettings _appSettings;
    private readonly IJwtRsaSigningCredentialsProvider _jwtSigning;

    private readonly IMessageBus _bus;

    public AuthController(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          IOptions<JwtSettings> appSettings,
                          IJwtRsaSigningCredentialsProvider jwtSigning,
                          IMessageBus bus,
                          ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _appSettings = appSettings.Value;
        _jwtSigning = jwtSigning;
        _bus = bus;
        _logger = logger;
    }

    [HttpPost("nova-conta")]
    public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        _logger.LogInformation("Solicitação de registro para Email {Email}", usuarioRegistro.Email);

        var user = new IdentityUser
        {
            UserName = usuarioRegistro.Email,
            Email = usuarioRegistro.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuário criado com sucesso - Email {Email}, UserId {UserId}", usuarioRegistro.Email, user.Id);

            await _userManager.AddToRoleAsync(user, TipoUsuario.Aluno.GetDescription().ToUpperInvariant());

            var clienteResult = await RegistrarAluno(usuarioRegistro);

            if (!clienteResult.ValidationResult.IsValid)
            {
                _logger.LogWarning("Falha ao registrar aluno na fila - Email {Email}: {Errors}", usuarioRegistro.Email,
                    string.Join(", ", clienteResult.ValidationResult.Errors.Select(e => e.ErrorMessage)));
                await _userManager.DeleteAsync(user);
                return CustomResponse(clienteResult.ValidationResult);
            }

            _logger.LogInformation("Registro completo com sucesso - Email {Email}, JWT gerado", usuarioRegistro.Email);
            return CustomResponse(await GerarJwt(usuarioRegistro.Email));
        }

        _logger.LogWarning("Falha ao criar usuário - Email {Email}: {Errors}", usuarioRegistro.Email,
            string.Join(", ", result.Errors.Select(e => e.Description)));

        foreach (var error in result.Errors)
        {
            AdicionarErroProcessamento(error.Description);
        }

        return CustomResponse();
    }

    [HttpPost("autenticar")]
    public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        _logger.LogInformation("Tentativa de login para Email {Email}", usuarioLogin.Email);

        var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

        if (result.Succeeded)
        {
            _logger.LogInformation("Login bem-sucedido para Email {Email}, JWT gerado", usuarioLogin.Email);
            return CustomResponse(await GerarJwt(usuarioLogin.Email));
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("Usuário bloqueado por tentativas inválidas - Email {Email}", usuarioLogin.Email);
            AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas");
            return CustomResponse();
        }

        _logger.LogWarning("Falha de autenticação - Email ou Senha incorretos para {Email}", usuarioLogin.Email);
        AdicionarErroProcessamento("Usuário ou Senha incorretos");
        return CustomResponse();
    }

    private async Task<UsuarioRespostaLogin> GerarJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new ApplicationException("Usuário não encontrado.");
        }

        var claims = await _userManager.GetClaimsAsync(user);

        var identityClaims = await ObterClaimsUsuario(claims, user);
        var encodedToken = CodificarToken(identityClaims);

        return ObterRespostaToken(encodedToken, user, claims);
    }

    private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser user)
    {
        if (user == null || string.IsNullOrEmpty(user.Email))
        {
            throw new ApplicationException("Usuário não encontrado.");
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim("role", userRole));
        }

        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(claims);

        return identityClaims;
    }

    private string CodificarToken(ClaimsIdentity identityClaims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _appSettings.Emissor,
            Audience = _appSettings.ValidoEm,
            Subject = identityClaims,
            Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
            SigningCredentials = _jwtSigning.SigningCredentials
        });

        return tokenHandler.WriteToken(token);
    }

    private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
    {
        return new UsuarioRespostaLogin
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
            UsuarioToken = new UsuarioToken
            {
                Id = user.Id,
                Email = user.Email,
                Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
            }
        };
    }

    private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    private async Task<ResponseMessage> RegistrarAluno(UsuarioRegistro usuarioRegistro)
    {
        var usuario = await _userManager.FindByEmailAsync(usuarioRegistro.Email);

        var usuarioRegistrado = new UsuarioRegistradoIntegrationEvent(
            Guid.Parse(usuario.Id), usuarioRegistro.Nome, usuarioRegistro.Email, usuarioRegistro.Cpf);

        try
        {
            return _bus.Request<UsuarioRegistradoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro ao tentar enviar para fila, verifique se o RabbitMQ esta acessível\r\n{Message}", ex.GetFullMessage());
            await _userManager.DeleteAsync(usuario);
            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("RabbitMQ", $"Ocorreu um erro ao tentar enviar para fila, verifique se o RabbitMQ esta acessível\r\n{ex.GetFullMessage()}"));
            return new ResponseMessage(validationResult);
        }
    }
}
