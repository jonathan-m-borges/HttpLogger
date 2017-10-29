using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttpLogger.Data;
using HttpLogger.Models;
using HttpLogger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HttpLogger.Controllers
{
    public class LoggersController : Controller
    {
        private readonly ILoggerRepository repository;
        private readonly IHttpLoggerService service;

        public LoggersController(ILoggerRepository repository, IHttpLoggerService service)
        {
            this.service = service;
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
                
            await SaveRequest(logger);

            return Content("OK");
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

            if (logger != null)
            {
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

        private async Task SaveRequest(string logger)
        {
            var request = new Request
            {
                DateTime = DateTime.Now,
                Method = Request.Method,
            };
            
            await service.LogRequestAsync(logger, request, Request.Body );
        }

    }
}