using ClubLeagues.Models;
using Elmah;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ClubLeagues.Controllers
{
    [Authorize(Roles = "ClubAdmin")]
    public class UserClubsController : Controller
    {
        private readonly LBClubsEntities _db = new LBClubsEntities();

        // GET: UserClubs
        public ActionResult Index(int id)
        {
            var userClubs = _db.UserClubs.Include(u => u.Club).Include(u => u.User).Where(x => x.ClubId == id);
            ViewBag.Name = _db.Clubs.Find(id).Name;
            var model = new UserClubViewModel()
            {
                Name = _db.Clubs.Find(id).Name,
                clubid = id,
                userClubs = userClubs
            };
            return View(model);
        }


        // GET: UserClubs/Create
        public ActionResult Create(int id)
        {
            var club = _db.Clubs.Find(id);
            if (club == null)
                return HttpNotFound();
            ViewBag.Name = club.Name;
            var list = _db.Users.Where(x => x.Roles != "Mailer" && x.Roles != "Admin").ToList();
            foreach (var userClub in _db.UserClubs.Where(x => x.ClubId == id))
            {
                if (list.Any(x => x.id == userClub.UserId))
                    list.RemoveAll(x => x.id == userClub.UserId);
            }
            ViewBag.UserId = new SelectList(list, "id", "username");
            var userclub = new UserClub()
            {
                ClubId = id
            };
            return View(userclub);
        }

        // POST: UserClubs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,UserId,ClubId,rowversion")] UserClub userClub)
        {
            if (ModelState.IsValid)
            {
                _db.UserClubs.Add(userClub);
                try
                {
                    _db.SaveChanges();
                    return RedirectToAction("Index", new { id = userClub.ClubId });
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    Exception ex = e;
                    while (ex.InnerException != null)
                        ex = ex.InnerException;
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    ModelState.AddModelError(string.Empty, "Insert failed");
                }
            }


            var list = _db.Users.Where(x => x.Roles != "Mailer" || x.Roles != "Admin").ToList();
            foreach (var item in _db.UserClubs.Where(x => x.ClubId == userClub.ClubId))
            {
                if (list.Any(x => x.id == item.UserId))
                    list.RemoveAll(x => x.id == item.UserId);
            }
            ViewBag.UserId = new SelectList(list, "id", "username");
            var club = _db.Clubs.Find(userClub.ClubId);
            ViewBag.Name = club.Name;
            return View(userClub);
        }

        // GET: UserClubs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserClub userClub = _db.UserClubs.Find(id);
            if (userClub == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = userClub.Club.Name;
            return View(userClub);
        }

        // POST: UserClubs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,UserId,ClubId,Roles,rowversion")] UserClub userClub)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _db.Entry(userClub).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index", new { id = userClub.ClubId });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (UserClub)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    ModelState.AddModelError(string.Empty,
                        "Unable to save changes. The member was deleted by another user.");
                }
                else
                {
                    var databaseValues = (UserClub)databaseEntry.ToObject();

                    if (databaseValues.UserId != clientValues.UserId)
                        ModelState.AddModelError("User", "Current value: "
                                                         + databaseValues.User.username);
                    if (databaseValues.ClubId != clientValues.ClubId)
                        ModelState.AddModelError("Club", "Current value: "
                                                           + databaseValues.Club.Name);
                    

                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                                           + "was modified by another user after you got the original value. The "
                                                           + "edit operation was canceled and the current values in the database "
                                                           + "have been displayed. If you still want to edit this record, click "
                                                           + "the Save button again. Otherwise click the Back to List hyperlink.");
                    userClub.rowversion = databaseValues.rowversion;
                }
            }
            catch (Exception dex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                ErrorSignal.FromCurrentContext().Raise(dex);
            }
            return View(userClub);
        }

        // GET: UserClubs/Delete/5
        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userClub = _db.UserClubs.Find(id);
            if (userClub == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index", "Clubs");
                }
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.Error = "The record you attempted to delete "
                                + "was modified by another user after you got the original values. "
                                + "The delete operation was canceled and the current values in the "
                                + "database have been displayed. If you still want to delete this "
                                + "record, click the Delete button again. Otherwise "
                                + "click the Back to List hyperlink.";
            }
            return View(userClub);
        }

        // POST: UserClubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, byte[] rowversion)
        {
            var userClub = _db.UserClubs.Find(id);
            if (userClub == null)
            {
                ViewBag.Error = "Record was delete by another user";
            }
            else
            {
                try
                {
                    _db.UserClubs.RemoveRange(_db.UserClubs.Where(x => x.ClubId == userClub.ClubId));
                    _db.Entry(userClub).Property("rowversion").OriginalValue = rowversion;
                    _db.Entry(userClub).State = EntityState.Deleted;
                    _db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    return RedirectToAction("Delete", new { concurrencyError = true, id = id });
                }
                catch (Exception dex)
                {
                    //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                    ViewBag.Error =
                        "Unable to delete. Try again, and if the problem persists contact your system administrator.";
                    ErrorSignal.FromCurrentContext().Raise(dex);

                }
            }
            return View(userClub);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
