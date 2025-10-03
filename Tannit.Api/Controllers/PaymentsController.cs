using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Tannit.Application.Transactions.Commands.ConfirmTransaction;
using Tannit.Application.Transactions.Commands.PurchaseTransaction;    
using Tannit.Infrastructure.Payments;
using Tannit.Application.Payments.DTOs;
using Tannit.Application.Payments;
using System.Security.Claims;
using Tannit.Domain.Entities;
using Tannit.Infrastructure.Repositories;
using Tannit.Application.Transactions;
using Microsoft.AspNetCore.Authorization;
using Tannit.Api.Services;

namespace Tannit.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        /*
        private readonly StripeClient _stripeClient;
        private readonly IMediator _mediator;

        public PaymentsController(StripeClient stripeClient, IMediator mediator)
        {
            _stripeClient = stripeClient;
            _mediator = mediator;
        }
        */
        private readonly IPaymentGateway _paymentGateway;
        private readonly ITransactionRepository _transactionRepository;
        private readonly TransactionVestingService _TransactionVestingService;
        private readonly IMediator _mediator;

        public PaymentsController(IPaymentGateway paymentGateway, ITransactionRepository transactionRepository, IMediator mediator)
        {
            _paymentGateway = paymentGateway;
            _transactionRepository = transactionRepository;
            _mediator = mediator;
        }
        /*
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            var sessionUrl = await _paymentGateway.CreateCheckoutSessionAsync(
                dto.Amount,
                dto.Currency,
                "https://localhost:3000/dashboard", // redirige après succès
                "https://localhost:5173/cancel"   // redirige si annulé
            );

            return Ok(new { url = sessionUrl });
        }
        */

        [Authorize]
        [HttpPost("create1")] //[HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreatePayment1([FromBody] CreatePaymentDto dto)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            // Conversion Tannit
            int tannitAmount = (int)(dto.Amount / 10); // puisque 1 TANNIT = 10£

            // 1 Créer transaction locale
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = dto.Amount,
                //AmountTannit = tannitAmount,
                Status = "Pending",
                Type = "Top-Up Transaction"
            };

            await _transactionRepository.CreateAsync(transaction);

            // 2️ Créer checkout Stripe

            var sessionUrl = await _paymentGateway.CreateCheckoutSessionAsync(
                dto.Amount,
                dto.Currency,
                $"http://localhost:3000/TopUpTransactionPage?sessionId={{CHECKOUT_SESSION_ID}}",
                "http://localhost:3000/dashboard"
            );

            return Ok(new { url = sessionUrl });
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            // Conversion Tannit (ex: 1 TANNIT = 10 GBP)
            int tannitAmount = (int)(dto.Amount / 10);

            // 1️⃣ Créer transaction locale
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = dto.Amount,
                Status = "Pending",
                Type = "Top-Up Transaction"
            };

            await _transactionRepository.CreateAsync(transaction);

            // 2️⃣ Créer checkout Stripe en liant transaction.Id
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(dto.Amount * 100), // en pence
                            Currency = dto.Currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Top-Up Transaction"
                            }
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"http://localhost:3000/TopUpTransactionPage?sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"http://localhost:3000/PaymentPage",

                // ✅ on passe l'ID transaction en ClientReferenceId
                ClientReferenceId = transaction.Id.ToString()
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Ok(new { url = session.Url });
        }

        /*
        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetStripeSession(string sessionId)
        {
            var service = new SessionService();
            try
            {
                var session = await service.GetAsync(sessionId);
                var txId = Guid.Parse(session.SuccessUrl.Split("txId=")[1]);
                // Ici tu peux mettre à jour la transaction dans ta DB si nécessaire
                //_TransactionVestingService.SettleTransactionAsync(session.ClientReferenceId)
                await _mediator.Send(new ConfirmTransactionCommand(txId));

                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        */

        /*
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            // 1. Créer transaction Pending dans ta DB
            var txId = await _mediator.Send(new PurchaseTransactionCommand(dto.UserId, dto.Tannit, "stripe"));

            // 2. Créer session Stripe Checkout
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = $"https://tonsite.com/payment/success?txId={txId}",
                CancelUrl = $"https://tonsite.com/payment/cancel",
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "gbp",
                        UnitAmount = dto.Amount * 100, // Stripe en pence
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Purchase {dto.Tannit} TANNITs"
                        }
                    },
                    Quantity = 1
                }
            }
            };

            var service = new SessionService(_stripeClient);
            var session = await service.CreateAsync(options);

            // 3. Retourner URL de redirection au frontend
            return Ok(new { redirectUrl = session.Url });
        }
        */

        /*
        // Stripe webhook (confirmation paiement)
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var settings = HttpContext.RequestServices.GetRequiredService<IOptions<StripeSettings>>().Value;

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], settings.WebhookSecret);

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    var txId = Guid.Parse(session.SuccessUrl.Split("txId=")[1]);

                    // Confirmer transaction Settled
                    await _mediator.Send(new ConfirmTransactionCommand(txId));
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook Error: {ex.Message}");
            }
        }
        */
        /*
        [HttpPost("webhook1")]
        public async Task<IActionResult> StripeWebhook1()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var settings = HttpContext.RequestServices.GetRequiredService<IOptions<StripeSettings>>().Value;

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    settings.WebhookSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    // 1️⃣ Récupère la transaction dans ta DB avec session.Id
                    var txId = Guid.Parse(session.SuccessUrl.Split("txId=")[1]);

                    if (txId != null)
                    {
                        // 2️⃣ Settle la transaction
                        await _mediator.Send(new ConfirmTransactionCommand(txId));
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
        */


        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var settings = HttpContext.RequestServices.GetRequiredService<IOptions<StripeSettings>>().Value;

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    settings.WebhookSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    if (!string.IsNullOrEmpty(session.ClientReferenceId))
                    {
                        var txId = Guid.Parse(session.ClientReferenceId);

                        // 2️⃣ Settle la transaction
                        await _mediator.Send(new ConfirmTransactionCommand(txId));
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }


    }

}
