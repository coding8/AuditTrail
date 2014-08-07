using AuditTrail.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AuditTrail.Controllers
{
    public class AuditsController : Controller
    {
        private AuditTrailContext db = new AuditTrailContext();

        // GET: Audits
        public ActionResult Index()
        {
            return View(db.Audit.OrderByDescending(x=>x.Id).ToList());
        }

        // GET: Audits/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit audit = db.Audit.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
