using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BowlingLegends.Controllers;
using BowlingLegends.Models;

namespace BowlingLegends.Controllers
{
    public class RoundsController : BaseController
    {

        // GET: Rounds
        public ActionResult Index()
        {
            var rounds = db.Rounds.Include(r => r.Series);
            return View(rounds.ToList().OrderByDescending(x => x.RoundID));
        }

        // GET: Rounds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Round round = db.Rounds.Find(id);
            if (round == null)
            {
                return HttpNotFound();
            }
            return View(round);
        }

        // GET: Rounds/Create
        [Authorize]
        public ActionResult Create()
        {
            var lastRound = db.Rounds.OrderByDescending(x => x.RoundID).FirstOrDefault();
            var currentSeries = db.Series.OrderByDescending(x => x.SeriesID).FirstOrDefault();
            if (lastRound != null)
            {
                ViewBag.NewRound = lastRound.RoundID + 1;
                if (lastRound.Scores.Count < 3)
                {
                    ViewBag.Warning = string.Format("LAST ROUND HAD {0} BOWLERS. A ROUND SHOULD HAVE 3 BOWLERS MINIMUM!", lastRound.Scores.Count);
                }
               else  if (lastRound.Series.Rounds.Count + 1 > 2 && currentSeries.SeriesID == lastRound.SeriesID)
                {
                    ViewBag.Warning = string.Format("CURRENT SERIES HAS {0} ROUNDS. A SERIES TYPICALLY HAVE 2 ROUNDS!", lastRound.Series.Rounds.Count);
                }
                ViewBag.CurrentRoundforSeries = currentSeries.Rounds.Any() ? currentSeries.Rounds.Count + 1 : 1;
            }   
            ViewBag.SeriesID = new SelectList(db.Series.OrderByDescending(x => x.SeriesID).Take(1), "SeriesID", "SeriesID");
            return View();
        }

        // POST: Rounds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "RoundID,SeriesID")] Round round)
        {
            if (ModelState.IsValid)
            {
                db.Rounds.Add(round);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SeriesID = new SelectList(db.Series, "SeriesID", "SeriesID", round.SeriesID);
            return View(round);
        }

        // GET: Rounds/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Round round = db.Rounds.Find(id);
            if (round == null)
            {
                return HttpNotFound();
            }
            ViewBag.SeriesID = new SelectList(db.Series, "SeriesID", "SeriesID", round.SeriesID);
            return View(round);
        }

        // POST: Rounds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "RoundID,SeriesID")] Round round)
        {
            if (ModelState.IsValid)
            {
                db.Entry(round).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SeriesID = new SelectList(db.Series, "SeriesID", "SeriesID", round.SeriesID);
            return View(round);
        }

    }
}
