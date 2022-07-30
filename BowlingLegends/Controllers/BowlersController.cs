using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BowlingLegends.Models;

namespace BowlingLegends.Controllers
{
    public class BowlersController : BaseController
    {
        // GET: Bowlers
        public ActionResult Index()
        {
            return View(db.Bowlers.ToList());
        }

        // GET: Bowlers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bowler bowler = db.Bowlers.Find(id);
            if (bowler == null)
            {
                return HttpNotFound();
            }
            var bowlerViewModel = new BowlerViewModel(db, bowler);
            bowlerViewModel.Update();
            return View(bowlerViewModel);
        }

        // GET: Bowlers/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bowlers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "BowlerID,Name,NickName,BowlingArm")] Bowler bowler)
        {
            if (ModelState.IsValid)
            {
                db.Bowlers.Add(bowler);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bowler);
        }

        // GET: Bowlers/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bowler bowler = db.Bowlers.Find(id);
            if (bowler == null)
            {
                return HttpNotFound();
            }
            return View(bowler);
        }

        // POST: Bowlers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "BowlerID,Name,NickName,BowlingArm")] Bowler bowler)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bowler).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bowler);
        }

        public JsonResult RoundScoresGraph(int id)
        {
            Bowler bowler = db.Bowlers.Find(id);
            var bowlerViewModel = new BowlerViewModel(db, bowler);
            bowlerViewModel.SetRoundScoresGraph();
            return Json(bowlerViewModel.RoundScoresGraph, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SeriesScoresGraph(int id)
        {
            Bowler bowler = db.Bowlers.Find(id);
            var bowlerViewModel = new BowlerViewModel(db, bowler);
            bowlerViewModel.SetSeriesScoresGraph();
            return Json(bowlerViewModel.SeriesScoresGraph, JsonRequestBehavior.AllowGet);
        }


    }
}
