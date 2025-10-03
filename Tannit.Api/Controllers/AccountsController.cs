using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Tannit.Application.Accounts;
using Tannit.Application.Accounts.Commands;
using Tannit.Application.Transactions;
using Tannit.Infrastructure.Repositories;

namespace Tannit.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserAccountRepository _repo;
    private readonly ITransactionRepository _transactionRepo;

    public AccountsController(IMediator mediator, IUserAccountRepository repo, ITransactionRepository transactionRepo)
    {
        _mediator = mediator; 
        _repo = repo;
        _transactionRepo = transactionRepo;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(email))
            return Unauthorized("Token invalid (missing sub/email).");

        var authUserId = Guid.Parse(userIdStr);

        // assure l’existence du compte métier
        await _mediator.Send(new EnsureUserAccountCommand(authUserId, email));

        // Récupérer toutes les transactions de l'utilisateur
        var userTransactions = await _transactionRepo.GetByUserIdAsync(authUserId); 

        var acct = await _repo.GetByAuthUserIdAsync(authUserId, HttpContext.RequestAborted); //Passer le token du HTTP context

        // Calcul des soldes
        var balance = userTransactions
            .Where(t => t.Status == "Vested")
            .Sum(t => t.Amount);

        return Ok(new
        {
            authUserId = acct!.AuthUserId,
            email = acct.Email,
            balance,
            vestedBalance = acct.VestedBalance,
            kycStatus = acct.KycStatus
        });
    }
}
