
using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpravkiController : ControllerBase
    {
        private static List<Spravka> _spravki = new List<Spravka>();

        public static List<Spravka> GetAllSpravki()
        {
            return _spravki;
        }

        // Получить все справки
        [HttpGet]
        [ProducesResponseType(typeof(List<Spravka>), 200)]
        public List<Spravka> GetAll()
        {
            return _spravki;
        }

        // Получить справку по ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Spravka), 200)]
        [ProducesResponseType(404)]
        public ActionResult<Spravka> GetById(int id)
        {
            Spravka? spravka = _spravki.FirstOrDefault(s => s.Id == id);

            if (spravka == null)
                return NotFound();

            return spravka;
        }

        // Добавить справку
        [HttpPost]
        [ProducesResponseType(typeof(Spravka), 201)]
        [ProducesResponseType(400)]
        public ActionResult<Spravka> Create(Spravka spravka)
        {
            Patient? patient = PatientsController.Patients
                .FirstOrDefault(p => p.Id == spravka.PatientId);

            if (patient == null)
                return BadRequest("Пациент не существует.");

            SpravkaType? type = SpravkaTypesController.Types
                .FirstOrDefault(t => t.Id == spravka.SpravkaTypeId);

            if (type == null)
                return BadRequest("Вид справки не существует.");

            spravka.Id = _spravki.Count == 0
                ? 1
                : _spravki.Max(s => s.Id) + 1;

            spravka.Name = type.Name;
            spravka.Price = type.Price;

            _spravki.Add(spravka);

            return CreatedAtAction(
                nameof(GetById),
                new { id = spravka.Id },
                spravka
            );
        }

        // Редактировать справку
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Spravka), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Spravka> Update(int id, Spravka updatedSpravka)
        {
            Spravka? spravka = _spravki.FirstOrDefault(s => s.Id == id);

            if (spravka == null)
                return NotFound();

            Patient? patient = PatientsController.Patients
                .FirstOrDefault(p => p.Id == updatedSpravka.PatientId);

            if (patient == null)
                return BadRequest("Пациент не существует.");

            SpravkaType? type = SpravkaTypesController.Types
                .FirstOrDefault(t => t.Id == updatedSpravka.SpravkaTypeId);

            if (type == null)
                return BadRequest("Вид справки не существует.");

            spravka.PatientId = updatedSpravka.PatientId;
            spravka.SpravkaTypeId = updatedSpravka.SpravkaTypeId;
            spravka.Name = type.Name;
            spravka.Price = type.Price;

            return Ok(spravka);
        }

        // Удалить справку
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            Spravka? spravka = _spravki.FirstOrDefault(s => s.Id == id);

            if (spravka == null)
                return NotFound();

            _spravki.Remove(spravka);

            return NoContent();
        }
    }
}
