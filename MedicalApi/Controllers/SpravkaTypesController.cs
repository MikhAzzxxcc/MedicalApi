
using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpravkaTypesController : ControllerBase
    {
        public static List<SpravkaType> Types = new List<SpravkaType>
        {
            new SpravkaType
            {
                Id = 1,
                Name = "Справка 083",
                Price = 100,
                Description = "Медицинская справка"
            },
            new SpravkaType
            {
                Id = 2,
                Name = "Санитарная книжка",
                Price = 750,
                Description = "Документ о прохождении медицинского осмотра"
            },
            new SpravkaType
            {
                Id = 3,
                Name = "Справка для учебы",
                Price = 300,
                Description = "Справка для предоставления в учебное заведение"
            },
            new SpravkaType
            {
                Id = 4,
                Name = "Справка для работы",
                Price = 400,
                Description = "Медицинская справка для работодателя"
            }
        };

        // Получить все виды справок
        [HttpGet]
        [ProducesResponseType(typeof(List<SpravkaType>), 200)]
        public List<SpravkaType> GetAll()
        {
            return Types;
        }

        // Получить вид справки по ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SpravkaType), 200)]
        [ProducesResponseType(404)]
        public ActionResult<SpravkaType> GetById(int id)
        {
            SpravkaType? type = Types.FirstOrDefault(t => t.Id == id);

            if (type == null)
                return NotFound();

            return type;
        }

        // Добавить новый вид справки
        [HttpPost]
        [ProducesResponseType(typeof(SpravkaType), 201)]
        [ProducesResponseType(400)]
        public ActionResult<SpravkaType> Create(SpravkaType type)
        {
            if (!IsTypeValid(type))
                return BadRequest("Название не должно быть пустым, а цена должна быть больше нуля.");

            type.Id = Types.Count == 0
                ? 1
                : Types.Max(t => t.Id) + 1;

            Types.Add(type);

            return CreatedAtAction(
                nameof(GetById),
                new { id = type.Id },
                type
            );
        }

        // Изменить вид справки
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SpravkaType), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<SpravkaType> Update(int id, SpravkaType updatedType)
        {
            SpravkaType? type = Types.FirstOrDefault(t => t.Id == id);

            if (type == null)
                return NotFound();

            if (!IsTypeValid(updatedType))
                return BadRequest("Название не должно быть пустым, а цена должна быть больше нуля.");

            type.Name = updatedType.Name;
            type.Price = updatedType.Price;
            type.Description = updatedType.Description;

            return Ok(type);
        }

        // Удалить вид справки
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            SpravkaType? type = Types.FirstOrDefault(t => t.Id == id);

            if (type == null)
                return NotFound();

            Types.Remove(type);

            return NoContent();
        }

        // Проверка данных вида справки
        private bool IsTypeValid(SpravkaType type)
        {
            if (string.IsNullOrWhiteSpace(type.Name))
                return false;

            if (type.Price <= 0)
                return false;

            return true;
        }
    }
}
