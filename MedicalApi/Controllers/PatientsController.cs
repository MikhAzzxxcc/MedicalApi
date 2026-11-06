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
        public List<Patient> GetAll()
        {
            return Patients;
        }

        // Получить пациента по ID
        [HttpGet("{id}")]
        public ActionResult<Patient> GetById(int id)
        {
            Patient? patient = Patients.FirstOrDefault(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        // Добавить нового пациента
        [HttpPost]
        public ActionResult<Patient> Create(Patient patient)
        {
            patient.Id = Patients.Count + 1;
            Patients.Add(patient);

            return CreatedAtAction(
                nameof(GetById),
                new { id = patient.Id },
                patient
            );
        }
    }
}
