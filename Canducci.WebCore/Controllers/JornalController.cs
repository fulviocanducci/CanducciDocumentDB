using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Canducci.WebCore.Models;
using Canducci.DocumentDBCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Canducci.WebCore.Controllers
{
    public class JornalController : Controller
    {
        public readonly RepositoryJornalAbstract Repository;
        //public readonly ConnectionDocumentDB Db;

        public JornalController(RepositoryJornalAbstract repository)
        {
            Repository = repository;
        }

        //public JornalController(IOptions<ConfigurationDocumentDB> options)
        //{
        //    Db = new ConnectionDocumentDB(options.Value.Url, options.Value.Key, options.Value.Database);
        //    Repository = new RepositoryJornal(Db);
        //}

        [HttpGet]
        public async Task<IActionResult> Index()
        {            
            return View(await Repository.AllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost()]
        public async Task<IActionResult> Create(Jornal jornal)
        {
            jornal = await Repository.InsertAsync(jornal);
            if (jornal != null)
            {
                return RedirectToAction("Edit", new { id = jornal.Id });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            return View(await Repository.FindAsync(id));
        }

        [HttpPost()]
        public async Task<IActionResult> Edit(Jornal jornal, string id)
        {
            await Repository.UpdateAsync(jornal, id);
            return RedirectToAction("Edit", new { id = jornal.Id });
        }
    }
}
