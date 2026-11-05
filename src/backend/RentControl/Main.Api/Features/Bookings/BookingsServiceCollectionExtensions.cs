namespace Main.Api.Features.Bookings;

public static class BookingsServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBookings()
        {
            services.AddScoped<IBookingStatusStateService, BookingStatusStateService>();

            return services;
        }
    }
}
