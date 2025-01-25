using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prog.Model;
using Prog.Services;
using System.Threading.Tasks;

namespace Prog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SortController : ControllerBase
    {
        private readonly ILogger<SortController> _logger;

        public SortController(ILogger<SortController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpPost("heapsort")]
        public async Task<IActionResult> HeapSortParamsJson()
        {
            try
            {
                // Чтение данных из запроса
                var request = await HttpContext.Request.ReadFromJsonAsync<SortRequest?>();

                // Проверка входных данных
                if (request == null || request.Array == null || request.Array.Length == 0)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid input array. Please provide a non-empty array of integers."
                    });
                }

                // Выполнение сортировки
                HeapSort heapSort = new HeapSort(request.Array);
                var sortedArray = heapSort.GetSortedArrayAsString();

                // Возврат результата
                return Ok(new { SortedArray = sortedArray });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing heap sort request.");
                return Problem("An error occurred while processing the request.");
            }
        }
    }

    public class SortRequest
    {
        public int[] Array { get; set; }
    }
}