using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplateExpander
{
    public static class Interpreter
    {
        public static Stack<object> Interpret(IEnumerable<string> tokens, params Operator[] operators)
        {
            operators = BuiltIns.All.Concat(operators).ToArray();

            var stack = new Stack<object>();

            foreach (var token in tokens.Where(token => !operators.Any(op => op(token, stack))))
                throw new InvalidOperationException("Could not understand: " + token);

            return stack;
        }

        public static Stack<object> Interpret(string input, params Operator[] operators)
        {
            return Interpret(Parser.Parse(input), operators);
        }
    }
}