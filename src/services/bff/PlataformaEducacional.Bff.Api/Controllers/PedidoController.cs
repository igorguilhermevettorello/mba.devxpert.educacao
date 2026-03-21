using Microsoft.AspNetCore.Mvc;

namespace PlataformaEducacional.Bff.Api.Controllers;

public class PedidoController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
