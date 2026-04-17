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
        if (await userManager.FindByEmailAsync(SeedUsuario.USUARIO_EMAIL) != null)
            return;

        var userAluno = new IdentityUser
        {
            Id = SeedUsuario.USUARIO_ID.ToString(),
            UserName = SeedUsuario.USUARIO_EMAIL,
            NormalizedUserName = SeedUsuario.USUARIO_EMAIL.ToUpperInvariant(),
            Email = SeedUsuario.USUARIO_EMAIL,
            NormalizedEmail = SeedUsuario.USUARIO_EMAIL.ToUpperInvariant(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        var result = await userManager.CreateAsync(userAluno, SeedUsuario.USUARIO_SENHA);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(userAluno, TipoUsuario.Aluno.GetDescription().ToUpperInvariant());
        }
    }

    private static async Task SeedAdmin(UserManager<IdentityUser> userManager)
    {
        if (await userManager.FindByEmailAsync(SeedUsuario.ADMIN_EMAIL) != null)
            return;

        var userAdmin = new IdentityUser
        {
            UserName = SeedUsuario.ADMIN_EMAIL,
            NormalizedUserName = SeedUsuario.ADMIN_EMAIL.ToUpperInvariant(),
            Email = SeedUsuario.ADMIN_EMAIL,
            NormalizedEmail = SeedUsuario.ADMIN_EMAIL.ToUpperInvariant(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        var result = await userManager.CreateAsync(userAdmin, SeedUsuario.ADMIN_SENHA);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(userAdmin, TipoUsuario.Administrador.GetDescription().ToUpperInvariant());
        }
    }
}
