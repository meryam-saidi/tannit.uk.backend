using MediatR;

namespace Tannit.Application.Accounts.Commands;

public sealed record EnsureUserAccountCommand(Guid AuthUserId, string Email) : IRequest;
