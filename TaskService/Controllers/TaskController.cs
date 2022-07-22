using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Http;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private TaskContext _context;

        private string queueName = "tasks";

        public TaskController(ILogger<TaskController> logger, TaskContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskService.Task>>> Get()
        {
            var tasks = await _context.Tasks.Select(x => x).ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Task taskMessage)
        {
            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };

            taskMessage = await TryAddTaskToDB(taskMessage);

            if(taskMessage == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(taskMessage);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);

                return Ok();
            }  
        }

        private async Task<TaskService.Task> TryAddTaskToDB(Task task)
        {
            _context.Tasks.Add(task);

            try
            {
                await _context.SaveChangesAsync();
                return task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
