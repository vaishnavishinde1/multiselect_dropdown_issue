using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using WebAPI.Helper;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;


namespace WebAPI.Models
{
    [Table("admin_approval_president")]
    public class AdminApprovalPresident : Audit
    {
        [NotMapped]
        public int Operation;
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PresidentID { get; set; }

        public static AdminApprovalPresident GetFirstDefaultPresident()
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            AdminApprovalPresident adminApprovalPresidents = new AdminApprovalPresident();
            using (var ctx = new CPPDbContext())
            {
                try
                {
                    adminApprovalPresidents = ctx.AdminApprovalPresident.FirstOrDefault();

                }
                catch (Exception ex)
                {
                    var stackTrace = new StackTrace(ex, true);
                    var line = stackTrace.GetFrame(0).GetFileLineNumber();
                    Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);
                }
                finally
                {
                }
            }
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Exit Point", Logger.logLevel.Debug);

            return adminApprovalPresidents;
        }

        public static String UpdateApproverPresident(AdminApprovalPresident adminApprovalPresident)
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            String result = "";

            using (var ctx = new CPPDbContext())
            {
                try
                {
                    AdminApprovalPresident retrivedAdminApprovalPresident = new AdminApprovalPresident();
                    retrivedAdminApprovalPresident = ctx.AdminApprovalPresident.Where(p => p.ID.Equals(adminApprovalPresident.ID)).FirstOrDefault();

                    if (retrivedAdminApprovalPresident.Equals(null))
                    {
                        result += "President ID: " + adminApprovalPresident.ID + ", could not be found.\n";
                        //throw new Exception(result);
                    }

                    else if (retrivedAdminApprovalPresident != null)
                    {
                        //  ctx.Entry(myEmp).CurrentValues.SetValues(employee); Good way to update an object but it will overwrite the existing object
                        CopyUtil.CopyFields<AdminApprovalPresident>(adminApprovalPresident, retrivedAdminApprovalPresident);
                        ctx.Entry(retrivedAdminApprovalPresident).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        result += adminApprovalPresident.ID + " has been updated successfully.\n";
                    }
                    else
                    {
                        result += adminApprovalPresident.ID + " failed to be updated, it does not exist.\n";
                    }
                }
                catch (Exception ex)
                {
                    var stackTrace = new StackTrace(ex, true);
                    var line = stackTrace.GetFrame(0).GetFileLineNumber();
                    Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);
                    result = ex.Message;
                }
                finally
                {
                    Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Exit Point", Logger.logLevel.Debug);
                }
            }
            return result;
        }
    }
}
