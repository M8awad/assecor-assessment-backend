using FluentAssertions;
using Moq;
using PersonManager.Test.Helpers;
using PersonsManager.Data.Entities;
using PersonsManager.Repository.Implementation;
using PersonsManager.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersonManager.Test.Repository
{
    public class PersonRepositoryTests
    {
        private readonly Mock<IBaseRepository> _mockBaseRepository;
        private readonly PersonRepository _personRepository;

        public PersonRepositoryTests()
        {
            _mockBaseRepository = new Mock<IBaseRepository>();
            _personRepository = new PersonRepository(_mockBaseRepository.Object);
        }

        [Fact]
        public async Task GetAllPersonsAsync_ShouldReturnAllPersons()
        {
            // Arrange
            var expectedPersons = TestDataHelper.GetTestPersons();
            _mockBaseRepository
                .Setup(x => x.GetAllAsync<Person>(It.IsAny<bool>()))
                .ReturnsAsync(expectedPersons);

            // Act
            var result = await _personRepository.GetAllPersonsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(expectedPersons.Count);
            result.Should().BeEquivalentTo(expectedPersons);

            // Verify the mock was called correctly
            _mockBaseRepository.Verify(x => x.GetAllAsync<Person>(It.IsAny<bool>()), Times.Once);
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
            var result = await _personRepository.GetPersonByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPerson);
            _mockBaseRepository.Verify(x => x.FirstOrDefaultAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            _mockBaseRepository
                .Setup(x => x.FirstOrDefaultAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()))
                .ReturnsAsync((Person)null);

            // Act
            var result = await _personRepository.GetPersonByIdAsync(999);

            // Assert
            result.Should().BeNull();
            _mockBaseRepository.Verify(x => x.FirstOrDefaultAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()), Times.Once);
        }


        [Fact]
        public async Task GetPersonsByColorAsync_ShouldReturnPersonsWithMatchingColor()
        {
            // Arrange
            var allPersons = TestDataHelper.GetTestPersons();
            var bluePersons = allPersons.Where(p => p.Color.ToLower() == "blau").ToList();

            _mockBaseRepository
                .Setup(x => x.GetAllWhereAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()))
                .ReturnsAsync(bluePersons);

            // Act
            var result = await _personRepository.GetPersonsByColorAsync("blau");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(bluePersons.Count);
            result.All(p => p.Color.ToLower() == "blau").Should().BeTrue();
        }

        [Fact]
        public async Task AddPersonAsync_ShouldAddPersonAndReturnIt()
        {
            // Arrange
            var newPerson = TestDataHelper.GetSingleTestPerson();
            _mockBaseRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(true);

            // Act
            var result = await _personRepository.AddPersonAsync(newPerson);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(newPerson);

            // Verify the correct sequence of calls
            _mockBaseRepository.Verify(x => x.AddAsync(newPerson), Times.Once);
            _mockBaseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        //[Fact]
        //public async Task DeletePersonAsync_WithExistingPerson_ShouldReturnTrue()
        //{
        //    // Arrange
        //    var existingPerson = TestDataHelper.GetSingleTestPerson();
        //    existingPerson.Id = 1;

        //    _mockBaseRepository
        //        .Setup(x => x.FirstOrDefaultAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()))
        //        .ReturnsAsync(existingPerson);
        //    _mockBaseRepository
        //        .Setup(x => x.SaveChangesAsync())
        //        .ReturnsAsync(true);

        //    // Act
        //    var result = await _personRepository.DeletePersonAsync(1);

        //    // Assert
        //    result.Should().BeTrue();
        //    _mockBaseRepository.Verify(x => x.Remove(existingPerson), Times.Once);
        //    _mockBaseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        //}

        //[Fact]
        //public async Task DeletePersonAsync_WithNonExistentPerson_ShouldReturnFalse()
        //{
        //    // Arrange
        //    _mockBaseRepository
        //        .Setup(x => x.FirstOrDefaultAsync<Person>(It.IsAny<Expression<Func<Person, bool>>>(), It.IsAny<bool>()))
        //        .ReturnsAsync((Person)null);

        //    // Act
        //    var result = await _personRepository.DeletePersonAsync(999);

        //    // Assert
        //    result.Should().BeFalse();
        //    _mockBaseRepository.Verify(x => x.Remove(It.IsAny<Person>()), Times.Never);
        //    _mockBaseRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        //}

    }
}

