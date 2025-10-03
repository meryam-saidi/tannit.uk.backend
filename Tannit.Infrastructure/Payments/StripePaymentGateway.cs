using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Options;
using Tannit.Application.Payments;
using Tannit.Application.Payments.DTOs;

namespace Tannit.Infrastructure.Payments
{
    /*
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly StripeClient _client;
        private readonly StripeSettings _settings;

        public StripePaymentGateway(IOptions<StripeSettings> opts)
        {
            _settings = opts.Value;
            _client = new StripeClient(_settings.SecretKey);
        }

        public async Task<string> CreateCheckoutSessionAsync(Guid transactionId, long amountInMinorUnits, string currency, string successUrl, string cancelUrl, CancellationToken ct = default)
        {
            var service = new SessionService(_client);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = $"{successUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = cancelUrl,
                ClientReferenceId = transactionId.ToString(),
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currency,
                        UnitAmount = amountInMinorUnits, // pence for GBP
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Purchase {transactionId}"
                        }
                    }
                }
            },
                Metadata = new Dictionary<string, string> { { "transactionId", transactionId.ToString() } }
            };

            var session = await service.CreateAsync(options, cancellationToken: ct);
            return session.Url!;
        }

        public bool ValidateWebhookSignature(string payload, string sigHeader, string webhookSecret)
        {
            try
            {
                var _ = EventUtility.ConstructEvent(payload, sigHeader, webhookSecret);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    */
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly StripeSettings _settings;

        public StripePaymentGateway(IOptions<StripeSettings> options)
        {
            _settings = options.Value;
            StripeConfiguration.ApiKey = _settings.SecretKey; // initialisation Stripe
        }

        public async Task<string> CreateCheckoutSessionAsync(decimal amount, string currency, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "TANNIT purchase"
                            },
                            UnitAmount = (long)(amount * 100), // Stripe attend des "cents"
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url; // URL du checkout Stripe
        }
    }
}
