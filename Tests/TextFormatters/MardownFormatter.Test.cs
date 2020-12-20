using Core.TextFormatters;
using NUnit.Framework;

namespace Core.Tests.TextFormatters
{
    public class MarkdownFormatter_Test
    {
        MarkdownFormatter Formatter;

        [SetUp]
        public void Setup()
        {
            Formatter = MarkdownFormatter.Get;
        }

        [Test]
        public void Test_MarkdownFormatter_InstanceIsNotNull()
        {
            Assert.IsNotNull(Formatter);
        }

        [TestCase("The quick brown fox", "The quick **brown** fox", 10, 15, Formatting.Bold)]
        [TestCase("The quick brown fox", "The ~~quick brown~~ fox", 4, 15, Formatting.Strikethrough)]
        [TestCase("The quick brown fox", "The *quick* brown fox", 4, 9, Formatting.Italics)]
        [TestCase("The quick brown fox", "The **quick brown fox", 4, 4, Formatting.Italics)]
        public void Test_MarkdownFormatter_CanFormatText(string initial, string expected, int start, int end, Formatting type)
        {
            Formatter.Format(ref initial, start, end, type);

            Assert.AreEqual(expected, initial, "Message was not formatted correctly");
        }
    }
}