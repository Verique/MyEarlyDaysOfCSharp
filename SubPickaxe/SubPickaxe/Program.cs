using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SubPickaxe
{
    public static class RegexExt
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
    }

    class Commentary
    {
        string content;

        public int Index { get; }

        public Commentary(int _index, string _content)
        {
            Index = _index;
            content = _content;
        }

        public override string ToString()
        {
            return content;
        }
    }

    class SubLine
    {
        string info;
        string text;
        List<Commentary> comments;


        public SubLine(string _info, string _text, List<Commentary> _comms)
        {
            info = _info;
            text = _text;
            comments = _comms;
        }

        public override string ToString()
        {
            string actualText = text;
            foreach (Commentary comm in comments)
            {
                actualText = actualText.Insert(comm.Index, comm.ToString());
            }
            return info + actualText;
        }
    }

    class Program
    {
        static void ParseASSLine(string line, out string info, out string text)
        {
            GroupCollection lineGroups = new Regex(@"(?<info>Dialogue:(.*?,){9})(?<text>.*)").Match(line).Groups;
            info = lineGroups["info"].Value;
            text = lineGroups["text"].Value;
        }

        static List<Commentary> ParseCommentaries(string text, Regex regExpr)
        {
            List<Commentary> comms = new List<Commentary>();
            foreach (Match mat in regExpr.Matches(text))
            {
                Group gr = mat.Groups["comm"];
                comms.Add(new Commentary(gr.Index, gr.Value));
            }
            return comms;
        }

        static string SRfication(string text)
        {
            return
                RegexExt
                .RecursiveReplace(text, @"""(?<stuff>\S.*?\S)""", new MatchEvaluator((Match m) => '«' + m.Groups["stuff"].Value + '»'))
                .Replace(" - ", " – ");
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
            {
                subFileLine = streamReader.ReadLine();
                ParseASSLine(subFileLine, out string info, out string text);
                Regex regExpr = new Regex(@"(?<comm>{.*?})");
                List<Commentary> comms = ParseCommentaries(text, regExpr);
                text = SRfication(regExpr.Replace(text, ""));
                string line = new SubLine(info, text, comms).ToString();
                writer.WriteLine(line);
            }
            streamReader.Close();
            writer.Close();
        }
    }
}