using Microsoft.AspNetCore.Mvc;
using Prog.Model;
using System;
using System.Linq;

namespace Prog.Controllers
{
    [ApiController]
    [Route("array")]
    public class ArrayController : ControllerBase
    {
        private static int[] _storedArray = System.Array.Empty<int>(); // Временное хранилище массива

        /// <summary>
        /// Передать массив на сервер.
        /// </summary>
        [HttpPost("send")]
        public IActionResult SendArray([FromBody] ArrayRequest request)
        {
            if (request.Array == null || request.Array.Length == 0)
            {
                return BadRequest(new { Message = "Array cannot be null or empty." });
            }

            _storedArray = request.Array;
            return Ok(new ArrayResponse { Message = "Array received successfully.", Array = _storedArray });
        }

        /// <summary>
        /// Отсортировать массив.
        /// </summary>
        [HttpPost("sort")]
        public IActionResult SortArray()
        {
            if (_storedArray == null || _storedArray.Length == 0)
            {
                return BadRequest(new { Message = "No array stored on the server." });
            }

            System.Array.Sort(_storedArray); // Сортировка массива
            return Ok(new ArrayResponse { Message = "Array sorted successfully.", Array = _storedArray });
        }

        /// <summary>
        /// Получить отсортированный массив.
        /// </summary>
        [HttpGet("sorted")]
        public IActionResult GetSortedArray()
        {
            if (_storedArray == null || _storedArray.Length == 0)
            {
                return BadRequest(new { Message = "No array stored on the server." });
            }

            var sortedArray = _storedArray.OrderBy(x => x).ToArray(); // Сортировка и возврат
            return Ok(new ArrayResponse { Message = "Sorted array retrieved successfully.", Array = sortedArray });
        }

        /// <summary>
        /// Получить отсортированную часть массива от индекса до индекса.
        /// </summary>
        [HttpPost("sortpart")]
        public IActionResult SortPartOfArray([FromBody] SortPartRequest request)
        {
            if (_storedArray == null || _storedArray.Length == 0)
            {
                return BadRequest(new { Message = "No array stored on the server." });
            }

            if (request.StartIndex < 0 || request.EndIndex >= _storedArray.Length || request.StartIndex > request.EndIndex)
            {
                return BadRequest(new { Message = "Invalid start or end index." });
            }

            // Сортировка части массива
            var partToSort = _storedArray
                .Skip(request.StartIndex)
                .Take(request.EndIndex - request.StartIndex + 1)
                .OrderBy(x => x)
                .ToArray();

            return Ok(new ArrayResponse { Message = "Part of array sorted successfully.", Array = partToSort });
        }

        /// <summary>
        /// Добавить элементы в начало, конец или после указанного индекса.
        /// </summary>
        [HttpPatch("addelements")] // Используем HttpPatch
public IActionResult AddElements([FromBody] AddElementsRequest request)
{
    try
    {
        if (request.Elements == null || request.Elements.Length == 0)
        {
            return BadRequest(new { Message = "Elements cannot be null or empty." });
        }

        if (request.Index == null)
        {
            // Добавить в конец
            _storedArray = _storedArray.Concat(request.Elements).ToArray();
            return Ok(new ArrayResponse { Message = "Elements added to the end of the array.", Array = _storedArray });
        }
        else if (request.Index == 0)
        {
            // Добавить в начало
            _storedArray = request.Elements.Concat(_storedArray).ToArray();
            return Ok(new ArrayResponse { Message = "Elements added to the beginning of the array.", Array = _storedArray });
        }
        else if (request.Index > 0 && request.Index < _storedArray.Length)
        {
            // Добавить после указанного индекса
            _storedArray = _storedArray
                .Take(request.Index.Value + 1)
                .Concat(request.Elements)
                .Concat(_storedArray.Skip(request.Index.Value + 1))
                .ToArray();
            return Ok(new ArrayResponse { Message = $"Elements added after index {request.Index}.", Array = _storedArray });
        }
        else
        {
            return BadRequest(new { Message = "Invalid index." });
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Message = "An error occurred while processing the request.", Error = ex.Message });
    }
}
        /// <summary>
        /// Сгенерировать случайный массив.
        /// </summary>
        [HttpPost("generate")]
        public IActionResult GenerateRandomArray([FromQuery] int size = 10)
        {
            if (size <= 0)
            {
                return BadRequest(new { Message = "Size must be greater than 0." });
            }

            var random = new Random();
            _storedArray = Enumerable.Range(0, size).Select(_ => random.Next(1, 100)).ToArray(); // Генерация случайного массива
            return Ok(new ArrayResponse { Message = "Random array generated successfully.", Array = _storedArray });
        }

        /// <summary>
        /// Удалить массив.
        /// </summary>
        [HttpDelete("clear")]
        public IActionResult ClearArray()
        {
            _storedArray = System.Array.Empty<int>(); // Очистка массива
            return Ok(new ClearArrayResponse { Message = "Array cleared successfully." });
        }
    }
}