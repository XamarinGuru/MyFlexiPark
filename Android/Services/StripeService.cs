using System;
using System.Threading.Tasks;
using FlexyPark.Core.Services;
using Stripe;

namespace FlexyPark.UI.Droid.Services
{
	public class StripeService : IStripeService
	{
		public async Task<string> GetCardToken(string cardName, string cardNumber, int expiryMonth, int expiryYear, string cvc)
		{
			var card = new Card
			{
				Name = cardName,
				Number = cardNumber,
				ExpiryMonth = expiryMonth,
				ExpiryYear = expiryYear,
				CVC = cvc
			};

			Console.WriteLine("stripe credit card request params:\n" + card.ToString());

			try
			{
				var token = await StripeClient.CreateToken(card);

				return token.Id;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return null;
		}
	}
}


