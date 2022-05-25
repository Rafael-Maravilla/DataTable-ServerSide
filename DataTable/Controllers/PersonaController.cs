using Microsoft.AspNetCore.Mvc;
using DataTable.Models;
using DataTable.Models.ViewModels;
using System.Linq;
using System.Linq.Dynamic;

namespace DataTable.Controllers
{
    public class PersonaController : Controller
    {
        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColumn = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult obtenerDatos()
        {
            List<TablePersona> lista = new List<TablePersona>();

            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][dir]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["searchValue"].FirstOrDefault();

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;

            using(DataTableContext db = new DataTableContext())
            {
                IQueryable<TablePersona> query = (from d in db.Personas
                         select new TablePersona
                         {
                             ID = d.Id,
                             Nombre = d.Nombre,
                             Edad = (int)d.Edad
                         });

                if(searchValue != null)
                {
                    query = query.Where(d => d.Nombre == searchValue);
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
                {
                    query = query.OrderBy(sortColumn + " " + sortColumnDir);
                }
                recordsTotal = query.Count();     
                
                lista = query.Skip(skip).Take(pageSize).ToList();
            }

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lista});
        }
    }
}
