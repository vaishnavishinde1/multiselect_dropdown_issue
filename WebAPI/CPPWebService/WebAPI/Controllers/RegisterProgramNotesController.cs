using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class RegisterProgramNotesController : ApiController
    {
        public HttpResponseMessage Post(ProgramNotes programNotes)
        {
            String result = "";
            
            if (programNotes.Operation == 2)
            {
                result += ProgramNotes.updateProgramNotes(programNotes);
            }
            else if (programNotes.Operation == 3)
            {
                result += ProgramNotes.DeleteProgramNotes(programNotes);
            }
            
            var jsonNew = new
            {
                result = result,
              
            };
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }
    }
}
