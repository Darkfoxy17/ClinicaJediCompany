﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Atendente, Dentista, Paciente")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
