using DotnetBlueprints.Auth.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(Guid UserId, string Email, string? DisplayName) : IRequest;

public sealed class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IAuthDbContext _db;
    public UpdateUserProfileCommandHandler(IAuthDbContext db) => _db = db;

    public async Task Handle(UpdateUserProfileCommand req, CancellationToken ct)
    {
        var emailInUse = await _db.Users
            .AnyAsync(u => !u.IsDeleted && u.Email == req.Email && u.Id != req.UserId, ct);
        if (emailInUse)
            throw new InvalidOperationException("Email is already in use.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == req.UserId, ct)
                   ?? throw new KeyNotFoundException("User not found.");

        if (!string.Equals(user.Email, req.Email, StringComparison.OrdinalIgnoreCase))
            user.ChangeEmail(req.Email);

        if (req.DisplayName is not null && !string.Equals(user.DisplayName, req.DisplayName, StringComparison.Ordinal))
            user.ChangeDisplayName(req.DisplayName);

        await _db.SaveChangesAsync(ct);
    }
}

