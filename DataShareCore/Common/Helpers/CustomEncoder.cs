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
}