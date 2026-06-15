using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlataformaEducacional.Alunos.Api.Controllers;
using PlataformaEducacional.Alunos.Api.DTOs.Matriculas;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.Alunos.Domain.Models;
using PlataformaEducacional.WebApi.Core.User;
using Xunit;

namespace PlataformaEducacional.Alunos.Tests
{
    public class AlunosControllerTests
    {
        private readonly Mock<IAlunoRepository> _repo = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IAspNetUser> _user = new();

        private AlunosController CreateController() => new AlunosController(_repo.Object, _mediator.Object, _user.Object);

        [Fact]
        public async Task RealizarMatricula_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = CreateController();
            controller.ModelState.AddModelError("CursoId", "required");

            var result = await controller.RealizarMatricula(Guid.NewGuid(), new MatricularAlunoDTO());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RealizarMatricula_ReturnsNotFound_WhenAlunoNull()
        {
            _repo.Setup(r => r.ObterPorId(It.IsAny<Guid>())).ReturnsAsync((Aluno?)null);
            var controller = CreateController();

            var result = await controller.RealizarMatricula(Guid.NewGuid(), new MatricularAlunoDTO { CursoId = Guid.NewGuid() });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RealizarMatricula_ReturnsForbid_WhenUserCannotView()
        {
            var alunoId = Guid.NewGuid();
            var aluno = new Aluno(alunoId, "N", "e@e.com", "12345678901");
            _repo.Setup(r => r.ObterPorId(alunoId)).ReturnsAsync(aluno);

            _user.Setup(u => u.PossuiRole("Administrador")).Returns(false);
            _user.Setup(u => u.ObterUserId()).Returns(Guid.NewGuid()); // different user

            var controller = CreateController();

            var result = await controller.RealizarMatricula(alunoId, new MatricularAlunoDTO { CursoId = Guid.NewGuid() });

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task RealizarMatricula_Created_WhenMediatorSucceeds()
        {
            var alunoId = Guid.NewGuid();
            var aluno = new Aluno(alunoId, "N", "e@e.com", "12345678901");
            _repo.Setup(r => r.ObterPorId(alunoId)).ReturnsAsync(aluno);

            _user.Setup(u => u.PossuiRole("Administrador")).Returns(false);
            _user.Setup(u => u.ObterUserId()).Returns(alunoId);

            _mediator.Setup(m => m.Send(It.IsAny<IRequest<ValidationResult>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var controller = CreateController();

            var result = await controller.RealizarMatricula(alunoId, new MatricularAlunoDTO { CursoId = Guid.NewGuid() });

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(AlunosController.ObterMatriculaPorId), created.ActionName);
        }

        [Fact]
        public async Task ObterEnderecoPorAlunoId_ReturnsNotFound_WhenNoAluno()
        {
            _repo.Setup(r => r.ObterPorId(It.IsAny<Guid>())).ReturnsAsync((Aluno?)null);
            var controller = CreateController();

            var result = await controller.ObterEnderecoPorAlunoId(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ObterEnderecoPorAlunoId_ReturnsForbid_WhenCannotView()
        {
            var alunoId = Guid.NewGuid();
            var aluno = new Aluno(alunoId, "N", "e@e.com", "12345678901");
            _repo.Setup(r => r.ObterPorId(alunoId)).ReturnsAsync(aluno);

            _user.Setup(u => u.PossuiRole("Administrador")).Returns(false);
            _user.Setup(u => u.ObterUserId()).Returns(Guid.NewGuid());

            var controller = CreateController();

            var result = await controller.ObterEnderecoPorAlunoId(alunoId);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task ObterEnderecoPorAlunoId_ReturnsNotFound_WhenNoEndereco()
        {
            var alunoId = Guid.NewGuid();
            var aluno = new Aluno(alunoId, "N", "e@e.com", "12345678901");
            _repo.Setup(r => r.ObterPorId(alunoId)).ReturnsAsync(aluno);
            _user.Setup(u => u.PossuiRole("Administrador")).Returns(true);

            _repo.Setup(r => r.ObterEnderecoPorAlunoId(alunoId)).ReturnsAsync((Endereco?)null);

            var controller = CreateController();

            var result = await controller.ObterEnderecoPorAlunoId(alunoId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}