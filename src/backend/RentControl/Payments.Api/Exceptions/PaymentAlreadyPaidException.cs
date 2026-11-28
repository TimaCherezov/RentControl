using RentControl.ServiceDefaults.Exceptions;

namespace Payments.Api.Exceptions;

public class PaymentAlreadyPaidException(string message)
    : ApiException(message, StatusCodes.Status409Conflict);
