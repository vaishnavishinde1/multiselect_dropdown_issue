using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using WebAPI.Helper;
using System.Diagnostics;

namespace WebAPI.Models
{
    [Table("contract_project_manager")]
    public class ContractProjectManager
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProgramId { get; set; }
        public bool IsMailSent { get; set; }


        // Narayan - Send Mail To Project Manager from Contract
        public static void SendSetUpMail(int ContractID, int id, string ModifiedValue)
        {
            try
            {
                using (var ctx = new CPPDbContext())
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
