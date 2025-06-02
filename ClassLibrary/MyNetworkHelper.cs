using System.Net.Sockets;

namespace ClassLibrary;

public static class MyNetworkHerper
{
    public static void SendDataBlock(NetworkStream networkStream, byte[] data)
    {
        // Відправити число - розмір рядка
        byte[] sizeBuffer = BitConverter.GetBytes(data.Length);
        networkStream.Write(sizeBuffer, 0, sizeBuffer.Length);
        // Відправити дані
        networkStream.Write(data, 0, data.Length);
        Console.WriteLine($"Data size {data.Length} send");
    }

    public static byte[] GetDataBlock(NetworkStream networkStream)
    {
        // Отримати число - розмір рядка
        byte[] sizeBuffer = new byte[sizeof(int)];
        networkStream.Read(sizeBuffer, 0, sizeBuffer.Length);
        int size = BitConverter.ToInt32(sizeBuffer, 0);



        byte[] dataBuffer = new byte[size];
        int offset = 0;
        while (offset < size)
        {
            int bytesRead = networkStream.Read(dataBuffer, offset, size - offset);
            if (bytesRead == 0)
            {
                throw new Exception("Connection closed by the client");
            }
            offset += bytesRead;
        }
        Console.WriteLine($"Data size {size} received");
        return dataBuffer;
    }

}
