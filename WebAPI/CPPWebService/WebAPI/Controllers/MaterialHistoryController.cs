using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class MaterialHistoryController : ApiController
    {
        public HttpResponseMessage Get(int MaterialID)
        {

            List<MaterialHistory> materialList = new List<MaterialHistory>();
            materialList = WebAPI.Models.MaterialHistory.GetMaterialHistoryByID(MaterialID);


            var jsonNew = new
            {
                result = materialList
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }

        public HttpResponseMessage Get()
        {

            List<MaterialHistory> materialList = new List<MaterialHistory>();
            materialList = WebAPI.Models.MaterialHistory.GetMaterialHistory();


            var jsonNew = new
            {
                result = materialList
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }
    }
}
