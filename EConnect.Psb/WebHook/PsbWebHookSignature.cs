using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EConnect.Psb.WebHook;

public static class PsbWebHookSignature
{
    private static string CreateSignatureText(byte[] messageHash)
    {
        var builder = new StringBuilder();
        builder.Append("sha256=");
        foreach (var b in messageHash)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }

    public static string ComputeSignature(string message, string secret)
    {
        var encoding = new UTF8Encoding();
        var keyByte = encoding.GetBytes(secret);
        var messageBytes = encoding.GetBytes(message);
        using var hmacSha256 = new HMACSHA256(keyByte);
        var messageHash = hmacSha256.ComputeHash(messageBytes);

        return CreateSignatureText(messageHash);
    }

    public static async Task<string> ComputeSignature(Stream body, string secret)
    {
        var encoding = new UTF8Encoding();
        var keyByte = encoding.GetBytes(secret);

        using var hmacSha256 = new HMACSHA256(keyByte);

        try
        {
            // Split body into 4K chunks.
            var buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = await body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            {
                hmacSha256.TransformBlock(
                    buffer,
                    inputOffset: 0,
                    inputCount: bytesRead,
                    outputBuffer: null,
                    outputOffset: 0);
            }
            hmacSha256.TransformFinalBlock(Array.Empty<byte>(), inputOffset: 0, inputCount: 0);

            return CreateSignatureText(hmacSha256.Hash);
        }
        finally
        {
            // Reset position for Json parser
            body.Seek(0L, SeekOrigin.Begin);
        }
    }
}