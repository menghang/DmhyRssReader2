using System;
using System.Globalization;

namespace DmhyRssReader2.Utils
{
    class StringUtil
    {
        public static bool Str2UInt16(string s, out int v)
        {
            v = 0;
            try
            {
                v = Convert.ToUInt16(s);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckMagnetlink(string url) => (!string.IsNullOrEmpty(url)) && url.StartsWith("magnet:?xt=urn:btih:", true, CultureInfo.InvariantCulture);
    }
}
