using System.Web.Mvc;
using AdventureWorks.Services.HumanResources;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Configuration;
using StackExchange.Redis;

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
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            var departments = httpClient.GetAsync("http://azure-training-webapps.azurewebsites.net/api/DepartmentApi/").Result.Content.ReadAsStringAsync().Result;
            List<Department> departmentGroups = Serializer.Deserialize<List<Department>>(departments);
            
            return View(departmentGroups);
        }

        // GET: Departments/Employees/{id}
        public ActionResult Employees(int id)
        {
            string strUrl = "http://azure-training-webapps.azurewebsites.net/api/DepartmentApi/" + id.ToString();

            JavaScriptSerializer EmployeeSerializer = new JavaScriptSerializer();
            var EmployeehttpClient = new HttpClient();
            var employees = EmployeehttpClient.GetAsync(strUrl).Result.Content.ReadAsStringAsync().Result;
            List<DepartmentEmployee> departmentEmployees = EmployeeSerializer.Deserialize<List<DepartmentEmployee>>(employees);

            JavaScriptSerializer InfoSerializer = new JavaScriptSerializer();
            HttpContent InfohttpContent = new StringContent("");
            InfohttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpClient = new HttpClient();
            var Info = httpClient.PostAsync(strUrl, InfohttpContent).Result.Content.ReadAsStringAsync().Result;
            //DepartmentInfo departmentInfo = JsonConvert.DeserializeObject(Info).;
            DepartmentInfo departmentInfo = EmployeeSerializer.Deserialize<DepartmentInfo>(Info);



            ViewBag.Title = "Employees in " + departmentInfo.Name + " Department";

            return View(departmentEmployees);
        }
    }
}
