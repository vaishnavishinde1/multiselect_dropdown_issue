using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Models
{
    public class NotifyChangeApproversMail
    {
        public static void ToPM(int ElementID, int userId, int empID, string designation)
        {
            try
            {
                using (var ctx = new CPPDbContext())
                {
                    Employee employee = ctx.Employee.Where(e => e.ID == empID).FirstOrDefault();
                    User user = ctx.User.Where(u => u.Id == userId).FirstOrDefault();
                    Project element = ctx.Project.Where(e => e.ProjectID == ElementID).FirstOrDefault();

                    //WebAPI.Services.MailServices.ProjectManagerChangeApprovalMail(user.Email, element.ProjectName, designation, employee.Name);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
