using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licitaciones.Areas.Licitacion.Controllers
{
    public class ChatBotController : Controller
    {
        // GET: Licitacion/ChatBot
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EditarArbol(int? idEscenario)
        {
            ViewBag.idEscenario = idEscenario;
            return View();
        }
    }
}