using Microsoft.AspNetCore.Identity;

namespace DotnetBlueprints.Auth.Infrastructure.Services;

public sealed class AspNetPasswordHasher : Application.Interfaces.IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) =>
        _hasher.HashPassword(null!, password);

    public bool Verify(string hashed, string provided) =>
        _hasher.VerifyHashedPassword(null!, hashed, provided) != PasswordVerificationResult.Failed;
}