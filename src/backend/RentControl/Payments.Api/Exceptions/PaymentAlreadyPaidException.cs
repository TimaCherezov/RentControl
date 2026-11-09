namespace Payments.Api.Exceptions;

public class PaymentAlreadyPaidException(string message) : Exception(message);
