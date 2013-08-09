using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplateExpander
{
    public static class Constants
    {
        public const string NamedOperatorPrefix = "@";

        public const char Quote = '\"';
    }

    public static class Operators
    {
        public static Operator MakeOperator(string name, Action<Stack<object>> op)
        {
            return (word, stack) =>
            {
                if (string.Compare(word, Constants.NamedOperatorPrefix + name, StringComparison.OrdinalIgnoreCase) != 0)
                    return false;

                try
                {
                    op(stack);
                }
                catch (Exception x)
                {
                    throw new InvalidOperationException("Error from " + Constants.NamedOperatorPrefix + name + ": " + x.Message);
                }

                return true;
            };
        }

        // Turns any delegate into an action that operates on the stack
        private static Action<Stack<object>> WrapDelegate(Delegate del)
        {
            var takes = del.Method.GetParameters();
            var returns = del.Method.ReturnType;
            return stack =>
            {
                // Pop the input values and check there's enough available
                var vals = new List<object>();
                for (var n = 0; n < takes.Count(); n++)
                {
                    if (stack.Count == 0)
                        throw new InvalidOperationException("Not enough inputs, need: " + 
                            string.Join(", ", takes.Select(t => t.Name).ToArray()));
                    vals.Add(stack.Pop());
                }
                vals.Reverse(); // They came off the stack backwards

                for (var n = 0; n < takes.Count(); n++)
                {
                    // Do type coercion where necessary/possible
                    if (!takes[n].ParameterType.IsInstanceOfType(vals[n]))
                    {
                        try
                        {
                            vals[n] = Convert.ChangeType(vals[n], takes[n].ParameterType);
                        }
                        catch (Exception)
                        {
                            throw new InvalidOperationException("Input " + vals[n] + 
                                " is wrong type, should be " + takes[n].ParameterType.Name);    
                        }
                    }
                }

                var retVal = del.DynamicInvoke(vals.ToArray());

                // Return value goes back on the stack
                if (returns != typeof(void))
                    stack.Push(retVal);
            };
        }

        // Some typed wrappers for convenience
        public static Operator MakeOperator<TOut>(string name, Func<TOut> op)
        {
            return MakeOperator(name, WrapDelegate(op));
        }

        public static Operator MakeOperator<TOut>(string name, TOut constantValue)
        {
            return MakeOperator(name, () => constantValue);
        }

        public static Operator MakeOperator<TIn1, TOut>(string name, Func<TIn1, TOut> op)
        {
            return MakeOperator(name, WrapDelegate(op));
        }

        public static Operator MakeOperator<TIn1, TIn2, TOut>(string name, Func<TIn1, TIn2, TOut> op)
        {
            return MakeOperator(name, WrapDelegate(op));
        }

        public static Operator MakeOperator<TIn1, TIn2, TIn3, TOut>(string name, Func<TIn1, TIn2, TIn3, TOut> op)
        {
            return MakeOperator(name, WrapDelegate(op));
        }
    }
}
