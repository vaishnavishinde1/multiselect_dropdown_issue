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
        public static void SendSetUpMail(int ContractID)
        {
            try
            {
                using (var ctx = new CPPDbContext())
                {
                    List<ContractProjectManager> contractProjectManagers = ctx.ContractProjectManagers.Where(c => c.ProgramId == ContractID).ToList();
                    Program contract = ctx.Program.Where(c => c.ProgramID == ContractID).FirstOrDefault();
                    for (int i = 0; i < contractProjectManagers.Count; i++)
                    {
                        if (contractProjectManagers[i].IsMailSent != true)
                        {
                            int userID = contractProjectManagers[i].UserId;
                            User user = ctx.User.Where(u => u.Id == userID).FirstOrDefault();
                            WebAPI.Services.MailServices.ContractProjectManagerMail(user.Email, contract.ContractNumber, contract.ProgramName);
                            contractProjectManagers[i].IsMailSent = true;
                        }
                    }
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
