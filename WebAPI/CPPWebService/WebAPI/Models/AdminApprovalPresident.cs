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
        public int PresidentUserID { get; set; }

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
                    Employee president = ctx.Employee.Where(e => e.ID == adminApprovalPresident.PresidentID).FirstOrDefault();

                    if (retrivedAdminApprovalPresident.Equals(null))
                    {
                        result += "President " + president.FirstName + ", could not be found.\n";
                        //throw new Exception(result);
                    }

                    else if (retrivedAdminApprovalPresident != null)
                    {
                        //  ctx.Entry(myEmp).CurrentValues.SetValues(employee); Good way to update an object but it will overwrite the existing object
                        CopyUtil.CopyFields<AdminApprovalPresident>(adminApprovalPresident, retrivedAdminApprovalPresident);
                        ctx.Entry(retrivedAdminApprovalPresident).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        result += president.FirstName + " has been updated successfully.\n";
                    }
                    else
                    {
                        result += president.FirstName + " failed to be updated, it does not exist.\n";
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


        public static String InsertUserIdInApproverProject()
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            String result = "";

            using (var ctx = new CPPDbContext())
            {
                try
                {
                    List<ProjectApproversDetails> projectApproverDetailsList = ctx.ProjectApproversDetails.ToList<ProjectApproversDetails>();

                    for (int i = 0; i < projectApproverDetailsList.Count; i++)
                    {
                        int employeeID = projectApproverDetailsList[i].EmpId;

                        ProgramElement programElement = WebAPI.Models.ProgramElement.getProgramElement(null, Convert.ToString(projectApproverDetailsList[i].ProjectId), null).FirstOrDefault();
                        if(programElement != null)
                        {
                            List<User> userEmp = ctx.User.Where(u => u.EmployeeID == employeeID
                       && u.DepartmentID == programElement.ProjectClassID).ToList();
                            for (int k = 0; k < userEmp.Count; k++)
                            {
                                int userId = userEmp[k].Id;
                                List<UserRoleRelation> roleWiseUser = new List<UserRoleRelation>();
                                if (projectApproverDetailsList[i].ApproverMatrixId == 1)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 33).ToList();
                                }
                                else if (projectApproverDetailsList[i].ApproverMatrixId == 3)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 26).ToList();
                                }
                                else if (projectApproverDetailsList[i].ApproverMatrixId == 4)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 36).ToList();
                                }
                                else if (projectApproverDetailsList[i].ApproverMatrixId == 5)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 34).ToList();
                                }
                                else if (projectApproverDetailsList[i].ApproverMatrixId == 6)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 28).ToList();
                                }

                                if (roleWiseUser.Count > 0)
                                {
                                    projectApproverDetailsList[i].UserId = userId;
                                    ctx.SaveChanges();
                                }
                            }
                        }
                        
                    }

                    result = "entry updated sucessfully for project table";
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
        public static String InsertUserIdInApproverTrend()
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            String result = "";

            using (var ctx = new CPPDbContext())
            {
                try
                {
                    List<TrendApproversDetails> trendApproverDetailsList = ctx.TrendApproversDetails.ToList<TrendApproversDetails>();

                    for (int i = 0; i < trendApproverDetailsList.Count; i++)
                    {
                        int employeeID = trendApproverDetailsList[i].EmpId;
                        Project project = WebAPI.Models.Project.getProject(null, null, Convert.ToString(trendApproverDetailsList[i].ProjectElementId), null).FirstOrDefault();
                        if (project != null)
                        {
                            ProgramElement programElement = WebAPI.Models.ProgramElement.getProgramElement(null, Convert.ToString(project.ProgramElementID), null).FirstOrDefault();
                            List <User> userEmp = ctx.User.Where(u => u.EmployeeID == employeeID
                            && u.DepartmentID == programElement.ProjectClassID).ToList();
                            for (int k = 0; k < userEmp.Count; k++)
                            {
                                int userId = userEmp[k].Id;
                                List<UserRoleRelation> roleWiseUser = new List<UserRoleRelation>();
                                if (trendApproverDetailsList[i].ApproverMatrixId == 1)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 33).ToList();
                                }
                                else if (trendApproverDetailsList[i].ApproverMatrixId == 3)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 26).ToList();
                                }
                                else if (trendApproverDetailsList[i].ApproverMatrixId == 4)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 36).ToList();
                                }
                                else if (trendApproverDetailsList[i].ApproverMatrixId == 5)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 34).ToList();
                                }
                                else if (trendApproverDetailsList[i].ApproverMatrixId == 6)
                                {
                                    roleWiseUser = ctx.UserRoleRelation.Where(r => r.UserId == userId && r.UserRoleId == 28).ToList();
                                }

                                if (roleWiseUser.Count > 0)
                                {
                                    trendApproverDetailsList[i].UserId = userId;
                                    ctx.SaveChanges();
                                }
                            }
                        }
                    }

                    result = "entry updated sucessfully for trend table";
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
