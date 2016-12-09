using System;

namespace FlexyPark.Core.Models
{
    public class User
    {
        public User(){}

        public User(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }

        public string UserId { get; set;}
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public string Email {get;set;}
        public string PhoneNumber {get;set;}
        public string FacebookId {get;set;}
		public string DateOfBirth { get; set; }
        public string Street {get;set;}
        public string City {get;set;}
        public string PostalCode {get;set;}
        public string Country {get;set;}
		//public string RemainingCredits {get;set;}
		private string remainingCredits = "0";
		public string RemainingCredits
		{
			get
			{
				return remainingCredits;
			}
			set
			{
				remainingCredits = value;
			}
		}
        /////password
        public string Password {get;set;}
    }
}

