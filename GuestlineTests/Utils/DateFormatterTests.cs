using Guestline.Utils;

namespace GuestlineTests.Utils
{
    public class DateFormatterTests
    {
        [TestCase("20240901", 2024, 9, 1)]
        [TestCase("20000101", 2000, 1, 1)]
        [TestCase("19991231", 1999, 12, 31)]
        [TestCase("20231225", 2023, 12, 25)]
        public void ConvertToDateOnly_ValidDateString_ReturnsCorrectDateOnly(string input, int year, int month, int day)
        {
            // Arrange
            DateOnly expected = new DateOnly(year, month, day);

            // Act
            DateOnly result = DateFormatter.ConvertToDateOnly(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ConvertToDateOnly_IncorrectFormat_ThrowsFormatException()
        {
            // Arrange
            string input = "2024-09-01";

            // Act && Assert
            Assert.Throws<FormatException>(() => DateFormatter.ConvertToDateOnly(input));
        }
    }
}
