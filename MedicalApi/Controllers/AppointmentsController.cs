
using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        public static List<Appointment> Appointments = new List<Appointment>();

        // Получить все записи
        [HttpGet]
        [ProducesResponseType(typeof(List<Appointment>), 200)]
        public List<Appointment> GetAll()
        {
            return Appointments;
        }

        // Найти записи по имени врача
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<Appointment>), 200)]
        public ActionResult<List<Appointment>> Search(string doctorName)
        {
            List<Appointment> result = Appointments
                .Where(a => a.DoctorName.Contains(
                    doctorName,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(result);
        }

        // Получить запись по ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Appointment), 200)]
        [ProducesResponseType(404)]
        public ActionResult<Appointment> GetById(int id)
        {
            Appointment? appointment = Appointments
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            return appointment;
        }

        // Создать запись
        [HttpPost]
        [ProducesResponseType(typeof(Appointment), 201)]
        [ProducesResponseType(400)]
        public ActionResult<Appointment> Create(Appointment appointment)
        {
            if (!IsAppointmentValid(appointment))
                return BadRequest("Проверьте данные записи. Имя врача не должно быть пустым, дата должна быть в будущем, а ID пациента должен быть положительным.");

            Patient? patient = PatientsController.Patients
                .FirstOrDefault(p => p.Id == appointment.PatientId);

            if (patient == null)
                return BadRequest("Пациент не существует.");

            appointment.Id = Appointments.Count == 0
                ? 1
                : Appointments.Max(a => a.Id) + 1;

            Appointments.Add(appointment);

            return CreatedAtAction(
                nameof(GetById),
                new { id = appointment.Id },
                appointment
            );
        }

        // Изменить запись
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Appointment), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Appointment> Update(int id, Appointment updatedAppointment)
        {
            Appointment? appointment = Appointments
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            if (!IsAppointmentValid(updatedAppointment))
                return BadRequest("Проверьте данные записи. Имя врача не должно быть пустым, дата должна быть в будущем, а ID пациента должен быть положительным.");

            Patient? patient = PatientsController.Patients
                .FirstOrDefault(p => p.Id == updatedAppointment.PatientId);

            if (patient == null)
                return BadRequest("Пациент не существует.");

            appointment.PatientId = updatedAppointment.PatientId;
            appointment.DoctorName = updatedAppointment.DoctorName;
            appointment.AppointmentDate = updatedAppointment.AppointmentDate;
            appointment.Description = updatedAppointment.Description;

            return Ok(appointment);
        }

        // Удалить запись
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            Appointment? appointment = Appointments
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            Appointments.Remove(appointment);

            return NoContent();
        }

        // Проверка данных записи
        private bool IsAppointmentValid(Appointment appointment)
        {
            if (string.IsNullOrWhiteSpace(appointment.DoctorName))
                return false;

            if (appointment.AppointmentDate <= DateTime.Now)
                return false;

            if (appointment.PatientId <= 0)
                return false;

            return true;
        }
    }
}
