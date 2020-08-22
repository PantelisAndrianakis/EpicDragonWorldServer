using System.Security.Cryptography;

/**
 * AES Rijndael encryption.
 * Author: Pantelis Andrianakis
 * Date: December 23rd 2017
 */
public class Encryption
{
    private static readonly RijndaelManaged CIPHER = new RijndaelManaged();
    private static readonly ICryptoTransform ENCRYPTOR = CIPHER.CreateEncryptor(Config.ENCRYPTION_SECRET_KEYWORD, Config.ENCRYPTION_PRIVATE_PASSWORD);
    private static readonly ICryptoTransform DECRYPTOR = CIPHER.CreateDecryptor(Config.ENCRYPTION_SECRET_KEYWORD, Config.ENCRYPTION_PRIVATE_PASSWORD);

    public static byte[] Encrypt(byte[] bytes)
    {
        return ENCRYPTOR.TransformFinalBlock(bytes, 0, bytes.Length);
    }

    public static byte[] Decrypt(byte[] bytes)
    {
        return DECRYPTOR.TransformFinalBlock(bytes, 0, bytes.Length);
    }
}