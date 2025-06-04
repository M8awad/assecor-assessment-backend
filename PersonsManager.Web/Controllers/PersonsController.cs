using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonsManager.Model.common;
using PersonsManager.Model.Dtos;
using PersonsManager.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonsManager.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        public async Task<ActionResult<ResultModel<List<PersonDto>>>> GetAllPersons()
        {
            var result = await _personService.GetAllPersonsAsync();

            if (result.Saved)
            {
                return Ok(result);
            }

            return StatusCode(500, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultModel<PersonDto>>> GetPersonById(int id)
        {
            var result = await _personService.GetPersonByIdAsync(id);

            if (result.Saved)
            {
                return Ok(result);
            }

            if (result.Data == null && result.ServerMessage.Contains("not found"))
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        [HttpGet("color/{color}")]
        public async Task<ActionResult<ResultModel<List<PersonDto>>>> GetPersonsByColor(string color)
        {
            var result = await _personService.GetPersonsByColorAsync(color);

            if (result.Saved)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResultModel<PersonDto>>> AddPerson(PersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return BadRequest(new ResultModel<PersonDto>
                {
                    Data = null,
                    Saved = false,
                    ModelStateError = errors,
                    ServerMessage = "Model validation failed"
                });
            }

            var result = await _personService.AddPersonAsync(personDto);

            if (result.Saved)
            {
                return CreatedAtAction(nameof(GetPersonById), new { id = result.Data.Id }, result);
            }

            return BadRequest(result);
        }



        [HttpPost("load-csv")]
        public async Task<ActionResult<ResultModel>> LoadCsvFile(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                return BadRequest(new ResultModel
                {
                    Saved = false,
                    ServerMessage = "No file provided"
                });
            }

            try
            {
                // Create uploads directory
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                Directory.CreateDirectory(uploadsPath);

                // Save uploaded file with .csv extension regardless of original extension
                var fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_data.csv";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await csvFile.CopyToAsync(stream);
                }

                // Load the CSV data
                var result = await _personService.LoadCsvFileAsync(filePath);

                if (result.Saved)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultModel
                {
                    Saved = false,
                    ServerMessage = $"Error uploading file: {ex.Message}"
                });
            }
        }



        //[HttpPut("{id}")]
        //public async Task<ActionResult<ResultModel<PersonDto>>> UpdatePerson(int id, PersonDto personDto)
        //{
        //    if (id != personDto.Id)
        //    {
        //        return BadRequest(new ResultModel<PersonDto>
        //        {
        //            Data = null,
        //            Saved = false,
        //            ModelStateError = "ID mismatch",
        //            ServerMessage = "The ID in the URL does not match the ID in the request body"
        //        });
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        var errors = string.Join("; ", ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage));

        //        return BadRequest(new ResultModel<PersonDto>
        //        {
        //            Data = null,
        //            Saved = false,
        //            ModelStateError = errors,
        //            ServerMessage = "Model validation failed"
        //        });
        //    }

        //    var result = await _personService.UpdatePerson(personDto);

        //    if (result.Saved)
        //    {
        //        return Ok(result);
        //    }

        //    if (result.ServerMessage.Contains("not found"))
        //    {
        //        return NotFound(result);
        //    }

        //    return BadRequest(result);
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ResultModel>> DeletePerson(int id)
        //{
        //    var result = await _personService.Delete(id);

        //    if (result.Saved)
        //    {
        //        return Ok(result);
        //    }

        //    if (result.ServerMessage.Contains("not found"))
        //    {
        //        return NotFound(result);
        //    }

        //    return BadRequest(result);
        //}
    }
}