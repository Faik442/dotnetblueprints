using DotnetBlueprints.Auth.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<TokenPair>;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenPair>
{
    private readonly ITokenService _tokens;

    public RefreshTokenCommandHandler(ITokenService tokens) => _tokens = tokens;

    public Task<TokenPair> Handle(RefreshTokenCommand req, CancellationToken ct)
        => _tokens.RefreshAsync(req.RefreshToken, ct);
}
