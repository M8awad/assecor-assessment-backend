using PersonsManager.Model.common;
using PersonsManager.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsManager.Service.Interface
{
    public interface IPersonService
    {
        Task<ResultModel<List<PersonDto>>> GetAllPersonsAsync();
        Task<ResultModel<PersonDto>> GetPersonByIdAsync(int id);
        Task<ResultModel<List<PersonDto>>> GetPersonsByColorAsync(string color);
        Task<ResultModel<PersonDto>> AddPersonAsync(PersonDto personDto);
        Task<ResultModel> LoadCsvFileAsync(string csvFilePath);
        //Task<ResultModel<PersonDto>> UpdatePerson(PersonDto personDto);
        //Task<ResultModel> Delete(int id);
    }
}
