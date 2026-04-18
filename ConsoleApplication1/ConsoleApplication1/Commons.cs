using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DLL
{
    public class Commons
    {

        public static class TimeInRemarks
        {
            public static string ON_TIME = "On Time";
            public static string LATE = "Late";
            public static string MISS_PUNCH = "Miss Punch";
            public static string OFF = "Off";
            public static string AB = "Absent";
        }

        public static class TimeOutRemarks
        {
            public static string ON_TIME = "On Time";
            public static string EARLY_GONE = "Early Out";
            public static string MISS_PUNCH = "Miss Punch";
            public static string OFF = "Off";
            public static string AB = "Absent";
        }

        public class FinalRemarks
        {
            public static string PRESENT = "PO";


            public static string POE = "POE";
            public static string POM = "POM";
            public static string PLO = "PLO";
            public static string PLE = "PLE";
            public static string PLM = "PLM";

            public static string OV = "OV";
            public static string OM = "OM";
            public static string OT = "OT";
            public static string OD = "OD";
            public static string OTV = "OTV";

            // These two will never occur, they are
            // just here for completion.
            public static string PMO = "PMO";
            public static string PME = "PME";

            public static string LV = "LV";

            public static string ABSENT = "AB";

            public static string OFF = "OFF";
        }

        public enum actionCode
        {
            insert,
            update,
            delete
        };

        public static class Roles
        {
            public const string ROLE_HR = "TimeTuneHR";
            public const string ROLE_EMP = "TimeTuneEMP";
            public const string ROLE_LM = "TimeTuneLM";
            public const string ROLE_SLM = "TimeTuneSLM";

            // This role will not reside in AccessGroup
            public const string ROLE_SUPER_USER = "TimeTuneSU";
            public const string ROLE_ADMIN = "TimeTuneADMIN";
            public const string ROLE_REPORT = "TimeTuneREPORT";
            public const string ROLE_SUDO = "TimeTuneSUDO";
        }

        public static class FunctionRoles
        {
            public const string FUNC_STUDENT = "student";
            public const string FUNC_LECTURER = "lecturer";
            public const string FUNC_STAFF = "staff";
        }


        public class Passwords
        {
            public static string[] generatePasswordAndSalt(string value)
            {
                // generate a 128-bit salt using a secure PRNG
                byte[] salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                string sal = Convert.ToBase64String(salt);

                string password = System.Text.Encoding.UTF8.GetString(getHash(value, sal));

                return new string[] { password, sal };
            }

            public static void setPassword(DLL.Models.Employee emp, string value)
            {
                // generate password and salt for the requested values.
                string[] passwordAndSalt = generatePasswordAndSalt(value);

                // If there was no error in generation.
                // proceed and assign password and salt.
                if (passwordAndSalt.Length == 2)
                {
                    emp.password = passwordAndSalt[0];

                    emp.salt = passwordAndSalt[1];
                }
            }

            public static bool validateHash(string attemptedPassword, string storedHash, string storedSalt)
            {
                string hashed = System.Text.Encoding.UTF8.GetString(getHash(attemptedPassword, storedSalt));

                return storedHash.Equals(hashed);
            }

            public static bool validate(DLL.Models.Employee emp, string attemptedPassword)
            {
                return validateHash(attemptedPassword, emp.password, emp.salt);
            }

            #region Helpers
            private static byte[] getHash(string password, string salt)
            {
                byte[] unhashedBytes = Encoding.Unicode.GetBytes(String.Concat(salt, password));

                SHA256Managed sha256 = new SHA256Managed();
                byte[] hashedBytes = sha256.ComputeHash(unhashedBytes);

                return hashedBytes;
            }

            private static bool compareHash(string attemptedPassword, byte[] hash, string salt)
            {
                string base64Hash = Convert.ToBase64String(hash);
                string base64AttemptedHash = Convert.ToBase64String(getHash(attemptedPassword, salt));

                return base64Hash == base64AttemptedHash;
            }
            #endregion
        }


    }
}
