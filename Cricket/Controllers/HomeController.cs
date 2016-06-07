using Cricket.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Cricket.Controllers
{
    public class HomeController : Controller
    {
        private CricketTeamsEntities db = new CricketTeamsEntities();
        private CricketTeamsEntities1 db1 = new CricketTeamsEntities1();
        // GET: Home
        public ActionResult Index()
        {
            var team = from t in db.tblTeam1 select t;
            return View(team);
        }

        public ActionResult Players(string team)
        {
            if (team == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team1 = from d in db1.tblPlayer1 where d.PlayerMainTeam.ToLower() == team.ToLower() select d;

            if (team1 == null)
            {
                return HttpNotFound();
            }
            return View(team1);
        }

        public ActionResult PlayersDetails(string name)
        {
            if (name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var name1 = from d in db1.tblPlayer1 where d.PlayerName.ToLower() == name.ToLower() select d;

            if (name1 == null)
            {
                return HttpNotFound();
            }
            return View(name1);
        }

        public ActionResult AddPlayer()
        {
            var TeamList = new List<string>();
            var TeamQuery = from t in db.tblTeam1
                            select t.TeamName;
            TeamList.AddRange(TeamQuery.Distinct());
            ViewBag.TeamNames = new SelectList(TeamList);
            return View();
        }

        [HttpPost]
        public ActionResult AddPlayer([Bind(Include ="PlayerID,PlayerName,PlayerDOB,PlayerTeams,PlayerBattingStyle,PlayerBowlingStyle,PlayerProfile,PlayerRole,PlayerMainTeam")] tblPlayer1 player, HttpPostedFileBase file, FormCollection form)
        {
            player.PlayerMainTeam = form["TeamNames"];
            if (file!=null)
            {
                string pic = Path.GetFileName(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"+player.PlayerMainTeam+"/"), pic);

                //file is uploaded

                file.SaveAs(path);
                player.PlayerPhoto = "~/images/" + player.PlayerMainTeam.ToLower() + "/" + pic;
            }

            db1.tblPlayer1.Add(player);
            db1.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var player = db1.tblPlayer1.Find(id);

            if(player == null)
            {
                return HttpNotFound();
            }

            return View(player);
        }

        [HttpPost, ActionName("Delete")]

        public ActionResult DeletePlayer(int? id)
        {
            tblPlayer1 player = db1.tblPlayer1.Find(id);
            db1.tblPlayer1.Remove(player);
            db1.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return  new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var player = db1.tblPlayer1.Find(id);

            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlayerID,PlayerName,PlayerDOB,PlayerTeams,PlayerBattingStyle,PlayerBowlingStyle,PlayerProfile,PlayerRole,PlayerMainTeam,PlayerPhoto")] tblPlayer1 player, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    string pic = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/" + player.PlayerMainTeam + "/"), pic);

                    //file is uploaded

                    file.SaveAs(path);
                    player.PlayerPhoto = "~/images/" + player.PlayerMainTeam.ToLower() + "/" + pic;
                }

                db1.Entry(player).State = EntityState.Modified;
                db1.SaveChanges();
                return RedirectToAction("Players", new { team = player.PlayerMainTeam });
            }
            return View(player);
        }
    }
}