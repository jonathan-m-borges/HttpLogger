using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttpLogger.Data;
using HttpLogger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HttpLogger.Controllers
{
    public class LoggersController : Controller
    {
        private readonly ILoggerRepository repository;

        public LoggersController(ILoggerRepository repository)
        {
            this.repository = repository;
        }

        [Route("/log/{logger}")]
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        public async Task<IActionResult> Index(string logger)
        {
            if (string.IsNullOrWhiteSpace(logger))
                return RedirectToAction(nameof(Index));

            var model = await repository.GetByNameAsync(logger);

            return model == null
                ? NotFound()
                : await SaveRequest(model);
        }

        public async Task<IActionResult> Index()
        {
            return View("Index", await repository.GetAllAsync());
        }

        public new async Task<IActionResult> View(string id)
        {
            var model = await repository.GetByNameAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")]Logger logger)
        {
            if (!ModelState.IsValid)
                return View("Index", await repository.GetAllAsync());

            await repository.CreateAsync(logger);
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Clear(string id)
        {
            var logger = await repository.GetByNameAsync(id);

            if (logger!=null){
                logger.Requests.Clear();
                await repository.UpdateAsync(logger);
            }
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            await repository.RemoveAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> SaveRequest(Logger logger)
        {
            var request = new Request
            {
                DateTime = DateTime.Now,
                Method = Request.Method,
                Body = new StreamReader(Request.Body).ReadToEnd()
            };

            var json = false;
            foreach (var requestHeader in Request.Headers)
            {
                var header = new Header(requestHeader.Key, requestHeader.Value);
                request.Headers.Add(header);

                if (header.Key.Equals("content-type", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (header.Value.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
                        json = true;
                }
            }

            if (json)
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject(request.Body);
                    request.Body = JsonConvert.SerializeObject(obj, Formatting.Indented);
                }
                catch (Exception)
                {
                }
            }

            logger.Requests.Add(request);

            await repository.UpdateAsync(logger);

            return Content("OK");
        }

    }
}