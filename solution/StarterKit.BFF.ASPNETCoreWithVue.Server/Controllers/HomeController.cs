﻿using Microsoft.AspNetCore.Mvc;

namespace StarterKit.BFF.ASPNETCoreWithVue.Server.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Route("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
