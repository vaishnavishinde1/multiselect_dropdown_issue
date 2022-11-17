using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class RegisterSpecialInstructionsController : ApiController
    {
        public HttpResponseMessage Post(SpecialInstructions SpecialInstructions)
        {
            String result = "";
            
            if (SpecialInstructions.Operation == 2)
            {
                result += SpecialInstructions.updateSpecialInstructions(SpecialInstructions);
            }
            //else if (SpecialInstructions.Operation == 3)
            //{
            //    result += SpecialInstructions.DeleteSpecialinstructions(SpecialInstructions);
            //}
            
            var jsonNew = new
            {
                result = result,
              
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }
    }
}
