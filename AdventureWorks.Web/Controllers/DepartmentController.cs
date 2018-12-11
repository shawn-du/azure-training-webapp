﻿using System.Web.Mvc;
using AdventureWorks.Services.HumanResources;
using Microsoft.ApplicationInsights;

namespace AdventureWorks.Web.Controllers
{
    public class DepartmentsController : Controller
    {
        // GET: Departments
        public ActionResult Index()
        {
            DepartmentApiController apiController = new DepartmentApiController();
            var departmentGroups = apiController.Get();
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
