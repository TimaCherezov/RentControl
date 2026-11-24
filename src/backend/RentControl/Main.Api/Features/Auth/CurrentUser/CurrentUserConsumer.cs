using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Auth.CurrentUser;

public class CurrentUserConsumer(
    ICurrentUserService currentUserService,
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<CurrentUserRequest>
{
    public async Task Consume(ConsumeContext<CurrentUserRequest> context)
    {
        var userId = currentUserService.GetUserId();

        var response = await uowFactory.ExecuteAsync(async uow =>
        {
            var user = await uow.Users.GetByIdAsync(userId, context.CancellationToken);
            if (user is null)
            {
                throw new UnauthorizedRequestException("User not found");
            }

            return new UserResponse(user.Id, user.Name, user.Email);
        }, context.CancellationToken);

        await context.RespondAsync(response);
    }
}
