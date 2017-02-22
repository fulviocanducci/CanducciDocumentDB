using Canducci.Web.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace Canducci.Web.Controllers
{
    public class CreditsController : Controller
    {
        protected RepositoryCreditAbstract Repository { get; private set; }

        public CreditsController(RepositoryCreditAbstract repository)
        {
            Repository = repository;            
        }        
        
        public async Task<ActionResult> Index(int? page)
        {                    
            return View(await Repository.AllAsync());
        }
                
        public async Task<ActionResult> Details(string id)
        {            
            return View(await Repository.FindAsync(id));
        }

        public ActionResult Create()
        {
            return View();
        }
                
        [HttpPost]
        public async Task<ActionResult> Create(Credit credit)
        {
            try
            {
                credit = await Repository.InsertAsync(credit);
                if (string.IsNullOrEmpty(credit.Id))
                    return RedirectToAction("Index");
                return RedirectToAction("Edit", new { id = credit.Id });
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            return View(await Repository.FindAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, Credit credit)
        {
            try
            {
                await Repository.UpdateAsync(credit, id);
                return RedirectToAction("Edit", new { id = credit.Id });
            }
            catch
            {
                return View();
            }
        }
        
        public async Task<ActionResult> Delete(string id)
        {
            return View(await Repository.FindAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id, Credit credit)
        {
            try
            {
                await Repository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
