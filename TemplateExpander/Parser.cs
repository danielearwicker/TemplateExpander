using System.Collections.Generic;

namespace TemplateExpander
{
    public static class Parser
    {
        public static IEnumerable<string> Parse(string input)
        {
            var start = 0;
            var inQuotes = false;
            for (var pos = 0; pos < input.Length; pos++)
            {
                var c = input[pos];
                if (inQuotes)
                {
                    if (c == Constants.Quote)
                    {
                        // If immediately followed by another quote, skip and ignore
                        if ((pos + 1) < input.Length && input[pos + 1] == Constants.Quote)
                            pos++;
                        else
                            inQuotes = false;
                    }
                }
                else
                {
                    if (c == Constants.Quote)
                        inQuotes = true;
                    else if (char.IsWhiteSpace(c))
                    {
                        var token = input.Substring(start, pos - start).Trim();
                        if (token.Length != 0)
                            yield return token;

                        start = pos + 1;
                    }
                }
            }
            var end = input.Substring(start).Trim();
            if (end.Length != 0)
                yield return end;
        }
    }
}