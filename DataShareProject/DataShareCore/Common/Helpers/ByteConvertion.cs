using System.Text.RegularExpressions;

namespace DataShareCore.Common.Helper;

using System.Text;


public class ByteConvertion
{
    public static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    public static byte[] StringToByteArray(String hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }
    
    public static bool IsValidBase64String(string input)
    {
        // Check if input is null or empty
        if (string.IsNullOrEmpty(input))
            return false;

        // Check if the input length is a multiple of 4
        if (input.Length % 4 != 0)
            return false;

        // Regular expression pattern for Base64 string validation
        string pattern = @"^(?:[A-Za-z0-9+/]{4})*" + // Match any number of valid 4-character blocks
                         @"(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$"; // Match the correct padding

        // Check if the input matches the pattern
        return Regex.IsMatch(input, pattern);
    }

}
