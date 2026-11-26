using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

internal class UserBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test User";
    private string _email = "user@example.com";
    private string _passwordHash = "password-hash";
    private RoleEnum _role = RoleEnum.Client;

    public UserBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithRole(RoleEnum role)
    {
        _role = role;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }

    public User Build() => new()
    {
        Id = _id,
        Name = _name,
        Email = _email,
        PasswordHash = _passwordHash,
        Role = _role,
    };
}
