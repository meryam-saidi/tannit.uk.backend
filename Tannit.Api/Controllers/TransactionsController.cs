using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tannit.Application.Transactions.Commands.PurchaseTransaction;
using Tannit.Application.Transactions.Commands.ConfirmTransaction;
using Tannit.Application.Transactions.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Tannit.Application.Accounts.Commands;
using Tannit.Application.Transactions;

namespace Tannit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionsController(IMediator mediator, ITransactionRepository transactionRepository)
        {
            _mediator = mediator;
            _transactionRepository = transactionRepository;
        }

        /* Controlleur reçoit directement la commande
        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchaseTransactionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        */

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchaseTannitDto dto)
        {
            var txId = await _mediator.Send(new PurchaseTransactionCommand(dto.UserId, dto.Amount, dto.PaymentMethod));
            return Ok(new { transactionId = txId, status = "Pending" });
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromBody] ConfirmTannitDto dto)
        {
            var txId = await _mediator.Send(new ConfirmTransactionCommand(dto.TransactionId));
            return Ok(new { transactionId = txId, status = "Settled" });
        }
        
        /*
        [Authorize]
        [HttpGet("getUserTransactions")]
        public async Task<IActionResult> GetUserTransactions(CancellationToken ct)
        {
            // 1️⃣ Récupérer l'ID de l'utilisateur connecté depuis le token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            // 2️⃣ Appeler le repository pour récupérer toutes les transactions
            var transactions = await _transactionRepository.GetByUserIdAsync(userId, ct);

            // 3️⃣ Retourner les transactions
            return Ok(transactions);
        }

        */
    }
}
