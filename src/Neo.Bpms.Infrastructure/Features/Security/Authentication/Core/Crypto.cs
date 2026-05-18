#pragma warning disable SYSLIB0014 // WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead.
#pragma warning disable SYSLIB0041
using System.Security.Cryptography;

namespace Neo.Bpms.Infrastructure.Features.Security.Authentication.Core;

internal static class Crypto
{
    private const int PBKDF2IterCount = 1000; // default for Rfc2898DeriveBytes
    private const int PBKDF2SubkeyLength = 256 / 8; // 256 bits
    private const int SaltSize = 128 / 8; // 128 bits

    /* =======================
		 * HASHED PASSWORD FORMATS
		 * =======================
		 * 
		 * Version 0:
		 * PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.
		 * (See also: SDL crypto guidelines v5.1, Part III)
		 * Format: { 0x00, salt, subkey }
		 */

    public static string HashPassword(string password)
    {
        if (password == null)
        {
            throw new ArgumentNullException("password");
        }

        // Produce a version 0 (see comment above) text hash.
        byte[] salt;
        byte[] subkey;
#pragma warning disable SYSLIB0060 // Type or member is obsolete
        using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, PBKDF2IterCount))
        {
            salt = deriveBytes.Salt;
            subkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
        }
#pragma warning restore SYSLIB0060 // Type or member is obsolete

        var outputBytes = new byte[1 + SaltSize + PBKDF2SubkeyLength];
        Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
        Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, PBKDF2SubkeyLength);
        return Convert.ToBase64String(outputBytes);
    }
}
#pragma warning restore SYSLIB0041
#pragma warning restore SYSLIB0014

