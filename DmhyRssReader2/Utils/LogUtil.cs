﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace DmhyRssReader2.Utils
{
    class LogUtil
    {
        public static void Log(object o)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            MethodBase mb = new StackTrace(true).GetFrame(1).GetMethod();
            sb.Append(string.Format(" [{0}] ", mb.DeclaringType.FullName));
            sb.Append(o.ToString());
            Trace.WriteLine(sb.ToString());
        }
    }
}
