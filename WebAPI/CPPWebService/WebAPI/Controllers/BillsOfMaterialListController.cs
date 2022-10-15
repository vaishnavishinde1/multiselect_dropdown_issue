using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;
namespace WebAPI.Controllers
{
    public class BillsOfMaterialListController : ApiController
    {

        public HttpResponseMessage Get(int? ProgramElementID = null)
        {
            if (ProgramElementID != null)
            {
                List<BillsOfMaterialList> billsofmaterialList = BillsOfMaterialList.WeeklyBillsofmaterialList((int)ProgramElementID);
                var jsonNew = new
                {
                    result = "success",
                    data = billsofmaterialList,
                };
                return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public HttpResponseMessage Post([FromBody] List<BillsOfMaterialList> billsOfMaterialLists)
        {

            String status = "";
            foreach (var materialList in billsOfMaterialLists)
            {

                if (materialList.Operation == 2)
                    status += WebAPI.Models.BillsOfMaterialList.updateMaterial(materialList);
                if (materialList.Operation == 4)
                    status += "";


            }

            var jsonNew = new
            {
                result = status
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }


    }
}
