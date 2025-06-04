using FluentAssertions.Execution;
using PersonsManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO; // ← ADD THIS
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonManager.Test.Helpers
{
    public static class TestDataHelper
    {
      
        public static string GetTestCsvContent()
        {
            return @"Müller,Hans,67742 Lauterecken,1
Schmidt,Anna,18439 Stralsund,2
Weber,Klaus,10115 Berlin,3";
        }

        public static List<Person> GetTestPersons()
        {
            return new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = "Hans",
                    LastName = "Müller",
                    ZipCode = "67742",
                    City = "Lauterecken",
                    Color = "blau"
                },
                new Person
                {
                    Id = 2,
                    Name = "Anna",
                    LastName = "Schmidt",
                    ZipCode = "18439",
                    City = "Stralsund",
                    Color = "grün"
                },
                new Person
                {
                    Id = 3,
                    Name = "Klaus",
                    LastName = "Weber",
                    ZipCode = "10115",
                    City = "Berlin",
                    Color = "rot"
                },
                new Person
                {
                    Id = 4,
                    Name = "John",
                    LastName = "Johnson",
                    ZipCode = "12345",
                    City = "NewYork",
                    Color = "rot"
                },
                new Person
                {
                    Id = 5,
                    Name = "Sarah",
                    LastName = "Brown",
                    ZipCode = "54321",
                    City = "London",
                    Color = "gelb"
                }
            };
        }

        public static Person GetSingleTestPerson()
        {
            return new Person
            {
                Name = "Test",
                LastName = "Person",
                ZipCode = "12345",
                City = "TestCity",
                Color = "blau"
            };
        }

        // Use this as the main CSV content method
        public static string GetValidCsvContent()
        {
                        return @"Müller,Hans,67742 Lauterecken,1
            Schmidt,Anna,18439 Stralsund,2
            Weber,Klaus,10115 Berlin,3
            Johnson,John,12345 NewYork,4
            Brown,Sarah,54321 London,5";
        }

        // Remove the old GetTestCsvContent() method to avoid confusion

        public static string GetInvalidCsvContent()
        {
            return @"InvalidLine
Incomplete,Data,1
TooMany,Parts,Here,Are,Too,Many,Parts,1
,EmptyName,12345 City,2
ValidName,,12345 City,3
ValidName,ValidLast,,4
ValidName,ValidLast,12345 City,";
        }

        public static string GetMixedValidInvalidCsvContent()
        {
            return @"Müller,Hans,67742 Lauterecken,1
InvalidLine
Schmidt,Anna,18439 Stralsund,2
Incomplete,Data
Weber,Klaus,10115 Berlin,3";
        }

        public static string GetCsvWithDifferentAddressFormats()
        {
            return @"Person1,Test1,67742 Lauterecken,1
Person2,Test2,123 ShortZip,2
Person3,Test3,NoNumberCity,3
Person4,Test4,12345678 LongNumber,4";
        }

        // Use only this method for creating temp files
        public static string CreateTempCsvFile(string content = null)
        {
            var tempPath = Path.GetTempFileName();
            var csvContent = content ?? GetValidCsvContent(); // Always use GetValidCsvContent by default
            File.WriteAllText(tempPath, csvContent);
            return tempPath;
        }

        public static void CleanupTempFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
