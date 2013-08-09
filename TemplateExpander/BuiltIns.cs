using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TemplateExpander
{
    public static class BuiltIns
    {
        // Built-in literal types

        public static bool IntegerLiteral(string token, Stack<object> stack)
        {
            int i;
            if (!Int32.TryParse(token, out i))
                return false;

            stack.Push(i);
            return true;
        }

        private static readonly string OneQuote = new string(Constants.Quote, 1);
        private static readonly string TwoQuote = new string(Constants.Quote, 2);

        public static bool StringLiteral(string token, Stack<object> stack)
        {
            if (token.Length >= 2 && 
                token[0] == Constants.Quote && 
                token[token.Length - 1] == Constants.Quote)
            {
                stack.Push(token.Substring(1, token.Length - 2).Replace(TwoQuote, OneQuote));
                return true;
            }
            return false;
        }

        public static bool RegexLiteral(string token, Stack<object> stack)
        {
            if (token[0] == '/')
            {
                stack.Push(new Regex(token.Substring(1), RegexOptions.CultureInvariant));
                return true;
            }
            return false;
        }

        // Some of the more involved built-in operators

        public static void Join(Stack<object> stack)
        {
            var joined = String.Join(String.Empty, stack.Reverse().Select(t => t.ToString()).ToArray());
            stack.Clear();
            stack.Push(joined);
        }

        public static string Mid(string str, int start, int length)
        {
            return start > str.Length
                       ? String.Empty
                       : start + length >= str.Length
                             ? str.Substring(start)
                             : str.Substring(start, length);
        }

        public static string Replace(string str, object find, string replaceWith)
        {
            var pat = find as Regex;
            if (pat != null)
                return pat.Replace(str, replaceWith);
            var fstr = find as string;
            if (fstr != null)
                return str.Replace(fstr, replaceWith);

            throw new InvalidOperationException("2nd parameter must be String or Regex");
        }

        public static string Match(string str, Regex regex)
        {
            var match = regex.Match(str);
            if (!match.Success)
                throw new InvalidOperationException("Input did not match Regex pattern");
            if (match.Groups.Count != 2)
                throw new InvalidOperationException("Regex did not define one group");
            return match.Groups[1].Value;
        }

        // All the built-in operators

        public static IEnumerable<Operator> All = new List<Operator>
            {
                IntegerLiteral,
                StringLiteral,
                RegexLiteral,
                Operators.MakeOperator("quote", "\""),
                Operators.MakeOperator("cr", "\r"),
                Operators.MakeOperator("lf", "\n"),
                Operators.MakeOperator("tab", "\t"),
                Operators.MakeOperator("join", Join),
                Operators.MakeOperator<string, string>("upper", str => str.ToUpperInvariant()),
                Operators.MakeOperator<string, string>("lower", str => str.ToLowerInvariant()),
                Operators.MakeOperator<string, string>("trim", str => str.Trim()),
                Operators.MakeOperator<string, string>("trimleft", str => str.TrimStart()),
                Operators.MakeOperator<string, string>("trimright", str => str.TrimEnd()),
                Operators.MakeOperator<string, string, int, string>("padleft", (str, padChar, length) => str.PadLeft(length, padChar[0])),
                Operators.MakeOperator<string, string, int, string>("padright", (str, padChar, length) => str.PadRight(length, padChar[0])),
                Operators.MakeOperator<string, int, string>("left", (str, length) => length >= str.Length ? str : str.Substring(0, length)),
                Operators.MakeOperator<string, int, string>("right", (str, length) => length >= str.Length ? str : str.Substring(str.Length - length, length)),
                Operators.MakeOperator<string, int, int, string>("mid", Mid),
                Operators.MakeOperator<string, object, string, string>("replace", Replace),
                Operators.MakeOperator<string, Regex, string>("match", Match),
            };
    }
}