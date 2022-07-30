using System.Web.Mvc;
using BowlingLegends.Models;

namespace BowlingLegends.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
            db = new BowlingLegendsContext();
        }

        protected BowlingLegendsContext db { get; set; }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}