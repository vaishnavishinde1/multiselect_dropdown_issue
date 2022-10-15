using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using WebAPI.Helper;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;


namespace WebAPI.Models
{
    [Table("admin_approval")]
    public class AdminApproval : Audit
    {
        [NotMapped]
        public int Operation;
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int DepartmentID { get; set; }
       // public int User_Role_ID { get; set; }
        public int DeptManagerID { get; set; }
        public int OpeManagerID { get; set; }

        public static List<AdminApproval> GetAllApprovers()
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            List<AdminApproval> approvalList = new List<AdminApproval>();
            using (var ctx = new CPPDbContext())
            {
                try
                {
                    approvalList = ctx.AdminApproval.ToList();

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

            return approvalList;
        }

        public static String RegisterNewApprover(AdminApproval adminapproval)
        {

            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            String result = "";

            using (var ctx = new CPPDbContext())
            {
                try
                {
                    AdminApproval retreivedadmin = new AdminApproval();
                    retreivedadmin = ctx.AdminApproval.Where(u => u.DepartmentID == adminapproval.DepartmentID).FirstOrDefault(); ;

                    if (retreivedadmin != null)
                    {
                        result += adminapproval.DepartmentID + "' failed to be created, duplicate unique identifier found for this organization.\n";
                    }
                    else if (retreivedadmin == null)
                    {
                        ctx.AdminApproval.Add(adminapproval);
                        ctx.SaveChanges();
                        result += adminapproval.DepartmentID + " has been created successfully.\n";
                    }
                    else
                    {
                        result += adminapproval.DepartmentID + " failed to be created, it already exist in the same position and organization.\n";
                    }
                }
                catch (Exception ex)
                {
                    var stackTrace = new StackTrace(ex, true);
                    var line = stackTrace.GetFrame(0).GetFileLineNumber();
                    Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);
                    //result = ex.Message;
                }
                finally
                {
                    Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Exit Point", Logger.logLevel.Debug);
                }
            }
            return result;
        }


        public static String UpdateApprover(AdminApproval adminApproval)
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            String result = "";

            using (var ctx = new CPPDbContext())
            {
                try
                {
                    AdminApproval retrivedApproval = new AdminApproval();
                    retrivedApproval = ctx.AdminApproval.Where(e => e.ID.Equals(adminApproval.ID)).FirstOrDefault();

                    AdminApproval duplicateApprover = new AdminApproval();
                    duplicateApprover = ctx.AdminApproval.Where(d => d.DepartmentID == adminApproval.DepartmentID
                    && d.DeptManagerID == adminApproval.DeptManagerID
                    && d.OpeManagerID == adminApproval.OpeManagerID).FirstOrDefault();

                    if (retrivedApproval.Equals(null))
                    {
                        result += "Approval ID: " + adminApproval.ID + ", could not be found.\n";
                        //throw new Exception(result);
                    }
                    else if (duplicateApprover != null)
                    {
                        result += adminApproval.ID + "' failed to be updated, duplicate will be created.\n";
                    }
                    else if(retrivedApproval != null)
                    {
                        UpdateApproverHistory(retrivedApproval.ID);
                        //  ctx.Entry(myEmp).CurrentValues.SetValues(employee); Good way to update an object but it will overwrite the existing object
                        CopyUtil.CopyFields<AdminApproval>(adminApproval, retrivedApproval);
                        ctx.Entry(retrivedApproval).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        result += adminApproval.ID + " has been updated successfully.\n";
                    }
                    else
                    {
                        result += adminApproval.ID + " failed to be updated, it does not exist.\n";
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


        public static String DeleteApprover(AdminApproval adminApproval)
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            String result = "";

            using (var ctx = new CPPDbContext())
            {
                try
                {
                    AdminApproval retrivedApproval = new AdminApproval();
                    retrivedApproval = ctx.AdminApproval.Where(e => e.ID.Equals(adminApproval.ID)).FirstOrDefault();
                    if (retrivedApproval.Equals(null))
                    {
                        result = adminApproval.ID + " failed to be deleted, it does not exist.\n";
                    }
                    else
                    {
                        ctx.AdminApproval.Remove(retrivedApproval);
                        ctx.SaveChanges();
                        result = adminApproval.ID + " has been deleted successfully.\n";
                    }
                }
                catch (Exception ex)
                {
                    result += adminApproval.ID + " failed to be deleted due to dependencies.\n";
                    var stackTrace = new StackTrace(ex, true);
                    var line = stackTrace.GetFrame(0).GetFileLineNumber();
                    Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);
                    //result = ex.Message;
                }
                finally
                {
                }
                Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Exit Point", Logger.logLevel.Debug);

            }
            return result;
        }

        public static String UpdateApproverHistory(int approverID)
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            String result = "";

            using (var ctx = new CPPDbContext())
            {
                try
                {
                    AdminApproval retrivedApproval = new AdminApproval();
                    retrivedApproval = ctx.AdminApproval.Where(e => e.ID.Equals(approverID)).FirstOrDefault();

                    ProjectClass projectClass = ctx.ProjectClass.Where(p => p.ProjectClassID == retrivedApproval.DepartmentID).FirstOrDefault();
                    Employee departmentManager = ctx.Employee.Where(e => e.ID == retrivedApproval.DeptManagerID).FirstOrDefault();
                    Employee operationManager = ctx.Employee.Where(e => e.ID == retrivedApproval.OpeManagerID).FirstOrDefault();


                    ApprovalHistory approvalHistory = new ApprovalHistory
                    {
                        Department = projectClass.ProjectClassName,
                        DepartmentManager = departmentManager.Name,
                        OperationManager = operationManager.Name,
                        UpdatedBy = retrivedApproval.UpdatedBy,
                        FromDate = retrivedApproval.CreatedDate,
                        ToDate = DateTime.UtcNow
                    };

                    ctx.ApprovalHistory.Add(approvalHistory);
                    ctx.SaveChanges();

                    result = "entry updated sucessfully";
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