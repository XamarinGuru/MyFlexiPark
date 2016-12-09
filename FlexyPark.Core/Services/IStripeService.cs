using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace FlexyPark.Core.Services
{
    public interface IStripeView
    {
        
    }

    public interface IStripeService
    {
		Task<string> GetCardToken(string cardName, string cardNumber, int expiryMonth, int expiryYear, string cvc);
    }

	public class StripeServiceForBank
	{
		public static async Task<string> GetCardToken(string cardName, string cardNumber)
		{
			try
			{
				var client = new HttpClient();
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppConstants.StripeAPIKey);

				string bankParams = "bank_account[account_number]=" + cardNumber+ //cardNumber = "BE89370400440532013000";
									"&bank_account[country]=" + "BE" +
									"&bank_account[currency]=" + "eur" +
									"&bank_account[account_holder_name]=" + cardName +
									"&bank_account[account_holder_type]=" + "individual";

				Debug.WriteLine("stripe bank account api key:\n" + AppConstants.StripeAPIKey);
				Debug.WriteLine("http method:\n" + "POST");
				Debug.WriteLine("request params:\n" + bankParams);

				StringContent encodedContent = new StringContent(bankParams.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

				var response = await client.PostAsync(AppConstants.StripeBankTokenURL, encodedContent);
				var responseJson = response.Content.ReadAsStringAsync().Result;
				var jsonObject = JObject.Parse(responseJson);

				if (jsonObject["error"] != null)
					Debug.WriteLine(jsonObject["error"]);

				return jsonObject["id"].ToString();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			return null;
		}


	}
}

