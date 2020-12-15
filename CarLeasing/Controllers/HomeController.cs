using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarLeasing.Models;

namespace CarLeasing.Controllers
{
    public class HomeController : Controller
    {
        // создаем контекст данных
        DemoContext db = new DemoContext();

        public ActionResult Index()
        {
            Customer customer = db.Customers.First();
            ViewBag.Customer = customer;
            // возвращаем представление
            return View();
        }

        [HttpPost]
        public ActionResult OrderCar()
        {
            Customer customer = db.Customers.First();
            Scoring.Score scoring = new Scoring.Score(customer);
            IEnumerable<Car> Cars = db.Cars;
            ViewBag.Customer = customer;
            ViewBag.Scoring = scoring;
            ViewBag.Cars = Cars.Where(x=> x.RequiredScore <= scoring.TotalScore);
            return View();
        }
    }
}