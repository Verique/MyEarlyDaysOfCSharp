using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace FolderSizeViewer
{
    class FolderInfo
    {
        public long Size
            { get; }
        public string Name
            { get; set; }
        static readonly string[] sizeUnits = { "B", "KB", "MB", "GB", "TB" };
        int currUnit = 0;
        public FolderInfo(long _size, string _name)
        {
            Name = _name;
            Size = _size;
        }

        override public string ToString()
        {
            long tmpSize = Size;
            while (tmpSize > 10240)
            {
                tmpSize /= 1024;
                currUnit++;
            }
            return tmpSize.ToString() + " " + sizeUnits[currUnit];
        }
    }


    class Program
    {
        static long GetDirectorySize(DirectoryInfo d)
        {
            long size = 0;
            foreach (DirectoryInfo dir in d.GetDirectories())
            {
                size += GetDirectorySize(dir);
            }
            foreach (FileInfo file in d.GetFiles())
            {
                size += file.Length;
            }
            return size;
        }

        static void Main(string[] args)
        {
            DirectoryInfo[] dirs = new DirectoryInfo(".").GetDirectories();
            List<FolderInfo> flInfos = new List<FolderInfo>();
            foreach (DirectoryInfo dir in dirs)
                flInfos.Add(new FolderInfo(GetDirectorySize(dir), dir.Name));

            int maxNameLength = dirs.Max(d => d.Name.Length);
            foreach (FolderInfo fInfo in from fi in flInfos
                                         orderby fi.Size descending
                                         select fi)
            {
                string dash = "─";
                for (int i = 0; i < maxNameLength - fInfo.Name.Length; i++)
                    dash += "─";
                Console.WriteLine("[{0}] {2} {1}", fInfo.Name, fInfo, dash);
            }
            Console.WriteLine("\n\n<Press any key to exit>");
            while (Console.ReadKey().Key == 0);
        }
    }
}
