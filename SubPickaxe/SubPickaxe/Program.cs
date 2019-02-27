using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SubPickaxe
{
    public static class MyExt
    {
        public static string RecursiveReplace(string input, string pattern, MatchEvaluator matchEvaluator)
        {
            string result = input;
            if (new Regex(pattern).IsMatch(input))
            {
                result = Regex.Replace(input, pattern, matchEvaluator);
                return RecursiveReplace(result, pattern, matchEvaluator);
            }
            else
            {
                return result;
            }
        }
        public static void MyReplace(this string _string, string substring, int index)
        {
            _string.Remove(index, substring.Length);
            _string.Insert(index, substring);
        }
    }

    class Program
    {
        static string SRfication(string text)
        {
            string result = text;
            Regex r = new Regex(@"^(?<info>.*?:(?:.*?,){9})(?:(?<text>[^""\{\}]*)| (?<lcomma>""(?=\p{L}))|(?<rcomma>(?<=\S)"")|(?<tag>\{.*?\}))+$");
            Match m = r.Match(text);
            foreach (Capture leftCap in m.Groups["lcomma"].Captures)
                result.MyReplace("«", leftCap.Index);
            foreach (Capture rightCap in m.Groups["lcomma"].Captures)
                result.MyReplace("»", rightCap.Index);
            return
                result.Replace(" - ", " – ");
            
        }

        static void Main(string[] args)
        {
            string path = args[0];
            int lineCounter = 0;
            StreamReader streamReader = new StreamReader(path);
            StreamWriter writer = new StreamWriter(path.Remove(path.LastIndexOf('.')) + "_new.ass");
            string subFileLine = "";
            while (subFileLine != "[Events]")
            {
                subFileLine = streamReader.ReadLine();
                writer.WriteLine(subFileLine);
                lineCounter++;
            }
            subFileLine = streamReader.ReadLine();
            writer.WriteLine(subFileLine);
            lineCounter++;
            while (!streamReader.EndOfStream)
                writer.WriteLine(SRfication(streamReader.ReadLine()));
            streamReader.Close();
            writer.Close();
        }
    }
}