using System;

namespace FlexyPark.Core.Helpers
{
    public static class Base64Extensions
    {
        //Encode
        public static string Base64Encode(this string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        //Decode
//        public static string Base64Decode(this string base64EncodedData) {
//            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
//            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
//        }

    }
}

