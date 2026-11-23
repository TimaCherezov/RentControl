using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Auth.CurrentUser;

public class CurrentUserConsumer(
    ICurrentUserService currentUserService,
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<CurrentUserRequest>
{
    public Task Consume(ConsumeContext<CurrentUserRequest> context)
    {
        var userId = currentUserService.GetUserId();

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
