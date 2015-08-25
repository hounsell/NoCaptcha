using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace Gnome.NoCaptcha
{
    public class NoCaptcha
    {
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }
        public Guid SessionKey { get; set; }

        public string SecureToken
        {
            get { return EncryptToken(GetToken()); }
        }

        private static readonly DateTime EpochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public NoCaptcha()
        {
            SessionKey = Guid.NewGuid();

            // Load config from Web.config
            SiteKey = ConfigurationManager.AppSettings["nc:SiteKey"];
            SecretKey = ConfigurationManager.AppSettings["nc:SecretKey"];
        }

        public NoCaptcha(string siteKey, string secretKey)
        {
            SessionKey = Guid.NewGuid();

            // Override with provided values
            SiteKey = siteKey;
            SecretKey = secretKey;
        }

        public NoCaptchaResult Validate(string response, string ip)
        {
            HttpWebRequest wreq = HttpWebRequest.Create($"https://www.google.com/recaptcha/api/siteverify?secret={SecretKey}&response={response}&remoteip={ip}") as HttpWebRequest;

            NoCaptchaResult ncResult;
            using (HttpWebResponse wres = wreq.GetResponse() as HttpWebResponse)
            using (StreamReader sr = new StreamReader(wres.GetResponseStream()))
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(NoCaptchaResult));

                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sr.ReadToEnd())))
                {
                    ncResult = dcjs.ReadObject(ms) as NoCaptchaResult;
                }
            }

            return ncResult ?? new NoCaptchaResult()
            {
                Succeeded = false,
                ErrorMessages = new string[]
                {
                    "NoCaptcha: Unable to parse data from Google"
                }
            };
        }

        private long ToUnixTime(DateTime date)
        {
            return Convert.ToInt64((date - EpochStart).TotalMilliseconds);
        }

        private string GetToken()
        {
            return $"{{\"session_id\":\"{SessionKey}\",\"ts_ms\":{ToUnixTime(DateTime.UtcNow)}}}";
        }

        private string EncryptToken(string token)
        {
            byte[] cryptKey = GetEncryptionKey(SecretKey);
            byte[] cryptToken = AesEncrypt(token, cryptKey, cryptKey);

            return Convert.ToBase64String(cryptToken)
                .Replace("=", "")
                .Replace("+", "-")
                .Replace("/", "_");
        }

        private byte[] GetEncryptionKey(string baseKey)
        {
            byte[] baseKeyBytes = Encoding.UTF8.GetBytes(baseKey);
            byte[] baseKeySha1;
            using (SHA1 sha = SHA1.Create())
            {
                baseKeySha1 = sha.ComputeHash(baseKeyBytes);
            }
            byte[] cryptoKey = new byte[16];
            Array.Copy(baseKeySha1, cryptoKey, 16);

            return cryptoKey;
        }

        private static byte[] AesEncrypt(string value, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }
            if (key == null || key.Length == 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length == 0)
            {
                throw new ArgumentNullException("iv");
            }

            byte[] output;
            using (AesManaged aes = new AesManaged()
            {
                Key = key,
                IV = iv,
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.ECB
            })
            using (MemoryStream ms = new MemoryStream())
            {
                ICryptoTransform ct = aes.CreateEncryptor(aes.Key, aes.IV);
                using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(value);
                }

                output = ms.ToArray();
            }

            return output;
        }
    }

    [DataContract]
    public class NoCaptchaResult
    {
        [DataMember(Name = "error-codes")]
        public string[] ErrorMessages { get; set; }

        [DataMember(Name = "success")]
        public bool Succeeded { get; set; }
    }
}
