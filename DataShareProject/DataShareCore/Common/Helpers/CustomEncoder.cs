using System.Security.Cryptography;
using System.Text;

namespace DataShareCore.Common.Helper;

public class CustomEncoder
{
    public static string EncodeNumber(int number)
    {
        // Convert integer to bytes
        byte[] bytesToEncode = BitConverter.GetBytes(number);

        // Encode bytes to Base64 string
        string encodedString = Convert.ToBase64String(bytesToEncode);

        return encodedString;
    }

    public static int DecodeNumber(string encodedString)
    {
        // Convert Base64 string to bytes
        byte[] bytesToDecode = Convert.FromBase64String(encodedString);

        // Convert bytes back to integer
        int decodedNumber = BitConverter.ToInt32(bytesToDecode, 0);

        return decodedNumber;
    }
    
    public static string EncodeString(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }
    
    public static string DecodeString(string encodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(encodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
    
    
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] hashBytes = ByteConvertion.StringToByteArray(storedHash);
        byte[] saltBytes = ByteConvertion.StringToByteArray(storedSalt);

        using (var hmac = new HMACSHA512(saltBytes))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            // Compare computed hash with stored hash
            return computedHash.SequenceEqual(hashBytes);
        }
    }
}