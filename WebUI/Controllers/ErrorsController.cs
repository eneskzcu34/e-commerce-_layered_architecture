using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebUI.Controllers
{
    [Route("[controller]")]
    public class ErrorsController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult PageWasNotFound()
        {
            return View();
        }

    }
}