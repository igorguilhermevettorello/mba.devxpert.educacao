using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlataformaEducacional.Conteudo.Api.Controllers;
using PlataformaEducacional.Conteudo.Api.DTOs.Aulas;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PlataformaEducacional.Conteudo.Api.Tests
{
    public class AulasControllerTests
    {
        private readonly Mock<IMediatorHandler> _mediator = new();
        private readonly Mock<IAulaRepository> _repo = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<INotificador> _notificador = new();

        private AulasController CreateController() =>
            new AulasController(_mediator.Object, _mapper.Object, _repo.Object, _notificador.Object);

        [Fact]
        public async Task Criar_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = CreateController();
            controller.ModelState.AddModelError("Titulo", "required");
            var dto = new CriarAulaDto();
            var result = await controller.Criar(dto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Criar_ReturnsCreated_WhenMediatorSucceeded()
        {
            var controller = CreateController();
            var dto = new CriarAulaDto
            {
                CursoId = Guid.NewGuid(),
                Titulo = "T",
                Descricao = "Descricao longa o suficiente",
                DuracaoMinutos = 30,
                Ordem = 1
            };

            _mediator.Setup(m => m.SendCommand(It.IsAny<CriarAulaCommand>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await controller.Criar(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(AulasController.ObterPorId), created.ActionName);
        }

        [Fact]
        public async Task ObterPorId_ReturnsNotFound_WhenNotExists()
        {
            var controller = CreateController();
            _repo.Setup(r => r.BuscarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Aula?)null);

            var result = await controller.ObterPorId(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ObterPorId_ReturnsOk_WhenExists()
        {
            var controller = CreateController();
            var aula = new Aula("T", "D", 30, 1);
            _repo.Setup(r => r.BuscarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(aula);

            var aulaDto = new AulaDto { Titulo = "T" };
            _mapper.Setup(m => m.Map<AulaDto>(aula)).Returns(aulaDto);

            var result = await controller.ObterPorId(Guid.NewGuid());

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }
    }
}