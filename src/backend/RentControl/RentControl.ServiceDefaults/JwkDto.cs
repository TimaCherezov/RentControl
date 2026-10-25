namespace RentControl.ServiceDefaults;

public record JwkDto(
    string Kty,
    string Use,
    string Alg,
    string Kid,
    string N,
    string E);
