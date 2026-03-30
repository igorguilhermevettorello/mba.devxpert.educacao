using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.WebApi.Core.Controllers;

namespace PlataformaEducacional.Bff.Api.Controllers;

public class CompraController : MainController
{ 
    public IActionResult Index()
    {
        return View();
    }
}
