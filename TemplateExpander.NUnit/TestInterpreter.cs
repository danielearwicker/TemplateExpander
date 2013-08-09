using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TemplateExpander.NUnit
{
    [TestFixture]
    public class TestInterpreter
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unrecognized()
        {
            Interpreter.Interpret("gibberish");
        }

        [Test]
        public void Nothing()
        {
            Assert.AreEqual(0, Interpreter.Interpret("").Count);
            Assert.AreEqual(0, Interpreter.Interpret("   ").Count);
        }

        [Test]
        public void Integer()
        {
            Assert.AreEqual(52, Interpreter.Interpret("52").Pop());
            Assert.AreEqual(52, Interpreter.Interpret("  52").Pop());
            Assert.AreEqual(52, Interpreter.Interpret("52   ").Pop());
            Assert.AreEqual(52, Interpreter.Interpret("   52   ").Pop());
        }

        [Test]
        public void String()
        {
            Assert.AreEqual("Hello, world", Interpreter.Interpret("\"Hello, world\"").Pop());
            Assert.AreEqual(" Hello, world", Interpreter.Interpret("\" Hello, world\"").Pop());
            Assert.AreEqual("Hello, world ", Interpreter.Interpret("\"Hello, world \"").Pop());

            var result = Interpreter.Interpret("\"a\" \"bbbb\"   \"cc\"");
            Assert.AreEqual("cc", result.Pop());
            Assert.AreEqual("bbbb", result.Pop());
            Assert.AreEqual("a", result.Pop());
            Assert.AreEqual(0, result.Count);

            // Use two quotes to escape a quote
            result = Interpreter.Interpret("\"the \"\"quick\"\" brown fox\" \"etc.\"");
            Assert.AreEqual("etc.", result.Pop());
            Assert.AreEqual("the \"quick\" brown fox", result.Pop());
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Upper()
        {
            Assert.AreEqual("HELLO, WORLD", Interpreter.Interpret("\"Hello, world\" @upper").Pop());
        }

        [Test]
        public void Lower()
        {
            Assert.AreEqual("hello, world", Interpreter.Interpret("\"Hello, world\" @lower").Pop());
        }

        [Test]
        public void Trim()
        {
            Assert.AreEqual("Hello, world", Interpreter.Interpret("\"Hello, world\" @trim").Pop());
            Assert.AreEqual("Hello, world", Interpreter.Interpret("\"  Hello, world   \" @trim").Pop());
            Assert.AreEqual("Hello, world   ", Interpreter.Interpret("\"  Hello, world   \" @trimleft").Pop());
            Assert.AreEqual("  Hello, world", Interpreter.Interpret("\"  Hello, world   \" @trimright").Pop());
        }

        [Test]
        public void Pad()
        {
            Assert.AreEqual("00123", Interpreter.Interpret("\"123\" \"0\" 5 @padleft").Pop());
            Assert.AreEqual("123--", Interpreter.Interpret("\"123\" \"-\" 5 @padright").Pop());
            Assert.AreEqual("123", Interpreter.Interpret("\"123\" \"0\" 2 @padleft").Pop());
        }

        [Test]
        public void LeftRightMid()
        {
            Assert.AreEqual("Hello", Interpreter.Interpret("\"Hello, world\" 5 @left").Pop());
            Assert.AreEqual("world", Interpreter.Interpret("\"Hello, world\" 5 @right").Pop());
            Assert.AreEqual("lo, wor", Interpreter.Interpret("\"Hello, world\" 3 7 @mid").Pop());
        }

        [Test]
        public void ImplicitCasts()
        {
            Assert.AreEqual("Hello", Interpreter.Interpret("\"Hello, world\" \"5\" @left").Pop());
            Assert.AreEqual("00123", Interpreter.Interpret("123 0 5 @padleft").Pop());
        }

        [Test]
        public void Replace()
        {
            Assert.AreEqual("Hello and goodbye", Interpreter.Interpret("\"Hello or goodbye\" \"or\" \"and\" @replace").Pop());
        }

        [Test]
        public void ReplaceRegex()
        {
            Assert.AreEqual("Hello ? good?bye", Interpreter.Interpret("\"Hello 123 good45bye\" /\\d+ \"?\" @replace").Pop());
        }

        [Test]
        public void CustomOperator()
        {
            var vars = new Dictionary<string, string>()
                {
                    { "name", "ken" },
                    { "len", "8" }
                };

            Operator op = (token, stack) =>
                {
                    stack.Push(vars[token]);
                    return true;
                };

            Assert.AreEqual("ken", Interpreter.Interpret("name", op).Pop());
            Assert.AreEqual("KEN", Interpreter.Interpret("name @upper", op).Pop());
            Assert.AreEqual("00000KEN", Interpreter.Interpret("name @upper 0 len @padleft", op).Pop());
        }

        [Test]
        public void ControlCharacters()
        {
            Assert.AreEqual("First line\r\nsecond \"line\"", Interpreter.Interpret("\"First line\" @cr @lf \"second \" @quote \"line\" @quote @join").Pop());
        }
    }
}