using DotnetBlueprints.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Auth.Commands.Login;

/// <summary>Authenticates a user and returns a bootstrap token pair.</summary>
public sealed record LoginCommand(string Email, string Password) : IRequest<TokenPair>;

/// <summary>Handles user authentication and issues a bootstrap token.</summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, TokenPair>
{
    private readonly IAuthDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public LoginCommandHandler(IAuthDbContext db, IPasswordHasher hasher, ITokenService tokens)
    {
        _db = db; _hasher = hasher; _tokens = tokens;
    }

    public async Task<TokenPair> Handle(LoginCommand req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email && !u.IsDeleted, ct)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (string.IsNullOrEmpty(user.PasswordHash) || !_hasher.Verify(user.PasswordHash, req.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return await _tokens.IssueAccessAsync(user.Id, ct);
    }
}