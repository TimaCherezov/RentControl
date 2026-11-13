namespace Main.Api.Features.Bookings;

public static class BookingsServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBookings(IConfiguration configuration)
        {
            services
                .AddOptions<BookingOptions>()
                .Bind(configuration.GetSection(BookingOptions.Section))
                .Validate(x => x.DepositRate >= 0m, "DepositRate must be non-negative")
                .Validate(
                    x => x.MaxBookingDuration > TimeSpan.Zero,
                    "MaxBookingDuration must be positive"
                )
                .ValidateOnStart();

            services.AddScoped<IBookingStatusStateService, BookingStatusStateService>();

            return services;
        }
    }
}
