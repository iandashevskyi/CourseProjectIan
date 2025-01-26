using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prog.Model;
using Prog.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prog.Controllers
{
    [ApiController]
    [Route("array")]
    public class SortController : ControllerBase
    {
        private static int[] _storedArray = Array.Empty<int>();
        private readonly ILogger<SortController> _logger;

        public SortController(ILogger<SortController> logger)
        {
            _logger = logger;
        }

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

        [HttpPost("sort")]
        public IActionResult SortArray()
        {
            if (_storedArray == null || _storedArray.Length == 0)
            {
                return BadRequest(new { Message = "No array stored on the server." });
            }

            Array.Sort(_storedArray);
            return Ok(new ArrayResponse { Message = "Array sorted successfully.", Array = _storedArray });
        }

        [HttpGet("sorted")]
        public IActionResult GetSortedArray()
        {
            if (_storedArray == null || _storedArray.Length == 0)
            {
                return BadRequest(new { Message = "No array stored on the server." });
            }

            var sortedArray = _storedArray.OrderBy(x => x).ToArray();
            return Ok(new ArrayResponse { Message = "Sorted array retrieved successfully.", Array = sortedArray });
        }

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

            var partToSort = _storedArray
                .Skip(request.StartIndex)
                .Take(request.EndIndex - request.StartIndex + 1)
                .OrderBy(x => x)
                .ToArray();

            return Ok(new ArrayResponse { Message = "Part of array sorted successfully.", Array = partToSort });
        }

        [HttpPatch("addelements")]
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
                    _storedArray = _storedArray.Concat(request.Elements).ToArray();
                    return Ok(new ArrayResponse { Message = "Elements added to the end of the array.", Array = _storedArray });
                }
                else if (request.Index == 0)
                {
                    _storedArray = request.Elements.Concat(_storedArray).ToArray();
                    return Ok(new ArrayResponse { Message = "Elements added to the beginning of the array.", Array = _storedArray });
                }
                else if (request.Index > 0 && request.Index < _storedArray.Length)
                {
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

        [HttpPost("generate")]
        public IActionResult GenerateRandomArray([FromQuery] int size = 10)
        {
            if (size <= 0)
            {
                return BadRequest(new { Message = "Size must be greater than 0." });
            }

            var random = new Random();
            _storedArray = Enumerable.Range(0, size).Select(_ => random.Next(1, 100)).ToArray();
            return Ok(new ArrayResponse { Message = "Random array generated successfully.", Array = _storedArray });
        }

        [HttpDelete("clear")]
        public IActionResult ClearArray()
        {
            _storedArray = Array.Empty<int>();
            return Ok(new ClearArrayResponse { Message = "Array cleared successfully." });
        }

        [Authorize]
        [HttpPost("heapsort")]
        public async Task<IActionResult> HeapSortParamsJson()
        {
            try
            {
                var request = await HttpContext.Request.ReadFromJsonAsync<SortRequest?>();

                if (request == null || request.Array == null || request.Array.Length == 0)
                {
                    return BadRequest(new { Message = "Invalid input array. Please provide a non-empty array of integers." });
                }

                HeapSort heapSort = new HeapSort(request.Array);
                var sortedArray = heapSort.GetSortedArrayAsString();

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