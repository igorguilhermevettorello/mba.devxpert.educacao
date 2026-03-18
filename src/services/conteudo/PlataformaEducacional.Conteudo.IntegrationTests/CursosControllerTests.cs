using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PlataformaEducacional.Conteudo.Api.DTOs;
using PlataformaEducacional.Conteudo.Api.DTOs.ConteudoProgramatico;
using PlataformaEducacional.Conteudo.Api.DTOs.Cursos;
using PlataformaEducacional.Conteudo.Domain.Enums;
using PlataformaEducacional.Conteudo.IntegrationTests.Factories;
using System.Net;
using System.Net.Http.Json;

namespace PlataformaEducacional.Conteudo.IntegrationTests
{
    public class CursosControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        readonly HttpClient _httpClient;

        public CursosControllerTests()
        {
            var factory = new CustomWebApplicationFactory();
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task ObterTodosOsCursosDeveRetornarSucesso()
        {
            //act
            var response = await _httpClient.GetAsync("/api/cursos");

            //assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task AdminCadastraCursoComSucesso()
        {
            #region cadastro de curso
            var conteudoProgramaticoDto = new CriarConteudoProgramaticoDto()
            {
                Bibliografia = "Bibliografia Xpto",
                Ementa = "Ementa Xpto",
                MaterialUrl = "https://www.curso-motores.com.br/material",
                Objetivo = "Objetivo Xpto",
            };

            var cursoDto = new CriarCursoDto()
            {
                Titulo = "Fabricação de motores trifásicos",
                Descricao = "Curso avançado de fabricação de motores trifásicos",
                Instrutor = "José Mecanico",
                Nivel = NivelCurso.Avancado,
                ConteudoProgramatico = conteudoProgramaticoDto,
                Valor = 2500
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/cursos", cursoDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var resultDto = await postResponse.Content.ReadFromJsonAsync<ResultDto<Guid>>();
            resultDto?.Success.Should().BeTrue();
            #endregion
        }

        [Fact]
        public async Task AdminEditaCursoComSucesso()
        {
            #region cadastro de curso
            var conteudoProgramaticoDto = new CriarConteudoProgramaticoDto()
            {
                Bibliografia = "Bibliografia Xpto",
                Ementa = "Ementa Xpto",
                MaterialUrl = "https://www.curso-motores.com.br/material",
                Objetivo = "Objetivo Xpto",
            };

            var cursoDto = new CriarCursoDto()
            {
                Titulo = "Fabricação de motores trifásicos",
                Descricao = "Curso avançado de fabricação de motores trifásicos",
                Instrutor = "José Mecanico",
                Nivel = NivelCurso.Avancado,
                ConteudoProgramatico = conteudoProgramaticoDto,
                Valor = 2500
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/cursos", cursoDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var resultadocriacao = await postResponse.Content.ReadFromJsonAsync<ResultDto<Guid>>();
            resultadocriacao?.Success.Should().BeTrue();
            #endregion

            #region edição de curso
            var cursoId = resultadocriacao?.Data;
            var novoConteudoProgramaticoDto = new CriarConteudoProgramaticoDto()
            {
                Bibliografia = "Bibliografia editada",
                Ementa = "Ementa editada",
                MaterialUrl = "https://www.curso-motores.com.br/material-editado",
                Objetivo = "Objetivo editado",
            };

            var atualizarCursoDto = new AtualizarCursoDto()
            {
                Titulo = "Fabricação de motores monofásicos",
                Descricao = "Curso de fabricação de motores monofásicos",
                Nivel = NivelCurso.Intermediario,
                ConteudoProgramatico = novoConteudoProgramaticoDto,
            };

            var putResponse = await _httpClient.PutAsJsonAsync($"/api/cursos/{cursoId}", atualizarCursoDto);
            putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultadoAtualizacao = await postResponse.Content.ReadFromJsonAsync<ResultDto>();
            resultadoAtualizacao?.Success.Should().BeTrue();
            #endregion

            #region consulta do curso editado
            var cursoEditado = await _httpClient.GetFromJsonAsync<CursoDto>($"/api/cursos/{cursoId}");

            cursoEditado.Should().NotBeNull();
            cursoEditado.Titulo.Should().Be(atualizarCursoDto.Titulo);
            cursoEditado.Descricao.Should().Be(atualizarCursoDto.Descricao);
            cursoEditado.Nivel.Should().Be(atualizarCursoDto.Nivel);

            cursoEditado.ConteudoProgramatico.Should().NotBeNull();
            cursoEditado.ConteudoProgramatico.Ementa.Should().Be(atualizarCursoDto.ConteudoProgramatico.Ementa);
            cursoEditado.ConteudoProgramatico.Objetivo.Should().Be(atualizarCursoDto.ConteudoProgramatico.Objetivo);
            cursoEditado.ConteudoProgramatico.Bibliografia.Should().Be(atualizarCursoDto.ConteudoProgramatico.Bibliografia);
            cursoEditado.ConteudoProgramatico.MaterialUrl.Should().Be(atualizarCursoDto.ConteudoProgramatico.MaterialUrl);
            #endregion
        }

        [Fact]
        public async Task ConsultaCursoPorIdDeveRetornarCurso()
        {
            #region cadastro de curso
            var conteudoProgramaticoDto = new CriarConteudoProgramaticoDto()
            {
                Bibliografia = "Bibliografia Xpto",
                Ementa = "Ementa Xpto",
                MaterialUrl = "https://www.curso-motores.com.br/material",
                Objetivo = "Objetivo Xpto",
            };

            var cursoDto = new CriarCursoDto()
            {
                Titulo = "Fabricação de motores trifásicos",
                Descricao = "Curso avançado de fabricação de motores trifásicos",
                Instrutor = "José Mecanico",
                Nivel = NivelCurso.Avancado,
                ConteudoProgramatico = conteudoProgramaticoDto,
                Valor = 2500
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/cursos", cursoDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var resultadocriacao = await postResponse.Content.ReadFromJsonAsync<ResultDto<Guid>>();
            resultadocriacao?.Success.Should().BeTrue();
            #endregion                      

            #region consulta do curso
            var cursoId = resultadocriacao?.Data;
            var cursoConsultado = await _httpClient.GetFromJsonAsync<CursoDto>($"/api/cursos/{cursoId}");
            cursoConsultado.Should().NotBeNull();
            #endregion
        }

        [Fact]
        public async Task ObterCursosAtivosDeveRetornarApenasCursosAtivos()
        {
            #region cadastro de curso
            var conteudoProgramaticoDto = new CriarConteudoProgramaticoDto()
            {
                Bibliografia = "Bibliografia Xpto",
                Ementa = "Ementa Xpto",
                MaterialUrl = "https://www.curso-motores.com.br/material",
                Objetivo = "Objetivo Xpto",
            };

            var cursoDto = new CriarCursoDto()
            {
                Titulo = "Fabricação de motores trifásicos",
                Descricao = "Curso avançado de fabricação de motores trifásicos",
                Instrutor = "José Mecanico",
                Nivel = NivelCurso.Avancado,
                ConteudoProgramatico = conteudoProgramaticoDto,
                Valor = 2500
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/cursos", cursoDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var resultDto = await postResponse.Content.ReadFromJsonAsync<ResultDto<Guid>>();
            resultDto?.Success.Should().BeTrue();
            #endregion

            #region consulta de cursos ativos
            var responseConsulta = await _httpClient.GetAsync("/api/cursos/ativos");
            var cursos = await responseConsulta.Content.ReadFromJsonAsync<IEnumerable<CursoDto>>();

            responseConsulta.StatusCode.Should().Be(HttpStatusCode.OK);
            cursos.Should().NotBeEmpty();
            cursos.Where(x => x.Ativo == false).Should().BeEmpty();
            #endregion
        }

        [Fact]
        public async Task ObterCursosDeveRetornarCursos()
        {
            #region cadastro de curso
            var conteudoProgramaticoDto = new CriarConteudoProgramaticoDto()
            {
                Bibliografia = "Bibliografia Xpto",
                Ementa = "Ementa Xpto",
                MaterialUrl = "https://www.curso-motores.com.br/material",
                Objetivo = "Objetivo Xpto",
            };

            var cursoDto = new CriarCursoDto()
            {
                Titulo = "Fabricação de motores trifásicos",
                Descricao = "Curso avançado de fabricação de motores trifásicos",
                Instrutor = "José Mecanico",
                Nivel = NivelCurso.Avancado,
                ConteudoProgramatico = conteudoProgramaticoDto,
                Valor = 2500
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/cursos", cursoDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var resultDto = await postResponse.Content.ReadFromJsonAsync<ResultDto<Guid>>();
            resultDto?.Success.Should().BeTrue();
            #endregion

            #region consulta de cursos
            var responseConsulta = await _httpClient.GetAsync("/api/cursos");
            var cursos = await responseConsulta.Content.ReadFromJsonAsync<IEnumerable<CursoDto>>();

            responseConsulta.StatusCode.Should().Be(HttpStatusCode.OK);
            cursos.Should().NotBeEmpty();
            #endregion
        }

        [Fact]
        public async Task AdminExcluiCursoComSucesso()
        {
            #region cadastro de curso
            var conteudoProgramaticoDto = new CriarConteudoProgramaticoDto()
            {
                Bibliografia = "Bibliografia Xpto",
                Ementa = "Ementa Xpto",
                MaterialUrl = "https://www.curso-motores.com.br/material",
                Objetivo = "Objetivo Xpto",
            };

            var cursoDto = new CriarCursoDto()
            {
                Titulo = "Fabricação de motores trifásicos",
                Descricao = "Curso avançado de fabricação de motores trifásicos",
                Instrutor = "José Mecanico",
                Nivel = NivelCurso.Avancado,
                ConteudoProgramatico = conteudoProgramaticoDto,
                Valor = 2500
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/cursos", cursoDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var resultadocriacao = await postResponse.Content.ReadFromJsonAsync<ResultDto<Guid>>();
            resultadocriacao?.Success.Should().BeTrue();
            #endregion

            #region exclusão de curso
            var cursoId = resultadocriacao?.Data;

            var deleteResponse = await _httpClient.DeleteAsync($"/api/cursos/{cursoId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            #endregion

            #region consulta o curso excluido
            var consultaResponse = await _httpClient.GetAsync($"/api/cursos/{cursoId}");
            consultaResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            #endregion
        }

        [Fact]
        public async Task AdminAtivaCursoComSucesso()
        {
            #region cadastro de curso
            var conteudoProgramaticoDto = new CriarConteudoProgramaticoDto()
            {
                Bibliografia = "Bibliografia Xpto",
                Ementa = "Ementa Xpto",
                MaterialUrl = "https://www.curso-motores.com.br/material",
                Objetivo = "Objetivo Xpto",
            };

            var cursoDto = new CriarCursoDto()
            {
                Titulo = "Fabricação de motores trifásicos",
                Descricao = "Curso avançado de fabricação de motores trifásicos",
                Instrutor = "José Mecanico",
                Nivel = NivelCurso.Avancado,
                ConteudoProgramatico = conteudoProgramaticoDto,
                Valor = 2500
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/cursos", cursoDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var resultadocriacao = await postResponse.Content.ReadFromJsonAsync<ResultDto<Guid>>();
            resultadocriacao?.Success.Should().BeTrue();
            #endregion

            #region ativação do curso
            var cursoId = resultadocriacao?.Data;

            var ativacaoResponse = await _httpClient.PutAsync($"/api/cursos/{cursoId}/ativar", null);
            ativacaoResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            #endregion

            #region consulta o curso ativado
            var cursoConsultado = await _httpClient.GetFromJsonAsync<CursoDto>($"/api/cursos/{cursoId}");
            cursoConsultado?.Ativo.Should().BeTrue();
            #endregion
        }

        [Fact]
        public async Task AdminInativaCursoComSucesso()
        {
            #region cadastro de curso
            var conteudoProgramaticoDto = new CriarConteudoProgramaticoDto()
            {
                Bibliografia = "Bibliografia Xpto",
                Ementa = "Ementa Xpto",
                MaterialUrl = "https://www.curso-motores.com.br/material",
                Objetivo = "Objetivo Xpto",
            };

            var cursoDto = new CriarCursoDto()
            {
                Titulo = "Fabricação de motores trifásicos",
                Descricao = "Curso avançado de fabricação de motores trifásicos",
                Instrutor = "José Mecanico",
                Nivel = NivelCurso.Avancado,
                ConteudoProgramatico = conteudoProgramaticoDto,
                Valor = 2500
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/cursos", cursoDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var resultadocriacao = await postResponse.Content.ReadFromJsonAsync<ResultDto<Guid>>();
            resultadocriacao?.Success.Should().BeTrue();
            #endregion

            #region inativação do curso
            var cursoId = resultadocriacao?.Data;

            var ativacaoResponse = await _httpClient.PutAsync($"/api/cursos/{cursoId}/inativar", null);
            ativacaoResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            #endregion

            #region consulta o curso inativado
            var cursoConsultado = await _httpClient.GetFromJsonAsync<CursoDto>($"/api/cursos/{cursoId}");
            cursoConsultado?.Ativo.Should().BeFalse();
            #endregion
        }
    }
}
