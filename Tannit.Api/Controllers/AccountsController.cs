using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tannit.Application.Accounts;
using Tannit.Application.Accounts.Commands;
using Tannit.Infrastructure.Repositories;

namespace Tannit.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserAccountRepository _repo;

    public AccountsController(IMediator mediator, IUserAccountRepository repo)
    {
        _mediator = mediator; _repo = repo;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        //var balanceStr = User.FindFirst("balance")?.Value;
        //var balance = string.IsNullOrEmpty(balanceStr) ? 0 : decimal.Parse(balanceStr);

        if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(email))
            return Unauthorized("Token invalid (missing sub/email).");

        var authUserId = Guid.Parse(userIdStr);

        // assure l’existence du compte métier
        await _mediator.Send(new EnsureUserAccountCommand(authUserId, email));

        var acct = await _repo.GetByAuthUserIdAsync(authUserId, HttpContext.RequestAborted); //Passer le token du HTTP context
        return Ok(new
        {
            authUserId = acct!.AuthUserId,
            email = acct.Email,
            balance = acct.Balance,
            vestedBalance = acct.VestedBalance,
            kycStatus = acct.KycStatus
        });
    }
}
