using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HoursForYourLib
{
    public class PasswordHandler
    {
        //----------------------------------------------------------------------------------------------ValidatePassword

        public async Task<bool> ValidatePassword(string dbPassword, string userPassword)
        {
            //dewcode the password string to bytes
            byte[] dbHashBytes = Convert.FromBase64String(dbPassword);

            //extract the salt from the password hash
            byte[] salt = new byte[20];
            Array.Copy(dbHashBytes, 0, salt, 0, 20);

            //use the ectracted salt to encrypt the given password into a has byte sequence
            var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(userPassword, salt, 10000);
            byte[] enteredHash = pbkdf2.GetBytes(20);

            //comapre the two hash sequences byte by byte
            for (int i = 0; i < 20; i++)
            {
                //if any byte is different, the passwords do not match
                if (dbHashBytes[i + 20] != enteredHash[i])
                {
                    //fault out
                    return await Task.FromResult(false);
                }
            }
            //if all bytes match, the passwords are the same
            return await Task.FromResult(true);
        }//end ValidatePassword method

        //----------------------------------------------------------------------------------------------EncryptPassword

        //using PBKDF2 to encrypt the password with SALTING
        public async Task<string> EncryptPassword(string password)
        {
            //create the salt (20 bytes long) for password encryption and decryption using cryptographic PRNG
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[20]);

            //create the Rfc2898DeriveBytes
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            //get the hash value (20 bytes long)
            byte[] hash = pbkdf2.GetBytes(20);

            //combine the salt with the hash starting with the salt
            byte[] saltedHashBytes = new byte[40];
            Array.Copy(salt, 0, saltedHashBytes, 0, 20);//past from index 0 -- 20
            Array.Copy(hash, 0, saltedHashBytes, 20, 20);//paste from index 20 -- 40

            //return a string of the salt+hash in base64 format for storage
            return await Task.FromResult(Convert.ToBase64String(saltedHashBytes));
        }//end EncryptPassword method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________