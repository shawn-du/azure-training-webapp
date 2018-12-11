using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using System.Web.Mvc;
using AdventureWorks.Services.HumanResources;
using Microsoft.ApplicationInsights;

namespace AdventureWorks.Web.Controllers
{
    public class DepartmentApiController : ApiController
    {
        /// <summary>
        /// Get all departments
        /// </summary>
        /// <remarks>
        /// Get a list of all departments
        /// </remarks>
        /// <returns></returns>
        /// <response code="200"></response>
        [ResponseType(typeof(IEnumerable<Department>))]
        public HttpResponseMessage Get()
        {
            DepartmentService departmentService = new DepartmentService();
            var departmentGroups = departmentService.GetDepartments();

            return Request.CreateResponse(HttpStatusCode.OK, departmentGroups);
        }

        /// <summary>
        /// Get employees by id
        /// </summary>
        /// <remarks>
        /// Get list of employees by id
        /// </remarks>
        /// <param name="id">Id of the Department</param>
        /// <returns></returns>
        /// <response code="200">employees found</response>
        /// <response code="404">employees not found</response>
        [ResponseType(typeof(DepartmentEmployee))]
        public HttpResponseMessage GetEmployeeById(int id)
        {
            DepartmentService departmentService = new DepartmentService();
            var departmentEmployees = departmentService.GetDepartmentEmployees(id);
            var departmentInfo = departmentService.GetDepartmentInfo(id);

//            ViewBag.Title = "Employees in " + departmentInfo.Name + " Department";

            return departmentEmployees == null
                ? Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employees not found")
                : Request.CreateResponse(HttpStatusCode.OK, departmentEmployees);
        }

        /// <summary>
        /// Get Department infomation
        /// </summary>
        /// <remarks>
        /// Get Department infomation
        /// </remarks>
        /// <param name="id">department id</param>
        /// <returns></returns>
        /// <response code="201">department information found</response>
        [ResponseType(typeof(DepartmentInfo))]
        public HttpResponseMessage Post(int id)
        {
            DepartmentService departmentService = new DepartmentService();
            var departmentInfo = departmentService.GetDepartmentInfo(id);


            return Request.CreateResponse(HttpStatusCode.Created, departmentInfo);
        }

        // PUT: api/DepartmentApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DepartmentApi/5
        public void Delete(int id)
        {
        }
    }
}
