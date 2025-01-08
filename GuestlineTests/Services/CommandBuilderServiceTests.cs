using FluentAssertions;
using Guestline.Models.Commands;
using Guestline.Services;

namespace GuestlineTests.Services
{
    public class CommandBuilderServiceTests
    {
        private CommandBuilderService _buildCommandSerivce;

        [SetUp]
        public void SetUp()
        {
            _buildCommandSerivce = new CommandBuilderService();
        }

        [Test]
        public void BuildSearchCommand_Test()
        {
            //Arrange
            var input = "Search(H1, 365, SGL)";
            var expectedResult = new SearchCommand()
            {
                DaysInterval = 365,
                HotelId = "H1",
                RoomCode = "SGL",
            };

            //Act
            var result = _buildCommandSerivce.BuildCommand(input);

            //Assert
            result.Value.Should().NotBeNull();
            result.Error.Should().BeNull();
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void BuildAvailabilityCommand_Test()
        {
            //Arrange
            var input = "Availability(H1, 20240901, SGL)";
            var exptectedResult = new AvailabilityCommand()
            {
                HotelId = "H1",
                DateRange = new DateOnlyRange()
                {
                    StartDate = new DateOnly(2024, 09, 01),
                    EndDate = new DateOnly(2024, 09, 01),
                },
                RoomCode = "SGL"
            };

            //Act
            var result = _buildCommandSerivce.BuildCommand(input);

            //Assert
            result.Value.Should().NotBeNull();
            result.Error.Should().BeNull();
            result.Value.Should().BeEquivalentTo(exptectedResult);
        }

        [Test]
        public void BuildAvailabilityCommand_DateRange_Test()
        {
            //Arrange
            var input = "Availability(H1, 20240901-20240903, DBL)";
            var exptectedResult = new AvailabilityCommand()
            {
                HotelId = "H1",
                DateRange = new DateOnlyRange()
                {
                    StartDate = new DateOnly(2024, 09, 01),
                    EndDate = new DateOnly(2024, 09, 03),
                },
                RoomCode = "DBL"
            };

            //Act
            var result = _buildCommandSerivce.BuildCommand(input);

            //Assert
            result.Value.Should().NotBeNull();
            result.Error.Should().BeNull();
            result.Value.Should().BeEquivalentTo(exptectedResult);
        }
    }
}
