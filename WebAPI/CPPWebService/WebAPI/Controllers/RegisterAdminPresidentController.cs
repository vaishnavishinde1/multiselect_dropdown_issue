using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Http;

using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class RegisterAdminPresidentController : ApiController
    {

        public HttpResponseMessage Get()
        {
            AdminApprovalPresident adminApprovalPresident = AdminApprovalPresident.GetFirstDefaultPresident();
            List<ApprovalHistory> approvalHistories = ApprovalHistory.GetApproversHistory();
            var jsonNew = new
            {
                result = "success",
                presidentData = adminApprovalPresident,
                approvalHistory = approvalHistories
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }

        // GET: RegisterAdminPresident
        public HttpResponseMessage Post([FromBody] AdminApprovalPresident adminApprovalPresident)
        {
            String status = "";
            if (adminApprovalPresident.Operation == 2)
            {
                status += AdminApprovalPresident.UpdateApproverPresident(adminApprovalPresident);
            }
            var jsonNew = new
            {
                result = status
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }
    }
}