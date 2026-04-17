using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Auth.Api.Data;
using PlataformaEducacional.Core.Enumerators;
using PlataformaEducacional.Core.Extensions;
using PlataformaEducacional.SeedDados;

namespace PlataformaEducacional.Auth.Api.Configurations.Seed;

public static class DatabaseMigrationStartDataExtension
{
    public static void UseDatabseMigrationStartData(this WebApplication app)
    {
        EnsureSeedData(app).Wait();
    }

    private static async Task EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var env = services.GetRequiredService<IWebHostEnvironment>();

        try
        {
            if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

                await context.Database.MigrateAsync();

                await EnsureSeedRoles(context);
                await EnsureSeedSecurity(userManager, context);
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }

    private static async Task EnsureSeedRoles(ApplicationDbContext contextIdentity)
    {
        // Verifica se já existem roles criadas
        if (await contextIdentity.Roles.AnyAsync())
            return;

        // Obtém todos os valores do enum TipoUsuario
        var tipoUsuarios = Enum.GetValues(typeof(TipoUsuario)).Cast<TipoUsuario>();

        foreach (var tipoUsuario in tipoUsuarios)
        {
            var roleName = tipoUsuario.GetDescription();
            var normalizedRoleName = roleName.ToUpperInvariant();
            if (!await contextIdentity.Roles.AnyAsync(r => r.NormalizedName == normalizedRoleName))
            {
                await contextIdentity.Roles.AddAsync(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName,
                    NormalizedName = normalizedRoleName,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }
        }

        contextIdentity.SaveChanges();
    }

    private static async Task EnsureSeedSecurity(UserManager<IdentityUser> userManager, ApplicationDbContext contextSecurity)
    {
        await SeedAdmin(userManager);
        await SeedAluno(userManager);
        contextSecurity.SaveChanges();
    }

    private static async Task SeedAluno(UserManager<IdentityUser> userManager)
    {
        if (await userManager.FindByEmailAsync(SeedUsuario.UsuarioEmail) != null)
            return;

        var userAluno = new IdentityUser
        {
            Id = SeedUsuario.UsuarioId.ToString(),
            UserName = SeedUsuario.UsuarioEmail,
            NormalizedUserName = SeedUsuario.UsuarioEmail.ToUpperInvariant(),
            Email = SeedUsuario.UsuarioEmail,
            NormalizedEmail = SeedUsuario.UsuarioEmail.ToUpperInvariant(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        var result = await userManager.CreateAsync(userAluno, SeedUsuario.UsuarioSenha);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(userAluno, TipoUsuario.Aluno.GetDescription().ToUpperInvariant());
        }
    }

    private static async Task SeedAdmin(UserManager<IdentityUser> userManager)
    {
        if (await userManager.FindByEmailAsync(SeedUsuario.AdminEmail) != null)
            return;

        var userAdmin = new IdentityUser
        {
            UserName = SeedUsuario.AdminEmail,
            NormalizedUserName = SeedUsuario.AdminEmail.ToUpperInvariant(),
            Email = SeedUsuario.AdminEmail,
            NormalizedEmail = SeedUsuario.AdminEmail.ToUpperInvariant(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        var result = await userManager.CreateAsync(userAdmin, SeedUsuario.AdminSenha);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(userAdmin, TipoUsuario.Administrador.GetDescription().ToUpperInvariant());
        }
    }
}
