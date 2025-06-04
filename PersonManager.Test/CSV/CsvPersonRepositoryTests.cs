using FluentAssertions;
using PersonManager.Test.Helpers;
using PersonsManager.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonsManager.Data.Entities; 

namespace PersonManager.Test.CSV
{
    public class CsvPersonRepositoryTests : IDisposable
    {
        private string _tempCsvFile;
        private CsvPersonRepository _csvRepository;

        public CsvPersonRepositoryTests()
        {
            // This will be set up in each test
        }

        [Fact]
        public async Task Constructor_WithValidCsvFile_ShouldLoadPersonsCorrectly()
        {
            // Arrange
            _tempCsvFile = TestDataHelper.CreateTempCsvFile(); // This now uses GetValidCsvContent() with 5 persons

            // Act
            _csvRepository = new CsvPersonRepository(_tempCsvFile);
            var persons = await _csvRepository.GetAllPersonsAsync();

            // Assert
            persons.Should().NotBeNull();
            persons.Should().HaveCount(5); // Now correctly expects 5 persons

            var personsList = persons.ToList();

            // Test first person
            var firstPerson = personsList.First(p => p.LastName == "Müller");
            firstPerson.Name.Should().Be("Hans");
            firstPerson.ZipCode.Should().Be("67742");
            firstPerson.City.Should().Be("Lauterecken");
            firstPerson.Color.Should().Be("blau"); // Color ID 1 = blau

            // Test second person
            var secondPerson = personsList.First(p => p.LastName == "Schmidt");
            secondPerson.Name.Should().Be("Anna");
            secondPerson.ZipCode.Should().Be("18439");
            secondPerson.City.Should().Be("Stralsund");
            secondPerson.Color.Should().Be("grün"); // Color ID 2 = grün
        }


        [Fact]
        public async Task Constructor_WithNonExistentFile_ShouldCreateEmptyRepository()
        {
            // Arrange
            var nonExistentFile = "nonexistent.csv";

            // Act
            _csvRepository = new CsvPersonRepository(nonExistentFile);
            var persons = await _csvRepository.GetAllPersonsAsync();

            // Assert
            persons.Should().NotBeNull();
            persons.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersonsAsync_WithValidData_ShouldReturnAllPersons()
        {
            // Arrange
            _tempCsvFile = TestDataHelper.CreateTempCsvFile();
            _csvRepository = new CsvPersonRepository(_tempCsvFile);

            // Act
            var persons = await _csvRepository.GetAllPersonsAsync();
            var personsList = persons.ToList();

            // Assert
            persons.Should().HaveCount(5);

          

            // Instead, verify the actual data that matters
            personsList.Should().Contain(p => p.LastName == "Müller" && p.Name == "Hans");
            personsList.Should().Contain(p => p.LastName == "Schmidt" && p.Name == "Anna");
            personsList.Should().Contain(p => p.LastName == "Weber" && p.Name == "Klaus");
            personsList.Should().Contain(p => p.LastName == "Johnson" && p.Name == "John");
            personsList.Should().Contain(p => p.LastName == "Brown" && p.Name == "Sarah");

            // Verify all persons have required data
            personsList.All(p => !string.IsNullOrEmpty(p.Name)).Should().BeTrue();
            personsList.All(p => !string.IsNullOrEmpty(p.LastName)).Should().BeTrue();
            personsList.All(p => !string.IsNullOrEmpty(p.Color)).Should().BeTrue();
        }


        [Fact]
        public async Task ColorMapping_ShouldMapCorrectly()
        {
            // Arrange
            var csvContent = @"Person1,Test1,12345 City,1
Person2,Test2,12345 City,2
Person3,Test3,12345 City,3
Person4,Test4,12345 City,4
Person5,Test5,12345 City,5
Person6,Test6,12345 City,6
Person7,Test7,12345 City,7
Person8,Test8,12345 City,99"; // Invalid color ID

            _tempCsvFile = TestDataHelper.CreateTempCsvFile(csvContent);
            _csvRepository = new CsvPersonRepository(_tempCsvFile);

            // Act
            var persons = (await _csvRepository.GetAllPersonsAsync()).ToList();

            // Assert
            persons.Should().HaveCount(8);

            persons[0].Color.Should().Be("blau");     // ID 1
            persons[1].Color.Should().Be("grün");     // ID 2
            persons[2].Color.Should().Be("violett");  // ID 3
            persons[3].Color.Should().Be("rot");      // ID 4
            persons[4].Color.Should().Be("gelb");     // ID 5
            persons[5].Color.Should().Be("türkis");   // ID 6
            persons[6].Color.Should().Be("weiß");     // ID 7
            persons[7].Color.Should().Be("unknown");  // Invalid ID 99
        }

        [Fact]
        public async Task AddressExtraction_ShouldParseCorrectly()
        {
            // Arrange
            var csvContent = TestDataHelper.GetCsvWithDifferentAddressFormats();
            _tempCsvFile = TestDataHelper.CreateTempCsvFile(csvContent);
            _csvRepository = new CsvPersonRepository(_tempCsvFile);

            // Act
            var persons = (await _csvRepository.GetAllPersonsAsync()).ToList();

            // Assert
            persons.Should().HaveCount(4);

            // Standard 5-digit ZIP
            var person1 = persons.First(p => p.LastName == "Person1");
            person1.ZipCode.Should().Be("67742");
            person1.City.Should().Be("Lauterecken");

            // Short ZIP
            var person2 = persons.First(p => p.LastName == "Person2");
            person2.ZipCode.Should().Be("123");
            person2.City.Should().Be("ShortZip");

            // No number in address
            var person3 = persons.First(p => p.LastName == "Person3");
            person3.ZipCode.Should().Be("");
            person3.City.Should().Be("NoNumberCity");

            // Long number
            var person4 = persons.First(p => p.LastName == "Person4");
            person4.ZipCode.Should().Be("12345678");
            person4.City.Should().Be("LongNumber");
        }

        [Fact]
        public async Task InvalidCsvLines_ShouldBeSkippedGracefully()
        {
            // Arrange
            var csvContent = TestDataHelper.GetMixedValidInvalidCsvContent();
            _tempCsvFile = TestDataHelper.CreateTempCsvFile(csvContent);
            _csvRepository = new CsvPersonRepository(_tempCsvFile);

            // Act
            var persons = (await _csvRepository.GetAllPersonsAsync()).ToList();

            // Assert - Only valid lines should be parsed
            persons.Should().HaveCount(3); // Only 3 valid lines

            persons.Should().Contain(p => p.LastName == "Müller" && p.Name == "Hans");
            persons.Should().Contain(p => p.LastName == "Schmidt" && p.Name == "Anna");
            persons.Should().Contain(p => p.LastName == "Weber" && p.Name == "Klaus");
        }


        public void Dispose()
        {
            if (!string.IsNullOrEmpty(_tempCsvFile))
            {
                TestDataHelper.CleanupTempFile(_tempCsvFile);
            }
        }
    }
}
