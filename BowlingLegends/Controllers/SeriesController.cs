using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BowlingLegends.Models;

namespace BowlingLegends.Controllers
{
    public class SeriesController : BaseController
    {
        // GET: Series
        public ActionResult Index()
        {
            var series = db.Series.Include(s => s.Season);
            return View(series.ToList().OrderByDescending(x => x.SeriesID));
        }

        // GET: Series/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Series series = db.Series.Find(id);
            if (series == null)
            {
                return HttpNotFound();
            }
            return View(series);
        }

        // GET: Series/Create
        [Authorize]
        public ActionResult Create()
        {
            var lastSeries = db.Series.OrderByDescending(x => x.SeriesID).FirstOrDefault();
            if (lastSeries != null)
            {
                ViewBag.NewSeries = lastSeries.SeriesID + 1;
                if (lastSeries.Rounds.Count != 2)
                {
                    ViewBag.Warning = string.Format("LAST SERIES HAD {0} ROUNDS. A SERIES TYPICALLY HAVE 2 ROUNDS!", lastSeries.Rounds.Count);
                }
            }
            ViewBag.SeasonID = new SelectList(db.Seasons.OrderByDescending(x => x.SeasonID).Take(1), "SeasonID", "Year");
            return View();
        }

        // POST: Series/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "SeriesID,Date,SeasonID")] Series series)
        {
            if (ModelState.IsValid)
            {
                series.Date = DateTime.Now;
                db.Series.Add(series);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "Year", series.SeasonID);
            return View(series);
        }

        // GET: Series/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Series series = db.Series.Find(id);
            if (series == null)
            {
                return HttpNotFound();
            }
            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "Year", series.SeasonID);
            return View(series);
        }

        // POST: Series/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "SeriesID,Date,SeasonID")] Series series)
        {
            if (ModelState.IsValid)
            {
                db.Entry(series).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "Year", series.SeasonID);
            return View(series);
        }

    }
}
