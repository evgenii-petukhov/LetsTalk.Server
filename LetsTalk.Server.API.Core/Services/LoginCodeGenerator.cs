using System.Security.Cryptography;
using LetsTalk.Server.API.Core.Abstractions;

namespace LetsTalk.Server.API.Core.Services;

public class LoginCodeGenerator : ILoginCodeGenerator
{
    public int GenerateCode()
    {
        byte[] randomNumber = new byte[4];
        RandomNumberGenerator.Fill(randomNumber);
        int value = BitConverter.ToInt32(randomNumber, 0) & 0x7FFFFFFF; // Ensure positive number
        return 1000 + (value % 9000); // Ensure the number is between 1000 and 9999
    }
}
