using System;
using System.IO;
using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: December 23rd 2017
 */
public class SendablePacket
{
    private readonly MemoryStream memoryStream;

    public SendablePacket()
    {
        memoryStream = new MemoryStream();
    }

    public void WriteString(string value)
    {
        if (value != null)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(value);
            // Since we use short value maximum byte size for strings is 32767.
            // Take care that maximum packet size data is 32767 bytes as well.
            // Sending a 32767 byte string would require all the available packet size.
            WriteShort(byteArray.Length);
            WriteBytes(byteArray);
        }
        else
        {
            memoryStream.WriteByte(0);
        }
    }

    public void WriteBytes(byte[] byteArray)
    {
        for (int i = 0; i < byteArray.Length; i++)
        {
            memoryStream.WriteByte(byteArray[i]);
        }
    }

    public void WriteByte(int value)
    {
        memoryStream.WriteByte((byte)value);
    }

    public void WriteShort(int value)
    {
        memoryStream.WriteByte((byte)value);
        memoryStream.WriteByte((byte)(value >> 8));
    }

    public void WriteInt(int value)
    {
        memoryStream.WriteByte((byte)value);
        memoryStream.WriteByte((byte)(value >> 8));
        memoryStream.WriteByte((byte)(value >> 16));
        memoryStream.WriteByte((byte)(value >> 24));
    }

    public void WriteLong(long value)
    {
        memoryStream.WriteByte((byte)value);
        memoryStream.WriteByte((byte)(value >> 8));
        memoryStream.WriteByte((byte)(value >> 16));
        memoryStream.WriteByte((byte)(value >> 24));
        memoryStream.WriteByte((byte)(value >> 32));
        memoryStream.WriteByte((byte)(value >> 40));
        memoryStream.WriteByte((byte)(value >> 48));
        memoryStream.WriteByte((byte)(value >> 56));
    }

    public void WriteFloat(float fvalue)
    {
        long value = BitConverter.SingleToInt32Bits(fvalue);
        memoryStream.WriteByte((byte)value);
        memoryStream.WriteByte((byte)(value >> 8));
        memoryStream.WriteByte((byte)(value >> 16));
        memoryStream.WriteByte((byte)(value >> 24));
    }

    public void WriteDouble(double dvalue)
    {
        long value = BitConverter.DoubleToInt64Bits(dvalue);
        memoryStream.WriteByte((byte)value);
        memoryStream.WriteByte((byte)(value >> 8));
        memoryStream.WriteByte((byte)(value >> 16));
        memoryStream.WriteByte((byte)(value >> 24));
        memoryStream.WriteByte((byte)(value >> 32));
        memoryStream.WriteByte((byte)(value >> 40));
        memoryStream.WriteByte((byte)(value >> 48));
        memoryStream.WriteByte((byte)(value >> 56));
    }

    public byte[] GetSendableBytes()
    {
        // Encrypt bytes.
        byte[] encryptedBytes = Encryption.Process(memoryStream.ToArray()); // Encrypt
        int size = encryptedBytes.Length;

        // Create two bytes for length (short - max length 32767).
        byte[] lengthBytes = new byte[2];
        lengthBytes[0] = (byte)(size & 0xff);
        lengthBytes[1] = (byte)((size >> 8) & 0xff);

        // Join bytes.
        byte[] result = new byte[size + 2];
        Array.Copy(lengthBytes, 0, result, 0, 2);
        Array.Copy(encryptedBytes, 0, result, 2, size);

        // Return the data.
        return result;
    }
}
