using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SubPickaxe
{
    public static class MyExt
    {
        public static string MyReplace(this string _string, string substring, int index)
        {
            return _string.Remove(index, substring.Length).Insert(index, substring);
        }

    }

    class Program
    {
        static string SRfication(string text)
        {
            string result = text;
            Regex r = new Regex(@"^(?<info>.*?:(?:.*?,){9})(?:(?(?=(?<text>[^\{\}""]+))\k<text>|(?(?=(?<tag>\{.*?\}))\k<tag>|(?(?=(?<lcomma>""(?=\p{L})))\k<lcomma>|(?(?=(?<rcomma>(?<=[\p{L}\p{P}])""))\k<rcomma>|(?<unrecognised>.))))))+$");
            Match m = r.Match(text);
            foreach (Capture leftCap in m.Groups["lcomma"].Captures)
                result = result.MyReplace("«", leftCap.Index);
            foreach (Capture rightCap in m.Groups["rcomma"].Captures)
                result = result.MyReplace("»", rightCap.Index);
            return
                result.Replace(" - ", " – ");
            
        }

        static void WriteASSFileInfo(StreamReader r, StreamWriter w)
        {
            string subFileLine = "";
            string lastLine = "";
            while (lastLine != "[Events]")
            {
                lastLine = subFileLine;
                subFileLine = r.ReadLine();
                w.WriteLine(subFileLine);
            }
        }

        static void Main(string[] args)
        {
            string path = args[0];
            StreamReader sReader = new StreamReader(path);
            StreamWriter sWriter = new StreamWriter(path.Remove(path.LastIndexOf('.')) + "_new.ass");
            WriteASSFileInfo(sReader, sWriter);
            while (!sReader.EndOfStream)
                sWriter.WriteLine(SRfication(sReader.ReadLine()));
            sReader.Close();
            sWriter.Close();
        }
    }
}