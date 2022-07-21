using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TaskProcessorService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskProcessorController : ControllerBase
    {
        private TaskContext _context;

        public TaskProcessorController(TaskContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tasks>>> Get()
        {
            var registries = await _context
                .Tasks.Select(x => x)
                .ToListAsync();

            return Ok(registries);
        }
    }
}
