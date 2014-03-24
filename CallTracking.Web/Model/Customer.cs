using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Massive;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Web.Security;
using CallTracking.Web.Infrastructure.Logging;
using System.Dynamic;

namespace CallTracking.Web.Model
{
    public class Customer : DynamicModel
    {
        ILogger _logger;

        public enum PasswordFormatEnum { Clear = 1, Hashed = 2, Encrypted = 3 };

        public Customer(ILogger logger)
            : base("MassiveConnection", "CallTracking_Customers", "Id")
        {
            _logger = logger;
        }

        public MembershipCreateStatus Register(string email, string password, string confirmPassword, string referKey, string referedBy)
        {
            if (password.Equals(confirmPassword))
            {
                try
                {
                    if (this.All(where: "Email = @0", args: email).Count() == 0)
                    {
                        int format = (int)PasswordFormatEnum.Encrypted;
                        string salt = GenerateSalt();
                        string pass = EncodePassword(password, format, salt);

                        this.Insert(new
                        {
                            Email = email,
                            IsSubscribed = true,
                            PasswordFormat = format,
                            Password = pass,
                            PasswordSalt = salt,
                            ReferKey = referKey,
                            ReferredBy = referedBy
                        });
                        return MembershipCreateStatus.Success;
                    }
                    else
                        return MembershipCreateStatus.DuplicateEmail;
                    //"A user with that email address already exists. Please use the Forgotten Password link if you need to recover your password.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                    return MembershipCreateStatus.ProviderError;
                }
            }
            return MembershipCreateStatus.InvalidPassword;
        }

        public bool Validate(string email, string password)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
                return false;

            bool result = false;
            var m = this.GetByEmail(email);
            if (m != null)
            {
                string pass = EncodePassword(password, m.PasswordFormat, m.PasswordSalt);
                result = m.Password.Equals(pass);
            }
            return result;
        }

        public dynamic GetByEmail(string email)
        {
            dynamic table = new Customer(_logger);
            var first = table.First(Email: email);
            return first;
        }
        public string ResetPassword(string email)
        {
            dynamic c = GetByEmail(email);
            if (c != null)
            {
                string pass = Membership.GeneratePassword(8, 2);
                c.Password = EncodePassword(pass, c.PasswordFormat, c.PasswordSalt);
                this.Update(c, c.Id);
                return pass;
            }
            return String.Empty;
        }

        #region Private Helpers
        private string GenerateSalt()
        {
            byte[] buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }
        private string EncodePassword(string pass, int passwordFormat, string salt)
        {
            if (passwordFormat == 0) // MembershipPasswordFormat.Clear
                return pass;

            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet = null;

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
            if (passwordFormat == 1)
            { // MembershipPasswordFormat.Hashed
                HashAlgorithm s = HashAlgorithm.Create("SHA1");
                bRet = s.ComputeHash(bAll);
            }
            else
            {
                return Encrypt(pass, "t4L3nt5!t3", salt, "SHA1", 1, "@1B2c6D4e9F6g7f7", 128);
            }
            return Convert.ToBase64String(bRet);
        }
        internal string Encrypt(string plainText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }
        internal string Decrypt(string cipherText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                          decryptor,
                                                          CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                       0,
                                                       plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }
        #endregion
    }
}