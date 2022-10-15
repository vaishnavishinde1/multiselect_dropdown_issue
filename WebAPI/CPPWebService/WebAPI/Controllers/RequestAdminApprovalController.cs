using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Web.Http;

using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class RequestAdminApprovalController : ApiController
    {
        // GET: RequestAdminApproval
        public HttpResponseMessage Get()
        {

            List<AdminApproval> AllApproversList = new List<AdminApproval>();
            AllApproversList = WebAPI.Models.AdminApproval.GetAllApprovers();


            var jsonNew = new
            {
                result = AllApproversList
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }
    }
}