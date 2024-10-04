using System.Security.Cryptography;

namespace backend.Classes.Security
{
    public class Bcrypt
    {
        public byte[] BcryptHash(string password, byte[]? salt = null, int cost = 10)
        {
            // Generate a random salt if not provided
            if (salt == null)
            {
                salt = GenerateRandomSalt();
            }

            // Encode the salt and cost
            string saltString = EncodeSalt(salt, cost);

            // Stretch the password
            byte[] hashedPassword = StretchPassword(password, saltString, cost);

            return hashedPassword;
        }

        private byte[] StretchPassword(string password, string saltString, int cost)
        {
            byte[] key = System.Text.Encoding.UTF8.GetBytes(password); // Convert password to byte array

            // Apply the bcrypt key stretching algorithm
            for (int i = 0; i < Math.Pow(2, cost); i++)
            {
                key = Crypt(key, saltString);
            }

            return key;
        }

        private byte[] Crypt(byte[] key, string saltString)
        {
            // Use the ComputeHash method of the SHA256 class on the key and salt
            byte[] combined = new byte[key.Length + saltString.Length];
            Buffer.BlockCopy(key, 0, combined, 0, key.Length);
            Buffer.BlockCopy(System.Text.Encoding.UTF8.GetBytes(saltString), 0, combined, key.Length, saltString.Length);

            return Sha256.ComputeHash(combined);
        }

        private byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[16]; // Create a byte array of length 16
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); // Fill the array with random bytes
            }
            return salt;
        }

        private string EncodeSalt(byte[] salt, int cost)
        {
            // Convert the salt to a Base64 string (or another encoding as needed)
            return Convert.ToBase64String(salt) + "$" + cost;
        }
    }
}
