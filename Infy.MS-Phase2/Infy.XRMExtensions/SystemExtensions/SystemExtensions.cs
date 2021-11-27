using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class SystemExtensions
    {
        /// <summary>
        /// Extension method to Convert Double value to Decimal value
        /// </summary>
        /// <param name="doubleValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this double doubleValue)
        {
            return Convert.ToDecimal(doubleValue);
        }

        /// <summary>
        /// Extension method to Convert Decimal value to Double value
        /// </summary>
        /// <param name="doubleValue"></param>
        /// <returns></returns>
        public static double ToDouble(this decimal decimalValue)
        {
            return Convert.ToDouble(decimalValue);
        }

        /// <summary>
        /// round up the decimal number 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="places"></param>
        /// <returns></returns>
        public static decimal RoundUp(this decimal input, int places)
        {
            var multiplier = (decimal)Math.Pow(10, Convert.ToDouble(places));
            if (multiplier == 0)
            {
                multiplier = 1;
            }
            return Math.Ceiling(input * multiplier) / multiplier;
        }

        /// <summary>
        /// Check if value is present in the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsIn<T>(this T @id, IEnumerable<T> collection)
        {
            return collection.Any(x => x.Equals(@id));
        }

        /// <summary>
        /// Encrypt given string using key
        /// </summary>
        /// <param name="stringToBeEncrypted"></param>
        /// <param name="encryptionKey"></param>
        /// <returns></returns>
        public static string EncryptString(string stringToBeEncrypted, string encryptionKey)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(stringToBeEncrypted);
            var tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(encryptionKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            var cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// Decrypt given encrypted string using key
        /// </summary>
        /// <param name="stringToBeDecrypted"></param>
        /// <param name="decrypionKey"></param>
        /// <returns></returns>
        public static string DecryptString(string stringToBeDecrypted, string decrypionKey)
        {
            byte[] inputArray = Convert.FromBase64String(stringToBeDecrypted);
            var tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(decrypionKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            var cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
