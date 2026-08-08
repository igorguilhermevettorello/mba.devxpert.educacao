using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PlataformaEducacional.Conteudo.Api.Controllers;
using PlataformaEducacional.Conteudo.Api.DTOs.Cursos;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Notifications;
using PlataformaEducacional.WebApi.Core.Enumerators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PlataformaEducacional.Conteudo.Api.Tests
{
    public class CursosControllerTests
    {
        private readonly Mock<IMediatorHandler> _mediator = new();
        private readonly Mock<ICursoRepository> _repo = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<INotificador> _notificador = new();

        private CursosController CreateController()
        {
            var loggerMock = new Mock<ILogger<CursosController>>();
            return new CursosController(_mediator.Object, _mapper.Object, _repo.Object, _notificador.Object, loggerMock.Object);
        }

        [Fact]
        public async Task Criar_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = CreateController();
            controller.ModelState.AddModelError("Titulo", "required");

            var dto = new CriarCursoDto();
            var result = await controller.Criar(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Criar_ReturnsCreated_WhenMediatorSucceeded()
        {
            var controller = CreateController();
            var dto = new CriarCursoDto
            {
                Titulo = "T",
                Descricao = "Descricao longa o suficiente",
                Instrutor = "I",
                Nivel = NivelCurso.Basico,
                Valor = 10m
            };

            _mediator.Setup(m => m.SendCommand(It.IsAny<CriarAulaCommand>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await controller.Criar(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CursosController.ObterPorId), created.ActionName);
        }

        [Fact]
        public async Task Criar_ReturnsBadRequest_WhenMediatorReturnsErrors()
        {
            var controller = CreateController();
            var dto = new CriarCursoDto
            {
                Titulo = "T",
                Descricao = "Descricao longa o suficiente",
                Instrutor = "I",
                Nivel = NivelCurso.Basico,
                Valor = 10m
            };

            var validation = new ValidationResult();
            validation.Errors.Add(new FluentValidation.Results.ValidationFailure("Prop", "err"));

            _mediator.Setup(m => m.SendCommand(It.IsAny<CriarAulaCommand>()))
                .ReturnsAsync(validation);


            var result = await controller.Criar(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ObterPorId_ReturnsNotFound_WhenNotExists()
        {
            var controller = CreateController();
            _repo.Setup(r => r.BuscarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Curso?)null);

            var result = await controller.ObterPorId(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ObterPorId_ReturnsOk_WhenExists()
        {
            var controller = CreateController();
            var curso = new Curso("T", "D", "I", NivelCurso.Basico, 10m);
            _repo.Setup(r => r.BuscarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(curso);

            var cursoDto = new CursoDto { Titulo = "T" };
            _mapper.Setup(m => m.Map<CursoDto>(curso)).Returns(cursoDto);

            var result = await controller.ObterPorId(Guid.NewGuid());

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }
    }
}
