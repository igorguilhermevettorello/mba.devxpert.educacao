using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using PlataformaEducacional.Auth.Api.Configurations.Seed;
using PlataformaEducacional.Auth.Api.Data;
using PlataformaEducacional.Core.Enumerators;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacional.Core.Extensions;

namespace PlataformaEducacional.Auth.Api.Tests
{
    public class DatabaseMigrationStartDataExtensionTests
    {
        private static MethodInfo GetPrivateStatic(string name)
            => typeof(DatabaseMigrationStartDataExtension).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!;

        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var configuration = new ConfigurationBuilder().Build();
            return new ApplicationDbContext(options, configuration);
        }

        private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(store.Object,
                null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task EnsureSeedRoles_AddsAllRoles_WhenNoneExist()
        {
            // arrange
            await using var context = CreateInMemoryContext();

            // ensure empty
            Assert.False(await context.Roles.AnyAsync());

            var method = GetPrivateStatic("EnsureSeedRoles");

            // act
            var task = (Task)method.Invoke(null, new object[] { context })!;
            await task;

            // assert
            var expectedCount = Enum.GetValues(typeof(TipoUsuario)).Cast<TipoUsuario>().Count();
            var actualCount = await context.Roles.CountAsync();
            Assert.Equal(expectedCount, actualCount);

            // check presence of normalized names
            foreach (TipoUsuario t in Enum.GetValues(typeof(TipoUsuario)))
            {
                var norm = t.GetDescription().ToUpperInvariant();
                Assert.True(await context.Roles.AnyAsync(r => r.NormalizedName == norm));
            }
        }

        [Fact]
        public async Task EnsureSeedRoles_DoesNotDuplicate_WhenRolesAlreadyExist()
        {
            // arrange
            await using var context = CreateInMemoryContext();

            // add one role to simulate pre-seeded DB
            var firstRole = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = TipoUsuario.Aluno.GetDescription(),
                NormalizedName = TipoUsuario.Aluno.GetDescription().ToUpperInvariant()
            };
            await context.Roles.AddAsync(firstRole);
            await context.SaveChangesAsync();

            var countBefore = await context.Roles.CountAsync();

            var method = GetPrivateStatic("EnsureSeedRoles");

            // act
            var task = (Task)method.Invoke(null, new object[] { context })!;
            await task;

            // assert
            var countAfter = await context.Roles.CountAsync();
            Assert.True(countAfter >= countBefore);
            // ensure no duplicates for normalized name
            var duplicates = (await context.Roles.ToListAsync())
                .GroupBy(r => r.NormalizedName)
                .Any(g => g.Count() > 1);
            Assert.False(duplicates);
        }

        [Fact]
        public async Task EnsureSeedSecurity_CallsSaveChanges_AfterSeeding()
        {
            // arrange
            var userManagerMock = CreateUserManagerMock();

            // simulate "not found" so CreateAsync will be invoked
            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser?)null);

            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var configuration = new ConfigurationBuilder().Build();

            // create mock context to verify SaveChanges called (SaveChanges is virtual)
            var contextMock = new Mock<ApplicationDbContext>(options, configuration) { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Verifiable();

            var method = GetPrivateStatic("EnsureSeedSecurity");

            // act
            var task = (Task)method.Invoke(null, new object[] { userManagerMock.Object, contextMock.Object })!;
            await task;

            // assert
            contextMock.Verify(c => c.SaveChanges(), Times.Once);
            // verify that at least one user create was attempted
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task SeedAdmin_CreatesAdmin_WhenNotExists()
        {
            // arrange
            var uf = CreateUserManagerMock();

            // not found -> create
            uf.Setup(u => u.FindByEmailAsync(It.Is<string>(s => s == It.IsAny<string>())))
                .ReturnsAsync((IdentityUser?)null);

            uf.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            uf.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            var method = GetPrivateStatic("SeedAdmin");

            // act
            var task = (Task)method.Invoke(null, new object[] { uf.Object })!;
            await task;

            // assert
            var expectedRole = TipoUsuario.Administrador.GetDescription().ToUpperInvariant();
            uf.Verify(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), expectedRole), Times.Once);
        }

        [Fact]
        public async Task SeedAdmin_DoesNotCreate_WhenAlreadyExists()
        {
            // arrange
            var uf = CreateUserManagerMock();
            uf.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser { Email = "exists@example.com" });

            uf.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Throws(new InvalidOperationException("Should not be called"));

            var method = GetPrivateStatic("SeedAdmin");

            // act / assert: should not throw
            var task = (Task)method.Invoke(null, new object[] { uf.Object })!;
            await task;
        }

        [Fact]
        public async Task SeedAluno1_And_SeedAluno2_Create_WhenNotExists()
        {
            // arrange
            var uf = CreateUserManagerMock();

            uf.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser?)null);

            uf.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            uf.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            var seedAluno1 = GetPrivateStatic("SeedAluno1");
            var seedAluno2 = GetPrivateStatic("SeedAluno2");

            // act
            var t1 = (Task)seedAluno1.Invoke(null, new object[] { uf.Object })!;
            var t2 = (Task)seedAluno2.Invoke(null, new object[] { uf.Object })!;
            await Task.WhenAll(t1, t2);

            // assert expected role name for alunos
            var expectedRole = TipoUsuario.Aluno.GetDescription().ToUpperInvariant();
            uf.Verify(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), expectedRole), Times.AtLeast(2));
        }

        [Fact]
        public async Task SeedAluno1_DoesNotCreate_WhenExists()
        {
            var uf = CreateUserManagerMock();
            uf.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser { Email = "alunoexists@example.com" });

            var seedAluno1 = GetPrivateStatic("SeedAluno1");
            var task = (Task)seedAluno1.Invoke(null, new object[] { uf.Object })!;
            await task;

            uf.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
        }
    }
}