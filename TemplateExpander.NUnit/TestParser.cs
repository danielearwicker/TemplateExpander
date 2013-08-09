using System.Linq;
using NUnit.Framework;

namespace TemplateExpander.NUnit
{
    [TestFixture]
    public class TestParser
    {
        public static void Check(string input, params string[] expected)
        {
            Assert.IsTrue(Parser.Parse(input).SequenceEqual(expected));
        }

        [Test]
        public void Simple()
        {
            Check("");
            Check("a", "a");
            Check("a b", "a", "b");
            Check("a  b", "a", "b");
            Check(" a  b ", "a", "b");
            Check(" a    b c", "a", "b", "c");
        }

        [Test]
        public void Strings()
        {
            Check("\"Quick brown fox\"", "\"Quick brown fox\"");
            Check("\"Quick brown fox\" jumped \"over the lazy\" dog", "\"Quick brown fox\"", "jumped", "\"over the lazy\"", "dog");            
        }
    }
}
