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
using BowlingLegends.OtherModels;

namespace BowlingLegends.Controllers
{
    public class ScoresController : BaseController
    {

        // GET: Scores
        public ActionResult Index()
        {
            var scores = db.Scores.Include(s => s.Bowler).Include(s => s.Round);
            return View(scores.ToList().OrderByDescending(x => x.ScoreID));
        }

        // GET: Scores
        public ActionResult TopScores()
        {
            var scores = db.Scores.OrderByDescending(x => x.Score1).Take(10).ToList();
            return View(scores);
        }

        public ActionResult TopSeriesScores()
        {
            var list = new List<SeriesScoresId>();
            foreach (var series in db.Series)
            {
                foreach (var bowler in series.Rounds.SelectMany(x => x.Scores.Select(a => a.Bowler)).Distinct())
                {
                    var score = new SeriesScoresId
                    {
                        Series = series.SeriesID,
                        Bowler = bowler,
                        Score = series.Rounds.Sum(x => x.Scores.Where(b => b.Bowler.BowlerID == bowler.BowlerID).Sum(a => a.Score1))
                    };
                    list.Add(score);
                }
            }
            return View(list.OrderByDescending(x => x.Score).Take(10));
        }


        // GET: Scores/Create
        [Authorize]
        public ActionResult Create()
        {
            var lastScore = db.Scores.OrderByDescending(x => x.ScoreID).FirstOrDefault();
            if (lastScore != null)
            {
                ViewBag.NewScore = lastScore.ScoreID + 1;
            }
            var presentBowlers = db.Rounds.OrderByDescending(x => x.RoundID).FirstOrDefault().Scores.Select(x => x.Bowler);

            var result = db.Bowlers.ToList().Where(x => !presentBowlers.Any(p => p.BowlerID == x.BowlerID));
            ViewBag.BowlerID = new SelectList(result, "BowlerID", "Name");
            ViewBag.RoundID = new SelectList(db.Rounds.OrderByDescending(x => x.RoundID).Take(1), "RoundID", "RoundID");
            return View();
        }

        // POST: Scores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ScoreID,BowlerID,Score1,RoundID")] Score score)
        {
            if (db.Scores.Any(x => x.RoundID == score.RoundID && x.BowlerID == score.BowlerID))
            {
                ViewBag.Error = string.Format("{0} ALREADY HAS A SCORE FOR THIS ROUND", db.Bowlers.Where(x => x.BowlerID == score.BowlerID).FirstOrDefault().Name);
                var lastScore = db.Scores.OrderByDescending(x => x.ScoreID).FirstOrDefault();
                if (lastScore != null)
                {
                    ViewBag.NewScore = lastScore.ScoreID + 1;
                }

                ViewBag.BowlerID = new SelectList(db.Bowlers, "BowlerID", "Name");
                ViewBag.RoundID = new SelectList(db.Rounds.OrderByDescending(x => x.RoundID).Take(1), "RoundID", "RoundID");
                return View();
            }
            if (ModelState.IsValid)
            {
                db.Scores.Add(score);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BowlerID = new SelectList(db.Bowlers, "BowlerID", "Name", score.BowlerID);
            ViewBag.RoundID = new SelectList(db.Rounds, "RoundID", "RoundID", score.RoundID);
            return View(score);
        }

        // GET: Scores/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Score score = db.Scores.Find(id);
            if (score == null)
            {
                return HttpNotFound();
            }
            ViewBag.BowlerID = new SelectList(db.Bowlers, "BowlerID", "Name", score.BowlerID);
            ViewBag.RoundID = new SelectList(db.Rounds, "RoundID", "RoundID", score.RoundID);
            return View(score);
        }

        // POST: Scores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ScoreID,BowlerID,Score1,RoundID")] Score score)
        {
            if (ModelState.IsValid)
            {
                db.Entry(score).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BowlerID = new SelectList(db.Bowlers, "BowlerID", "Name", score.BowlerID);
            ViewBag.RoundID = new SelectList(db.Rounds, "RoundID", "RoundID", score.RoundID);
            return View(score);
        }

    }
}
