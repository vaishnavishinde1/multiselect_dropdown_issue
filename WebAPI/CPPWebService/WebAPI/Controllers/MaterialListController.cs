using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;
namespace WebAPI.Controllers
{
    public class MaterialListController : ApiController
    {
        public HttpResponseMessage Get(int? projectId = null,int? activityId=null)
        {
            if (projectId != null)
            {
                BillOfMaterial materiallist = BillOfMaterial.GeMaterialList((int)projectId,(int)activityId);

                var jsonNew = new
                {
                    result = "success",
                    data = materiallist,
                };
                return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
