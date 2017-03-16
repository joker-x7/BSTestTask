using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Entities;

namespace Logic
{
    public class BinaryHelper
    {
        public void Write(string path, CustomFile file)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate), System.Text.Encoding.Default))
                {
                    writer.Write(TypeToByte<Header>(file.Header));
                    writer.Flush();

                    foreach (TradeRecord trade in file.Trades)
                    {
                        writer.Write(TypeToByte<TradeRecord>(trade));
                        writer.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private byte[] TypeToByte<T>(T structure)
        {
            int size = Marshal.SizeOf(structure);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }
    }
}
