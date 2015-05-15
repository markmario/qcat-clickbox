// --------------------------------------------------------------------------------------------------
//  <copyright file="RSAClass.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Infrastructure
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class RsaClass
    {
        #region Constants

        private const string PrivateKey =
            "<RSAKeyValue><Modulus>5Oj3TQ/EWbSuz8eSovkhgtkr/5ywEPaeAQQ3SxI4jD3yhapvw9KahJkOHDnfqhGOONQrp84t3Buh9iy93BvzHby6UmusMldWjHbMeCKrILiuyAkexxfGWAwQs9SgZ1ZmlYCWC64abEyrgNydE8GIbCN+j2t2ne5WuJMTsYz+LR0=</Modulus><Exponent>AQAB</Exponent><P>/79+ZZXWm2Z4Kv8nDRSh7w8UqsEEcXLz6Gwq9/KpSsPFQdZ6/if7PspqfozfTKt61a4VzzK1zKKTVAo3t4/Emw==</P><Q>5SKz+sELp91HclZ2cqQi0eKpvQ9yeGKel3y0m8W1T+7gZejoAVLBejkXcDiSWb/olkqVQU/uqMLzQFFQk0+Epw==</Q><DP>MdA9sVGvHFOoIk/SbmTPab3ZO60ezW4jfejbsbHNMafSGxHIoQpukHtipMWRlOBtq4Md8l6hNHuSELNwyMsy8w==</DP><DQ>zA6HsfxRYQETK6QMgDPkPn5ZI2GqU8Of8NDCFyePPMxDUv6D/wmv/CTz1qDK1NqvS4jIOw3wQKK89r5zv3zFaQ==</DQ><InverseQ>Zw9v7iO8fp6aZOhUK1wqdGgbe+xQj11V32V5jM56MrhR+1ym3Yu2C4E0GLl5/5gsGiCYkySrojMmKmK7VxhPew==</InverseQ><D>DCMJJIhX+pTGZXBNy9YobTOOaZSbjzT579CTDaf84I6uRxfgPHAYzs5HhuMq8KRk7on/nkS2EMLml9LCeNnwhRUxArcU1GbkEAFwAXTqtbe/AH2jwvpm4C8VRdGC5R6AW6L2hhzrjPxDR8tTh8KUsHAg4CrkX2GY/6qshbrrre8=</D></RSAKeyValue>";

        private const string PublicKey =
            "<RSAKeyValue><Modulus>5Oj3TQ/EWbSuz8eSovkhgtkr/5ywEPaeAQQ3SxI4jD3yhapvw9KahJkOHDnfqhGOONQrp84t3Buh9iy93BvzHby6UmusMldWjHbMeCKrILiuyAkexxfGWAwQs9SgZ1ZmlYCWC64abEyrgNydE8GIbCN+j2t2ne5WuJMTsYz+LR0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        #endregion

        #region Static Fields

        private static readonly UnicodeEncoding Encoder = new UnicodeEncoding();

        #endregion

        #region Public Methods and Operators

        public static string Decrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            var dataArray = data.Split(new[] { ',' });
            var dataByte = new byte[dataArray.Length];
            for (var i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }

            rsa.FromXmlString(PrivateKey);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return Encoder.GetString(decryptedByte);
        }

        public static string Encrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PublicKey);
            var dataToEncrypt = Encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray)
            {
                item++;
                sb.Append(x);

                if (item < length)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}