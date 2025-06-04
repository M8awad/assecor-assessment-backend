using PersonsManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsManager.Repository.Interface
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllPersonsAsync();
        Task<Person> GetPersonByIdAsync(int id);
        Task<IEnumerable<Person>> GetPersonsByColorAsync(string color);
        Task<Person> AddPersonAsync(Person person);
        Task<bool> DeletePersonAsync(int id); 
        Task ClearAsync();
        Task AddRangeAsync(IEnumerable<Person> persons);
    }
}
