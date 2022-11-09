


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
    public class RegisterAdminApprovalController : ApiController
    {
        //
        // GET: /RegisterUser/
        public HttpResponseMessage Post([FromBody] List<AdminApproval> adminApprovalList)
        {

            String status = "";
            foreach (var AdminApproval in adminApprovalList)
            {
                switch (AdminApproval.Operation)
                {
                    case 1:
                        status += WebAPI.Models.AdminApproval.RegisterNewApprover(AdminApproval);
                        break;
                    case 2:
                        status += WebAPI.Models.AdminApproval.UpdateApprover(AdminApproval);
                        break;
                    case 3:
                        status += WebAPI.Models.AdminApproval.DeleteApprover(AdminApproval);
                        break;
                    case 4:
                        status += "";
                        break;
                }

            }
            var jsonNew = new
            {
                result = status
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }


        public HttpResponseMessage Get(int? actionNo)
        {
            String status = "";
            if (actionNo == 1001)
            {
                status += AdminApprovalPresident.InsertUserIdInApproverProject();
            }
            else if (actionNo == 2002)
            {
                status += AdminApprovalPresident.InsertUserIdInApproverTrend();
            }
            var jsonNew = new
            {
                result = status
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }
    }
}



















//    {
//        String status = "";
//            foreach (var admin_approval in admin_approval)
//            {
//                switch (admin_approval.Operation)
//                {
//                    case 1:
//                        status += WebAPI.Models.admin_approval.add(admin_approval);
//                        break;
//                    case 2:
//                        status += WebAPI.Models.admin_approval.update(admin_approval);
//                        break;
//                    case 3:
//                        status += WebAPI.Models.admin_approval.delete(admin_approval);
//                        break;
//                    case 4:
//                        status += "";
//                        break;
//                }
//    //i
//}
//var jsonNew = new
//{
//    result = status
//};
//return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
//        }
//    }
//}
