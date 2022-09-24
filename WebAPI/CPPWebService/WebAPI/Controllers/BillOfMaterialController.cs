using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class BillOfMaterialController : ApiController
    {
        public HttpResponseMessage Get(int? programelementId = null,string SearchText=null)
        {
            if (programelementId != null)
            {
                List<BillOfMaterial> billsofmaterialList = BillOfMaterial.GetBillsofmaterialList((int)programelementId,(string)SearchText);
                var jsonNew = new
                {
                    result = "success",
                    data = billsofmaterialList
                };
                return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        
        public HttpResponseMessage Post(string SearchText,BillOfMaterial billOfMaterialdata)
        {
            String result = "";
            if (billOfMaterialdata.Operation == 1)
            {
                result += BillOfMaterial.registerBillOfMaterial(billOfMaterialdata);
            }
            else if (billOfMaterialdata.Operation == 2)
            {
                result += BillOfMaterial.updateBillOfMaterial(billOfMaterialdata);
            }
            else if (billOfMaterialdata.Operation == 3)
            {
                result += BillOfMaterial.DeleteBillOfMaterial(billOfMaterialdata);
            }
            List<BillOfMaterial> billsofmaterialList = BillOfMaterial.GetBillsofmaterialList(billOfMaterialdata.ProgramElementID,SearchText);
            var jsonNew = new
            {
               result = result,
                data = billsofmaterialList
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }
    }
}
