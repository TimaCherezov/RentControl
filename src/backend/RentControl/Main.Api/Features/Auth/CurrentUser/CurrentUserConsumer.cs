using System.Security.Claims;
using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Auth.CurrentUser;

public class CurrentUserConsumer(
    IHttpContextAccessor httpContextAccessor,
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<CurrentUserRequest>
{
    public Task Consume(ConsumeContext<CurrentUserRequest> context)
    {
        var idValue = httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(idValue, out var userId))
        {
            throw new UnauthorizedRequestException("Missing/invalid NameIdentifier claim.");
        }

        return uowFactory.ExecuteAsync(async uow =>
        {
            var user = await uow.Users.GetByIdAsync(userId, context.CancellationToken);
            if (user is null)
            {
                throw new UnauthorizedRequestException("User not found");
            }

            await context.RespondAsync(new UserResponse(user.Id, user.Name, user.Email));
        }, context.CancellationToken);
    }
}
