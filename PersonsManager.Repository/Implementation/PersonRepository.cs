using Microsoft.EntityFrameworkCore;
using PersonsManager.Data.Entities;
using PersonsManager.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsManager.Repository.Implementation
{
    public class PersonRepository : IPersonRepository
    {
        private readonly IBaseRepository _baseRepository;

        public PersonRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            return await _baseRepository.GetAllAsync<Person>();
        }

        public async Task<Person> GetPersonByIdAsync(int id)
        {
            return await _baseRepository.FirstOrDefaultAsync<Person>(p => p.Id == id);
        }

        public async Task<IEnumerable<Person>> GetPersonsByColorAsync(string color)
        {
            return await _baseRepository.GetAllWhereAsync<Person>(p => p.Color.ToLower() == color.ToLower());
        }

        public async Task<Person> AddPersonAsync(Person person)
        {
            await _baseRepository.AddAsync(person);
            await _baseRepository.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            var person = await _baseRepository.FirstOrDefaultAsync<Person>(p => p.Id == id);
            if (person == null)
                return false;

            _baseRepository.Remove(person);
            return await _baseRepository.SaveChangesAsync();
        }

        public async Task ClearAsync()
        {
            var allPersons = await _baseRepository.GetAllAsync<Person>();
            foreach (var person in allPersons)
            {
                _baseRepository.Remove(person);
            }
            await _baseRepository.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
            {
                await _baseRepository.AddAsync(person);
            }
            await _baseRepository.SaveChangesAsync();
        }
    }
}
