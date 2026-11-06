
using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        public static List<Patient> Patients = new List<Patient>();

        // Получить всех пациентов
        [HttpGet]
        [ProducesResponseType(typeof(List<Patient>), 200)]
        public List<Patient> GetAll()
        {
            return Patients;
        }

        // Получить пациента по ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Patient), 200)]
        [ProducesResponseType(404)]
        public ActionResult<Patient> GetById(int id)
        {
            Patient? patient = Patients.FirstOrDefault(p => p.Id == id);

            if (patient == null)
                return NotFound();

            return patient;
        }

        // Добавить пациента
        [HttpPost]
        [ProducesResponseType(typeof(Patient), 201)]
        [ProducesResponseType(400)]
        public ActionResult<Patient> Create(Patient patient)
        {
            if (!IsPatientValid(patient))
                return BadRequest("Имя пациента не должно быть пустым, а дата рождения не может быть в будущем.");

            patient.Id = Patients.Count == 0
                ? 1
                : Patients.Max(p => p.Id) + 1;

            Patients.Add(patient);

            return CreatedAtAction(
                nameof(GetById),
                new { id = patient.Id },
                patient
            );
        }

        // Изменить пациента
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Patient), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Patient> Update(int id, Patient updatedPatient)
        {
            Patient? patient = Patients.FirstOrDefault(p => p.Id == id);

            if (patient == null)
                return NotFound();

            if (!IsPatientValid(updatedPatient))
                return BadRequest("Имя пациента не должно быть пустым, а дата рождения не может быть в будущем.");

            patient.Name = updatedPatient.Name;
            patient.BirthDate = updatedPatient.BirthDate;
            patient.Phone = updatedPatient.Phone;
            patient.Address = updatedPatient.Address;

            return Ok(patient);
        }

        // Удалить пациента
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            Patient? patient = Patients.FirstOrDefault(p => p.Id == id);

            if (patient == null)
                return NotFound();

            Patients.Remove(patient);

            return NoContent();
        }

        // Получить справки конкретного пациента
        [HttpGet("{id}/spravki")]
        [ProducesResponseType(typeof(List<Spravka>), 200)]
        [ProducesResponseType(404)]
        public ActionResult<List<Spravka>> GetPatientSpravki(int id)
        {
            Patient? patient = Patients.FirstOrDefault(p => p.Id == id);

            if (patient == null)
                return NotFound("Пациент не найден.");

            List<Spravka> spravki = SpravkiController.GetAllSpravki()
                .Where(s => s.PatientId == id)
                .ToList();

            return Ok(spravki);
        }

        // Проверка данных пациента
        private bool IsPatientValid(Patient patient)
        {
            if (string.IsNullOrWhiteSpace(patient.Name))
                return false;

            if (patient.BirthDate > DateTime.Now)
                return false;

            return true;
        }
    }
}
