using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class RequestSpecialInstructionsController : ApiController
    {
        public HttpResponseMessage Get(int? ProgramElementID = null)
        {

            if (ProgramElementID != null)
            {
                List<SpecialInstructions> specialInstructionsList = SpecialInstructions.getSpecialInstructions((int)ProgramElementID);
                var jsonNew = new
                {
                    result = "success",
                    data = specialInstructionsList
                };
                return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
