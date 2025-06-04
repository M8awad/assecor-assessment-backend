using FluentAssertions;
using Moq;
using PersonManager.Test.Helpers;
using PersonsManager.Data.Entities;
using PersonsManager.Repository.Interface;
using PersonsManager.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersonManager.Test.Service
{
    public class PersonServiceTests
    {
        private readonly Mock<IBaseRepository> _mockBaseRepository;
        private readonly PersonService _personService;

        public PersonServiceTests()
        {
            _mockBaseRepository = new Mock<IBaseRepository>();
            _personService = new PersonService(_mockBaseRepository.Object);
        }

        [Fact]
        public async Task GetAllPersonsAsync_ShouldReturnSuccessResultWithAllPersons()
        {
            // Arrange
            var expectedPersons = TestDataHelper.GetTestPersons();
            _mockBaseRepository
                .Setup(x => x.GetAllAsync<Person>(true)) // Explicitly provide the boolean value
                .ReturnsAsync(expectedPersons);

            // Act
            var result = await _personService.GetAllPersonsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(expectedPersons.Count);
            result.ServerMessage.Should().Contain("Successfully");

            _mockBaseRepository.Verify(x => x.GetAllAsync<Person>(true), Times.Once);
        }

        [Fact]
        public async Task GetAllPersonsAsync_WhenRepositoryThrows_ShouldReturnErrorResult()
        {
            // Arrange
            _mockBaseRepository
                .Setup(x => x.GetAllAsync<Person>(true)) // Explicitly provide the boolean value
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _personService.GetAllPersonsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();
            result.ServerMessage.Should().Contain("Error");
        }


        [Fact]
        public async Task GetPersonByIdAsync_WithValidId_ShouldReturnCorrectPerson()
        {
            // Arrange
            var expectedPerson = TestDataHelper.GetSingleTestPerson();
            expectedPerson.Id = 1;

            _mockBaseRepository
                .Setup(x => x.FirstOrDefaultAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()))
                .ReturnsAsync(expectedPerson);

            // Act
            var result = await _personService.GetPersonByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(1);
        }



        [Fact]
        public async Task GetPersonByIdAsync_WithInvalidId_ShouldReturnErrorResult()
        {
            // Act
            var result = await _personService.GetPersonByIdAsync(0);

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ServerMessage.Should().Be("Invalid ID provided");
            result.ModelStateError.Should().Be("ID must be greater than 0");

            // Verify repository was never called
            _mockBaseRepository.Verify(x => x.FirstOrDefaultAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()), Times.Never);
        }



        [Fact]
        public async Task GetPersonByIdAsync_WithNegativeId_ShouldReturnErrorResult()
        {
            // Act
            var result = await _personService.GetPersonByIdAsync(-1);

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ServerMessage.Should().Be("Invalid ID provided");
        }



        [Fact]
        public async Task GetPersonByIdAsync_WithNonExistentId_ShouldReturnErrorResult()
        {
            // Arrange
            _mockBaseRepository
                .Setup(x => x.FirstOrDefaultAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()))
                .ReturnsAsync((Person)null);

            // Act
            var result = await _personService.GetPersonByIdAsync(999);

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ServerMessage.Should().Be("Person with ID 999 not found");
        }




        [Fact]
        public async Task GetPersonByIdAsync_WhenRepositoryThrows_ShouldReturnErrorResult()
        {
            // Arrange
            _mockBaseRepository
                .Setup(x => x.FirstOrDefaultAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _personService.GetPersonByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ServerMessage.Should().Contain("Error retrieving person with ID 1");
        }



        [Fact]
        public async Task LoadCsvFileAsync_WithEmptyPath_ShouldReturnErrorResult()
        {
            // Act
            var result = await _personService.LoadCsvFileAsync("");

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeFalse();
            result.ServerMessage.Should().Be("CSV file path is required");
        }

        [Fact]
        public async Task LoadCsvFileAsync_WithNullPath_ShouldReturnErrorResult()
        {
            // Act
            var result = await _personService.LoadCsvFileAsync(null);

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeFalse();
            result.ServerMessage.Should().Be("CSV file path is required");
        }

        [Fact]
        public async Task LoadCsvFileAsync_WithNonExistentFile_ShouldReturnErrorResult()
        {
            // Act
            var result = await _personService.LoadCsvFileAsync("nonexistent.csv");

            // Assert
            result.Should().NotBeNull();
            result.Saved.Should().BeFalse();
            result.ServerMessage.Should().Contain("CSV file not found");
        }


    }
}
