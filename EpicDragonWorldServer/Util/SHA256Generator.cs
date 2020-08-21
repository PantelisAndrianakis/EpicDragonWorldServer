using System.Security.Cryptography;
using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: December 28th 2017
 */
public class SHA256Generator
{
    public static string Calculate(string input)
    {
        // Calculate SHA256 hash from input.
        SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Convert byte array to hex string.
        string result = "";
        for (int i = 0; i < hash.Length; i++)
        {
            result += hash[i].ToString("x2");
        }

        // Return the result.
        return result;
    }
}
