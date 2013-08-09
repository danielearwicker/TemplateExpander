using System.Collections.Generic;

namespace TemplateExpander
{
    public delegate bool Operator(string token, Stack<object> stack);
}