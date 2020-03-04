using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPracticeV2.Controllers
{
    public class ExchangeStudentController : StudentController
    {
        // GET: ExchangeStudent
        public ActionResult Index()
        {
            return View();
        }
    }
}