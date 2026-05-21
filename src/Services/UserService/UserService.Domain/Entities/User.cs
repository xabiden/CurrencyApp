namespace UserService.Domain.Entities;

public class User
{
    public const int MaxNameLength = 100;

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string PasswordHash { get; private set; }

    private User()
    {
        Name = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(Guid id, string name, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        if (name.Length > MaxNameLength)
            throw new ArgumentException(
                $"User name cannot exceed {MaxNameLength} characters.", nameof(name));

        Id = id;
        Name = name.Trim();
        PasswordHash = passwordHash;
    }
}
