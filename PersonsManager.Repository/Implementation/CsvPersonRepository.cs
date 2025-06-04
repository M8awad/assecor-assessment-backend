using CsvHelper;
using CsvHelper.Configuration;
using PersonsManager.Data.Entities;
using PersonsManager.Data.Enum;
using PersonsManager.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PersonsManager.Repository.Implementation
{
    public class CsvPersonRepository : IPersonRepository
    {
        private string _csvFilePath;
        private List<Person> _persons;
        private int _nextId;
        private readonly Dictionary<int, string> _colorMap;

        public CsvPersonRepository(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
            _colorMap = new Dictionary<int, string>
            {
                { (int)ColorType.Blue, "blau" },
                { (int)ColorType.Green, "grün" },
                { (int)ColorType.Violet, "violett" },
                { (int)ColorType.Red, "rot" },
                { (int)ColorType.Yellow, "gelb" },
                { (int)ColorType.Turquoise, "türkis" },
                { (int)ColorType.White, "weiß" }
            };
            LoadPersonsFromCsv();
        }

        public Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            return Task.FromResult<IEnumerable<Person>>(_persons);
        }

        public Task<Person> GetPersonByIdAsync(int id)
        {
            var person = _persons.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(person);
        }

        public Task<IEnumerable<Person>> GetPersonsByColorAsync(string color)
        {
            var persons = _persons.Where(p => p.Color.ToLower() == color.ToLower());
            return Task.FromResult(persons);
        }

        public Task<Person> AddPersonAsync(Person person)
        {
            person.Id = _nextId++;
            _persons.Add(person);
            return Task.FromResult(person);
        }

        public Task<bool> DeletePersonAsync(int id)
        {
            var person = _persons.FirstOrDefault(p => p.Id == id);
            if (person == null)
                return Task.FromResult(false);

            _persons.Remove(person);
            return Task.FromResult(true);
        }

        public void ReloadCsvData(string newCsvFilePath = null)
        {
            if (!string.IsNullOrEmpty(newCsvFilePath))
            {
                _csvFilePath = newCsvFilePath;
            }
            LoadPersonsFromCsv();
        }

        // ✅ Fixed CSV parsing logic
        private void LoadPersonsFromCsv()
        {
            _persons = new List<Person>();
            _nextId = 1;

            if (!File.Exists(_csvFilePath))
            {
                return;
            }

            try
            {
                var lines = File.ReadAllLines(_csvFilePath);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var person = ParseCsvLine(line, _nextId);
                        if (person != null)
                        {
                            _persons.Add(person);
                            _nextId++;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error and continue with next line
                        Console.WriteLine($"Error parsing line: {line}. Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }
        }

        // ✅ Fixed CSV line parsing for correct format
        private Person ParseCsvLine(string line, int id)
        {
            try
            {
                // Split by comma, but handle cases where there might be spaces
                var parts = line.Split(',').Select(p => p.Trim()).ToArray();

                if (parts.Length < 4)
                {
                    Console.WriteLine($"Invalid line format (not enough parts): {line}");
                    return null;
                }

                // ✅ Extract data according to CSV format: LastName, FirstName, Address, ColorId
                string lastName = parts[0];
                string firstName = parts[1];
                string fullAddress = parts[2];

                // Parse color ID from the last part
                if (!int.TryParse(parts[3], out int colorId))
                {
                    Console.WriteLine($"Invalid color ID in line: {line}");
                    return null;
                }

                // ✅ Extract ZIP code and city from address
                var (zipCode, city) = ExtractZipAndCity(fullAddress);

                // Get color name from color ID
                string colorName = _colorMap.TryGetValue(colorId, out var color) ? color : "unknown";

                return new Person
                {
                  
                    Name = firstName,
                    LastName = lastName,
                    ZipCode = zipCode,
                    City = city,
                    Color = colorName
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing line: {line}. Error: {ex.Message}");
                return null;
            }
        }

        // ✅ Smart ZIP code and city extraction
        private (string zipCode, string city) ExtractZipAndCity(string fullAddress)
        {
            try
            {
                // Pattern to match ZIP code (5 digits) followed by city name
                // Example: "67742 Lauterecken" or "18439 Stralsund"
                var match = Regex.Match(fullAddress.Trim(), @"^(\d{5})\s+(.+)$");

                if (match.Success)
                {
                    return (match.Groups[1].Value, match.Groups[2].Value.Trim());
                }

                // If no ZIP code pattern found, try to extract any numbers at the beginning
                var numberMatch = Regex.Match(fullAddress.Trim(), @"^(\d+)\s+(.+)$");
                if (numberMatch.Success)
                {
                    return (numberMatch.Groups[1].Value, numberMatch.Groups[2].Value.Trim());
                }

                // If no pattern matches, return the full address as city
                return ("", fullAddress.Trim());
            }
            catch
            {
                return ("", fullAddress?.Trim() ?? "");
            }
        }

        public Task ClearAsync()
        {
            _persons.Clear();
            _nextId = 1;
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
            {
                person.Id = _nextId++;
                _persons.Add(person);
            }
            return Task.CompletedTask;
        }
    }
}
