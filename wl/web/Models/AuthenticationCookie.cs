using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace web.Models
{
    public class AuthenticationCookie
    {
        public const string COOKIE_NAME = "wl-auth-cookie";

        //private const string IV = ":hVrM83EJS#]yG&y";
        //private const string Key = "TRwg?\\x35?eRw7z";

        private const string KEY = ":hVrM83EJS#]yG&yTRwg?\\x35?eRw7z";

        private readonly AuthenticationCookiePayload _payload;

        public AuthenticationCookie()
        {
            _payload = new AuthenticationCookiePayload();
        }

        public AuthenticationCookie(HttpCookie cookie)
        {
            if (cookie.Name != COOKIE_NAME)
                throw new InvalidOperationException("The cookie is not a wl-auth-cookie.");

            var json = Decrypt(cookie.Value);

            var val = Json.Decode<AuthenticationCookiePayload>(json);
            _payload = new AuthenticationCookiePayload();
            this.Username = val.Username;
            this.Password = val.Password;
            this.IsAuthenticated = val.IsAuthenticated;
            this.Timeout = val.Timeout;
        }

        public string Username
        {
            get
            {
                return _payload.Username;
            }
            set
            {
                _payload.Username = value;
            } 
        }
        public string Password
        {
            get
            {
                return _payload.Password;
            }
            set
            {
                _payload.Password = value;
            }
        }
        public bool IsAuthenticated
        {
            get
            {
                return _payload.IsAuthenticated;
            }
            set
            {
                _payload.IsAuthenticated = value;
            }
        }
        public long Timeout
        {
            get
            {
                return _payload.Timeout;
            }
            set
            {
                _payload.Timeout = value;
            }
        }

        private string Encrypt(string value)
        {
            using (var aes = Rijndael.Create())
            {
                aes.Padding = PaddingMode.ISO10126;
                var pdb = GetPasswordDeriveBytes();

                aes.IV = pdb.GetBytes(16);
                aes.Key = pdb.GetBytes(32);

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    var data = Encoding.UTF8.GetBytes(value);
                    cs.Write(data, 0, data.Length);

                    cs.FlushFinalBlock();

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private string Decrypt(string value)
        {
            using (var aes = Rijndael.Create())
            using (var pdb = GetPasswordDeriveBytes())
            {
                aes.Padding = PaddingMode.ISO10126;

                aes.IV = pdb.GetBytes(16);
                aes.Key = pdb.GetBytes(32);

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    var data = Convert.FromBase64String(value);

                    cs.Write(data, 0, data.Length);

                    cs.FlushFinalBlock();

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        private PasswordDeriveBytes GetPasswordDeriveBytes()
        {
            var pdb = new PasswordDeriveBytes(Encoding.UTF8.GetBytes(KEY), new byte[] {
                    0x01,
                    0x21,
                    0x23,
                    0x01,
                    0x54,
                    0x20
                });
            return pdb;
        }

        public HttpCookie ToCookie()
        {
            return new HttpCookie(COOKIE_NAME, Encrypt(Json.Encode(_payload)));
        }

        private class AuthenticationCookiePayload
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsAuthenticated { get; set; }
            public long Timeout { get; set; }
        }
    }
}