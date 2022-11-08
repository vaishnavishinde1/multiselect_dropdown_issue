using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;
namespace WebAPI.Controllers
{
    public class CostOverheadHistoryController : ApiController
    {
        public HttpResponseMessage Get(int CostID)
        {

            List<CostOverheadHistory> CostoverheadList = new List<CostOverheadHistory>();
            CostoverheadList = WebAPI.Models.CostOverheadHistory.GetCostOverheadHistory(CostID);


            var jsonNew = new
            {
                result = CostoverheadList
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }

        public HttpResponseMessage Get()
        {

            List<CostOverheadHistory> CostoverheadList = new List<CostOverheadHistory>();
            CostoverheadList = WebAPI.Models.CostOverheadHistory.GetAllCostOverheadHistory();


            var jsonNew = new
            {
                result = CostoverheadList
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }
    }
}
