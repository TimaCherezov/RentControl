using Main.Model.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace RentControl.E2ETests.Utils;

public sealed record AdminCredentials(string Email, string Password);

/// <summary>
/// API не позволяет создать админа (регистрация всегда выдаёт роль Client),
/// поэтому тестовый админ регистрируется через HTTP, а роль повышается прямым
/// обновлением в maindb. Логин и дальнейшие действия идут по обычному HTTP-пути.
/// </summary>
public static class AdminSeeder
{
    public static async Task<AdminCredentials> SeedAsync(
        AppHostFixture fixture,
        CancellationToken cancellationToken)
    {
        var email = TestUsers.NewEmail();
        var password = TestUsers.Password;

        await TestUsers.RegisterAsync(fixture.MainApi, email, password, cancellationToken);

        var userId = await fixture.WithMainDbAsync(async (db, token) =>
        {
            var user = await db.Users.SingleAsync(u => u.Email == email, token);
            Assert.Equal(RoleEnum.Client, user.Role);
            user.Role = RoleEnum.Admin;
            await db.SaveChangesAsync(token);
            return user.Id;
        }, cancellationToken);

        Assert.NotEqual(Guid.Empty, userId);
        return new AdminCredentials(email, password);
    }
}
