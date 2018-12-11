using System.Web.Mvc;
using AdventureWorks.Services.HumanResources;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace AdventureWorks.Web.Controllers
{
    public class DepartmentsController : Controller
    {
        // GET: Departments
        public ActionResult Index()
        {
            HttpContent httpContent = new StringContent("");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpClient = new HttpClient();
            var departments = httpClient.GetAsync("http://localhost:49886/api/DepartmentApi/").Result.Content.ReadAsStringAsync().Result;
            var departmentGroups = JsonConvert.DeserializeObject(departments);
            
            return View(departmentGroups);
        }

        // GET: Departments/Employees/{id}
        public ActionResult Employees(int id)
        {
            DepartmentService departmentService = new DepartmentService();
            var departmentEmployees = departmentService.GetDepartmentEmployees(id);
            var departmentInfo = departmentService.GetDepartmentInfo(id);

            ViewBag.Title = "Employees in " + departmentInfo.Name + " Department";

            return View(departmentEmployees);
        }
    }
}
