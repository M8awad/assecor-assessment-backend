using PersonsManager.Data.Entities;
using PersonsManager.Model.common;
using PersonsManager.Model.Dtos;
using PersonsManager.Repository.Implementation;
using PersonsManager.Repository.Interface;
using PersonsManager.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsManager.Service.Implementation
{
    // In your PersonService - Use IBaseRepository directly
    public class PersonService : IPersonService
    {
        private readonly IBaseRepository _baseRepository;

        public PersonService(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<ResultModel<List<PersonDto>>> GetAllPersonsAsync()
        {
            try
            {
                var persons = await _baseRepository.GetAllAsync<Person>();
                var result = persons.Select(MapToDto).OrderBy(x => x.Id).ToList();

                return new ResultModel<List<PersonDto>>
                {
                    Data = result,
                    Saved = true,
                    ServerMessage = $"Successfully retrieved {result.Count} persons"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<PersonDto>>
                {
                    Data = new List<PersonDto>(),
                    Saved = false,
                    ServerMessage = $"Error retrieving persons: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<PersonDto>> GetPersonByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new ResultModel<PersonDto>
                    {
                        Data = null,
                        Saved = false,
                        ServerMessage = "Invalid ID provided",
                        ModelStateError = "ID must be greater than 0"
                    };
                }

                var person = await _baseRepository.FirstOrDefaultAsync<Person>(p => p.Id == id);
                if (person == null)
                {
                    return new ResultModel<PersonDto>
                    {
                        Data = null,
                        Saved = false,
                        ServerMessage = $"Person with ID {id} not found"
                    };
                }

                var personDto = MapToDto(person);

                return new ResultModel<PersonDto>
                {
                    Data = personDto,
                    Saved = true,
                    ServerMessage = $"Successfully retrieved person: {personDto.Name} {personDto.LastName}",
                    ID = personDto.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<PersonDto>
                {
                    Data = null,
                    Saved = false,
                    ServerMessage = $"Error retrieving person with ID {id}: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<List<PersonDto>>> GetPersonsByColorAsync(string color)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(color))
                {
                    return new ResultModel<List<PersonDto>>
                    {
                        Data = new List<PersonDto>(),
                        Saved = false,
                        ServerMessage = "Color parameter is required",
                        ModelStateError = "Color cannot be null or empty"
                    };
                }

                var persons = await _baseRepository.GetAllWhereAsync<Person>(p => p.Color.ToLower() == color.ToLower());
                var result = persons.Select(MapToDto).OrderBy(x => x.Id).ToList();

                return new ResultModel<List<PersonDto>>
                {
                    Data = result,
                    Saved = true,
                    ServerMessage = $"Successfully retrieved {result.Count} persons with color '{color}'"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<PersonDto>>
                {
                    Data = new List<PersonDto>(),
                    Saved = false,
                    ServerMessage = $"Error retrieving persons by color '{color}': {ex.Message}"
                };
            }
        }

        public async Task<ResultModel<PersonDto>> AddPersonAsync(PersonDto personDto)
        {
            try
            {
                if (personDto == null)
                {
                    return new ResultModel<PersonDto>
                    {
                        Data = null,
                        Saved = false,
                        ServerMessage = "Person data is required",
                        ModelStateError = "PersonDto cannot be null"
                    };
                }

                // Check if person with same name and lastname already exists
                var existingPersons = await _baseRepository.GetAllAsync<Person>();
                var nameExists = existingPersons.Any(x => x.Name.ToLower() == personDto.Name.ToLower() &&
                                                         x.LastName.ToLower() == personDto.LastName.ToLower());

                if (nameExists)
                {
                    return new ResultModel<PersonDto>
                    {
                        Data = null,
                        Saved = false,
                        ServerMessage = "Person with this name and last name already exists"
                    };
                }

                var person = MapToEntity(personDto);
                await _baseRepository.AddAsync(person);
                await _baseRepository.SaveChangesAsync();

                var resultDto = MapToDto(person);

                return new ResultModel<PersonDto>
                {
                    Data = resultDto,
                    Saved = true,
                    ServerMessage = $"Person '{resultDto.Name} {resultDto.LastName}' created successfully",
                    ID = person.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<PersonDto>
                {
                    Data = null,
                    Saved = false,
                    ServerMessage = $"Error creating person: {ex.Message}"
                };
            }
        }

        //public async Task<ResultModel<PersonDto>> UpdatePerson(PersonDto personDto)
        //{
        //    try
        //    {
        //        if (personDto == null || personDto.Id <= 0)
        //        {
        //            return new ResultModel<PersonDto>
        //            {
        //                Data = null,
        //                Saved = false,
        //                ServerMessage = "Invalid person data",
        //                ModelStateError = "PersonDto and ID are required"
        //            };
        //        }

        //        var existingPerson = await _baseRepository.FirstOrDefaultAsync<Person>(p => p.Id == personDto.Id);

        //        if (existingPerson == null)
        //        {
        //            return new ResultModel<PersonDto>
        //            {
        //                Data = null,
        //                Saved = false,
        //                ServerMessage = $"Person with ID {personDto.Id} not found"
        //            };
        //        }

        //        // Update properties
        //        existingPerson.Name = personDto.Name;
        //        existingPerson.LastName = personDto.LastName;
        //        existingPerson.ZipCode = personDto.ZipCode;
        //        existingPerson.City = personDto.City;
        //        existingPerson.Color = personDto.Color;

        //        _baseRepository.Update(existingPerson);
        //        await _baseRepository.SaveChangesAsync();

        //        var updatedDto = MapToDto(existingPerson);

        //        return new ResultModel<PersonDto>
        //        {
        //            Data = updatedDto,
        //            Saved = true,
        //            ServerMessage = $"Person '{updatedDto.Name} {updatedDto.LastName}' updated successfully",
        //            ID = existingPerson.Id.ToString()
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultModel<PersonDto>
        //        {
        //            Data = null,
        //            Saved = false,
        //            ServerMessage = $"Error updating person: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<ResultModel> Delete(int id)
        //{
        //    try
        //    {
        //        if (id <= 0)
        //        {
        //            return new ResultModel
        //            {
        //                Saved = false,
        //                ServerMessage = "Invalid ID provided",
        //                ModelStateError = "ID must be greater than 0"
        //            };
        //        }

        //        var person = await _baseRepository.FirstOrDefaultAsync<Person>(p => p.Id == id);

        //        if (person == null)
        //        {
        //            return new ResultModel
        //            {
        //                Saved = false,
        //                ServerMessage = $"Person with ID {id} not found"
        //            };
        //        }

        //        _baseRepository.Remove(person);
        //        var deleted = await _baseRepository.SaveChangesAsync();

        //        if (deleted)
        //        {
        //            return new ResultModel
        //            {
        //                Saved = true,
        //                ServerMessage = $"Person '{person.Name} {person.LastName}' deleted successfully",
        //                ID = person.Id.ToString()
        //            };
        //        }
        //        else
        //        {
        //            return new ResultModel
        //            {
        //                Saved = false,
        //                ServerMessage = "Failed to delete person"
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultModel
        //        {
        //            Saved = false,
        //            ServerMessage = $"Error deleting person with ID {id}: {ex.Message}"
        //        };
        //    }
        //}

        private PersonDto MapToDto(Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                Name = person.Name,
                LastName = person.LastName,
                ZipCode = person.ZipCode,
                City = person.City,
                Color = person.Color
            };
        }

        private Person MapToEntity(PersonDto personDto)
        {
            return new Person
            {
                Name = personDto.Name,
                LastName = personDto.LastName,
                ZipCode = personDto.ZipCode,
                City = personDto.City,
                Color = personDto.Color
            };
        }





        public async Task<ResultModel> LoadCsvFileAsync(string csvFilePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(csvFilePath))
                {
                    return new ResultModel { Saved = false, ServerMessage = "CSV file path is required" };
                }

                if (!File.Exists(csvFilePath))
                {
                    return new ResultModel { Saved = false, ServerMessage = $"CSV file not found: {csvFilePath}" };
                }

                // 1. Parse CSV to Person objects
                var persons = await ParseCsvFileAsync(csvFilePath);

                // 2. Clear existing data
                //await ClearAllPersonsAsync();

                // 3. Add new persons
                await AddPersonsRangeAsync(persons);

                return new ResultModel
                {
                    Saved = true,
                    ServerMessage = $"Successfully imported {persons.Count} records"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Saved = false,
                    ServerMessage = $"Error loading CSV file: {ex.Message}"
                };
            }
        }

        private async Task<List<Person>> ParseCsvFileAsync(string filePath)
        {
            var persons = new List<Person>();
            var lines = await File.ReadAllLinesAsync(filePath);

            // Reuse your CSV parsing logic
            var csvRepo = new CsvPersonRepository(filePath);
            return (await csvRepo.GetAllPersonsAsync()).ToList();
        }

      

        private async Task AddPersonsRangeAsync(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
            {
                await _baseRepository.AddAsync(person);
            }
            await _baseRepository.SaveChangesAsync();
        }

    }

}
