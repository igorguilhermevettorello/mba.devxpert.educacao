using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PlataformaEducacional.Auth.Api.Controllers;
using PlataformaEducacional.Auth.Api.Models;
using PlataformaEducacional.Auth.Api.Security;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.MessageBus;
using Xunit;

namespace PlataformaEducacional.Auth.Api.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
        private readonly Mock<IJwtRsaSigningCredentialsProvider> _jwtSigningMock;
        private readonly Mock<IMessageBus> _busMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly IOptions<WebApi.Core.Identity.JwtSettings> _jwtOptions;

        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStore.Object,
                null, null, null, null, null, null, null, null);

            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            var httpAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();

            _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
                _userManagerMock.Object,
                httpAccessor.Object,
                userPrincipalFactory.Object,
                null, null, null, null);

            _jwtSigningMock = new Mock<IJwtRsaSigningCredentialsProvider>();
            var key = Encoding.UTF8.GetBytes("test-key-should-be-long-enough-and-extra-1234567890");
            _jwtSigningMock.SetupGet(x => x.SigningCredentials)
                .Returns(new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            _jwtSigningMock.SetupGet(x => x.JwksJson).Returns("{\"keys\":[]}");

            _busMock = new Mock<IMessageBus>();
            _loggerMock = new Mock<ILogger<AuthController>>();

            var jwtSettings = new WebApi.Core.Identity.JwtSettings
            {
                Emissor = "https://issuer.example.com/",
                ValidoEm = "https://audience.example.com",
                ExpiracaoHoras = 2
            };
            _jwtOptions = Options.Create(jwtSettings);

            _controller = new AuthController(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _jwtOptions,
                _jwtSigningMock.Object,
                _busMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Registrar_ReturnsBadRequest_WhenModelStateInvalid()
        {
            _controller.ModelState.AddModelError("Email", "required");
            var result = await _controller.Registrar(new UsuarioRegistro());

            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Registrar_ReturnsToken_OnSuccess()
        {
            var registro = new UsuarioRegistro
            {
                Nome = "Teste",
                Email = "teste@example.com",
                Senha = "senha123",
                Cpf = "00000000000"
            };

            var createdUser = new IdentityUser { Id = Guid.NewGuid().ToString(), Email = registro.Email };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), registro.Senha))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.FindByEmailAsync(registro.Email))
                .ReturnsAsync(createdUser);

            _userManagerMock.Setup(u => u.GetClaimsAsync(createdUser))
                .ReturnsAsync(new List<Claim>());

            _userManagerMock.Setup(u => u.GetRolesAsync(createdUser))
                .ReturnsAsync(new List<string> { "ALUNO" });

            // MessageBus Request returns successful ResponseMessage (no validation errors)
            _busMock.Setup(b => b.Request<UsuarioRegistradoIntegrationEvent, ResponseMessage>(It.IsAny<UsuarioRegistradoIntegrationEvent>()))
                .Returns(new ResponseMessage(new ValidationResult()));

            var result = await _controller.Registrar(registro);

            var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
            Assert.NotNull(ok.Value);

            // response should be UsuarioRespostaLogin
            Assert.IsType<UsuarioRespostaLogin>(ok.Value);
            var token = (UsuarioRespostaLogin)ok.Value;
            Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
            Assert.True(token.ExpiresIn > 0);
            Assert.Equal(createdUser.Email, token.UsuarioToken?.Email);
        }

        [Fact]
        public async Task Registrar_DeletesUser_AndReturnsErrors_WhenIntegrationReturnsInvalid()
        {
            var registro = new UsuarioRegistro
            {
                Nome = "Teste",
                Email = "teste2@example.com",
                Senha = "senha123",
                Cpf = "00000000000"
            };

            var createdUser = new IdentityUser { Id = Guid.NewGuid().ToString(), Email = registro.Email };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), registro.Senha))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.FindByEmailAsync(registro.Email))
                .ReturnsAsync(createdUser);

            // Simulate integration error
            var validation = new ValidationResult();
            validation.Errors.Add(new FluentValidation.Results.ValidationFailure("RabbitMQ", "Error"));
            _busMock.Setup(b => b.Request<UsuarioRegistradoIntegrationEvent, ResponseMessage>(It.IsAny<UsuarioRegistradoIntegrationEvent>()))
                .Returns(new ResponseMessage(validation));

            _userManagerMock.Setup(u => u.DeleteAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            var result = await _controller.Registrar(registro);

            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
            _userManagerMock.Verify(u => u.DeleteAsync(It.Is<IdentityUser>(x => x.Email == registro.Email)), Times.Once);
        }

        [Fact]
        public async Task Registrar_ReturnsErrors_WhenCreateFails()
        {
            var registro = new UsuarioRegistro
            {
                Nome = "Teste",
                Email = "fail@example.com",
                Senha = "senha123",
                Cpf = "00000000000"
            };

            var failed = IdentityResult.Failed(new IdentityError { Description = "Weak password" });
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), registro.Senha))
                .ReturnsAsync(failed);

            var result = await _controller.Registrar(registro);

            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenModelStateInvalid()
        {
            _controller.ModelState.AddModelError("Email", "required");
            var result = await _controller.Login(new UsuarioLogin());

            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsToken_WhenCredentialsValid()
        {
            var login = new UsuarioLogin { Email = "login@example.com", Senha = "senha" };

            _signInManagerMock.Setup(s => s.PasswordSignInAsync(login.Email, login.Senha, false, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Setup user for token generation
            var user = new IdentityUser { Id = Guid.NewGuid().ToString(), Email = login.Email };
            _userManagerMock.Setup(u => u.FindByEmailAsync(login.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string>());

            var result = await _controller.Login(login);

            var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
            Assert.IsType<UsuarioRespostaLogin>(ok.Value);
        }

        [Fact]
        public async Task Login_ReturnsLockedOut_WhenSignInLockedOut()
        {
            var login = new UsuarioLogin { Email = "locked@example.com", Senha = "senha" };

            _signInManagerMock.Setup(s => s.PasswordSignInAsync(login.Email, login.Senha, false, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            var result = await _controller.Login(login);

            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_OnInvalidCredentials()
        {
            var login = new UsuarioLogin { Email = "bad@example.com", Senha = "senha" };

            _signInManagerMock.Setup(s => s.PasswordSignInAsync(login.Email, login.Senha, false, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _controller.Login(login);

            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GerarJwt_Throws_WhenUserNotFound()
        {
            var method = typeof(AuthController).GetMethod("GerarJwt", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(method);

            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser?)null);

            var task = (Task)method.Invoke(_controller, new object[] { "noone@example.com" })!;
            await Assert.ThrowsAsync<ApplicationException>(async () => await task);
        }

        [Fact]
        public async Task ObterClaimsUsuario_ProducesExpectedClaims()
        {
            var privateMethod = typeof(AuthController).GetMethod("ObterClaimsUsuario", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(privateMethod);

            var user = new IdentityUser { Id = Guid.NewGuid().ToString(), Email = "u@example.com" };
            var claims = new List<Claim>();

            _userManagerMock.Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "ALUNO", "ADMIN" });

            var task = (Task)privateMethod.Invoke(_controller, new object[] { claims, user })!;
            await task;
            var resultProp = task.GetType().GetProperty("Result")!;
            var identity = (ClaimsIdentity)resultProp.GetValue(task)!;

            Assert.NotNull(identity);
            var types = identity.Claims.Select(c => c.Type).ToList();
            Assert.Contains(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, types);
            Assert.Contains(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, types);
            Assert.Contains("role", types);
        }

        [Fact]
        public void ToUnixEpochDate_ReturnsLong()
        {
            var method = typeof(AuthController).GetMethod("ToUnixEpochDate", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            var date = DateTime.UtcNow;
            var value = (long)method.Invoke(null, new object[] { date })!;
            Assert.True(value > 0);
        }

        [Fact]
        public async Task CodificarToken_ReturnsJwtString()
        {
            // create identity claims via private method
            var user = new IdentityUser { Id = Guid.NewGuid().ToString(), Email = "c@example.com" };
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string>());

            var obterClaims = typeof(AuthController).GetMethod("ObterClaimsUsuario", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var taskClaims = (Task)obterClaims.Invoke(_controller, new object[] { new List<Claim>(), user })!;
            await taskClaims;
            var identity = (ClaimsIdentity)taskClaims.GetType().GetProperty("Result")!.GetValue(taskClaims)!;

            var codificar = typeof(AuthController).GetMethod("CodificarToken", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var token = (string)codificar.Invoke(_controller, new object[] { identity })!;

            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Contains('.', token);
        }

        [Fact]
        public void ObterRespostaToken_ReturnsExpectedStructure()
        {
            var method = typeof(AuthController).GetMethod("ObterRespostaToken", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(method);

            var user = new IdentityUser { Id = Guid.NewGuid().ToString(), Email = "resp@example.com" };
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "resp") };

            var encoded = "header.payload.signature";
            var result = (UsuarioRespostaLogin)method.Invoke(_controller, new object[] { encoded, user, claims })!;

            Assert.Equal(encoded, result.AccessToken);
            Assert.True(result.ExpiresIn > 0);
            Assert.Equal(user.Email, result.UsuarioToken?.Email);
            Assert.Contains(result.UsuarioToken!.Claims, c => c.Type == ClaimTypes.Name);
        }

        [Fact]
        public async Task RegistrarAluno_CatchesBusException_DeletesUser_AndReturnsValidationError()
        {
            // call private RegistrarAluno and simulate _bus.Request throwing
            var registro = new UsuarioRegistro { Email = "rbus@example.com", Nome = "Nome", Cpf = "000", Senha = "senha123" };
            var user = new IdentityUser { Id = Guid.NewGuid().ToString(), Email = registro.Email };

            _userManagerMock.Setup(u => u.FindByEmailAsync(registro.Email))
                .ReturnsAsync(user);

            _busMock.Setup(b => b.Request<UsuarioRegistradoIntegrationEvent, ResponseMessage>(It.IsAny<UsuarioRegistradoIntegrationEvent>()))
                .Throws(new Exception("bus down"));

            _userManagerMock.Setup(u => u.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();

            var method = typeof(AuthController).GetMethod("RegistrarAluno", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var task = (Task)method.Invoke(_controller, new object[] { registro })!;
            await task;
            var response = (ResponseMessage)task.GetType().GetProperty("Result")!.GetValue(task)!;

            Assert.False(response.ValidationResult.IsValid);
            _userManagerMock.Verify(u => u.DeleteAsync(It.Is<IdentityUser>(x => x.Email == registro.Email)), Times.Once);
        }
    }
}