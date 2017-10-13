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
        private readonly LoggerContext _context;

        public LoggersController(LoggerContext context)
        {
            _context = context;
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

            var model = await _context.Loggers.SingleOrDefaultAsync(x =>
                x.Name.Equals(logger, StringComparison.InvariantCultureIgnoreCase));

            return model == null
                ? NotFound()
                : await SaveRequest(model);
        }

        public async Task<IActionResult> Index()
        {
            return View("Index", await _context.Loggers.ToListAsync());
        }

        public async Task<IActionResult> View(int id)
        {
            var model = await _context.Loggers.SingleOrDefaultAsync(x => x.Id == id);
            if (model == null)
                return NotFound();

            model.Requests = await _context.Requests.OrderByDescending(x => x.DateTime).ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name")]Logger logger)
        {
            if (!ModelState.IsValid)
                return View("Index", await _context.Loggers.ToListAsync());

            _context.Loggers.Add(logger);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var logger = await _context.Loggers.SingleOrDefaultAsync(x => x.Id == id);
            if (logger != null)
            {
                _context.Remove(logger);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> SaveRequest(Logger logger)
        {
            var request = new Request
            {
                Logger = logger,
                DateTime = DateTime.Now,
                Method = Request.Method,
                Body = new StreamReader(Request.Body).ReadToEnd()
            };

            await _context.Requests.AddAsync(request);

            var json = false;
            foreach (var requestHeader in Request.Headers)
            {
                var header = new Header
                {
                    Request = request,
                    Key = requestHeader.Key,
                    Value = requestHeader.Value
                };
                await _context.Headers.AddAsync(header);

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

            await _context.SaveChangesAsync();

            return Content("OK");
        }

    }
}