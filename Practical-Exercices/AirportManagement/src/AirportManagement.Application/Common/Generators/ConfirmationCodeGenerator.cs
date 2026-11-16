using System.Security.Cryptography;

namespace AirportManagement.Application.Common.Generators;

internal static class ConfirmationCodeGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; 

    public static string Generate(int length = 6)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);

        var chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = Alphabet[bytes[i] % Alphabet.Length];

        return new string(chars);
    }
}