using System;
using System.IO;
using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: December 23rd 2017
 */
public class ReceivablePacket
{
    private readonly MemoryStream _memoryStream;

    public ReceivablePacket(byte[] bytes)
    {
        _memoryStream = new MemoryStream(bytes);
    }

    public string ReadString()
    {
        // Since we use short value maximum byte size for strings is 32767.
        // Take care that maximum packet size data is 32767 bytes as well.
        // Sending a 32767 byte string would require all the available packet size.
        return Encoding.UTF8.GetString(ReadBytes(ReadShort()));
    }

    public byte[] ReadBytes(int length)
    {
        byte[] result = new byte[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = (byte)_memoryStream.ReadByte();
        }
        return result;
    }

    public int ReadByte()
    {
        return _memoryStream.ReadByte();
    }

    public int ReadShort()
    {
        byte[] byteArray = new byte[2];
        byteArray[0] = (byte)_memoryStream.ReadByte();
        byteArray[1] = (byte)_memoryStream.ReadByte();
        return BitConverter.ToInt16(byteArray);
    }

    public int ReadInt()
    {
        byte[] byteArray = new byte[4];
        byteArray[0] = (byte)_memoryStream.ReadByte();
        byteArray[1] = (byte)_memoryStream.ReadByte();
        byteArray[2] = (byte)_memoryStream.ReadByte();
        byteArray[3] = (byte)_memoryStream.ReadByte();
        return BitConverter.ToInt32(byteArray);
    }

    public long ReadLong()
    {
        byte[] byteArray = new byte[8];
        byteArray[0] = (byte)_memoryStream.ReadByte();
        byteArray[1] = (byte)_memoryStream.ReadByte();
        byteArray[2] = (byte)_memoryStream.ReadByte();
        byteArray[3] = (byte)_memoryStream.ReadByte();
        byteArray[4] = (byte)_memoryStream.ReadByte();
        byteArray[5] = (byte)_memoryStream.ReadByte();
        byteArray[6] = (byte)_memoryStream.ReadByte();
        byteArray[7] = (byte)_memoryStream.ReadByte();
        return BitConverter.ToInt64(byteArray);
    }

    public float ReadFloat()
    {
        byte[] byteArray = new byte[4];
        byteArray[0] = (byte)_memoryStream.ReadByte();
        byteArray[1] = (byte)_memoryStream.ReadByte();
        byteArray[2] = (byte)_memoryStream.ReadByte();
        byteArray[3] = (byte)_memoryStream.ReadByte();
        return BitConverter.ToSingle(byteArray);
    }

    public double ReadDouble()
    {
        byte[] byteArray = new byte[8];
        byteArray[0] = (byte)_memoryStream.ReadByte();
        byteArray[1] = (byte)_memoryStream.ReadByte();
        byteArray[2] = (byte)_memoryStream.ReadByte();
        byteArray[3] = (byte)_memoryStream.ReadByte();
        byteArray[4] = (byte)_memoryStream.ReadByte();
        byteArray[5] = (byte)_memoryStream.ReadByte();
        byteArray[6] = (byte)_memoryStream.ReadByte();
        byteArray[7] = (byte)_memoryStream.ReadByte();
        return BitConverter.ToDouble(byteArray);
    }
}
