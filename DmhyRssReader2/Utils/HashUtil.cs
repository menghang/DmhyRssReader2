using System.Security.Cryptography;
using System.Text;

namespace DmhyRssReader2.Utils
{
    class HashUtil
    {
        public static string SHA256EncryptString(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256.Create().ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
