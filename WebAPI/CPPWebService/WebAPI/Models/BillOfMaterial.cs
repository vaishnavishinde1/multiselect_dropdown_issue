using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.Data.OleDb;
using WebAPI.Controllers;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebAPI.Helper;

namespace WebAPI.Models
{
    [Table("billofmaterials")]
    public class BillOfMaterial : Audit
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [NotMapped]
        public int Operation;

        public int Id { get; set; }

        public int ProgramElementID { get; set; }

        public int MaterialCategoryID { get; set; }

        public String MaterialCategoryName { get; set; }

        public int MaterialID { get; set; }

        public String MaterialName { get; set; }

        public String Description { get; set; }

        public int ManufacturerID { get; set; }

        public String ManufacturerName { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalQuantity { get; set; } // Narayan - Total Quantity
        public String Unit_Type { get; set; }

        public String CostPerItem { get; set; }

        public String Contract_Warranty { get; set; }
        public int Deleted { get; set; }


        public static string registerBillOfMaterial(BillOfMaterial billOfMaterialdata)
        {
            var status = "";
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            try
            {
                using (var ctx = new CPPDbContext())
                {
                    BillOfMaterial retreivebillofMaterial = new BillOfMaterial();
                    retreivebillofMaterial = ctx.BillOfMaterial.Where(p => p.ProgramElementID == billOfMaterialdata.ProgramElementID && p.MaterialCategoryID == billOfMaterialdata.MaterialCategoryID && p.MaterialID == billOfMaterialdata.MaterialID && p.Deleted == 0).FirstOrDefault();

                    billOfMaterialdata.TotalQuantity = billOfMaterialdata.Quantity; // Narayan - Handle Total Quantity

                    if (retreivebillofMaterial != null)
                    {

                        status = "Bill of Material Failed to Add its Already Exist.\n";
                    }
                    else
                    {
                        ctx.BillOfMaterial.Add(billOfMaterialdata);
                        ctx.SaveChanges();
                        status = "Bill of Material Added Successfully!!!";
                    }





                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var line = stackTrace.GetFrame(0).GetFileLineNumber();
                Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);

            }
            return status;
        }

        public static string updateBillOfMaterial(BillOfMaterial billOfMaterialdata)
        {
            
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            String result = "";

            try
            {
                using (var ctx = new CPPDbContext())
                {
                    BillOfMaterial retreivebillofMaterial = new BillOfMaterial();
                    BillOfMaterial duplicatebillofMaterial = new BillOfMaterial();
                    duplicatebillofMaterial = ctx.BillOfMaterial.Where(p => p.ProgramElementID == billOfMaterialdata.ProgramElementID && p.MaterialCategoryID == billOfMaterialdata.MaterialCategoryID && p.MaterialID == billOfMaterialdata.MaterialID && p.Deleted == 0).FirstOrDefault();
                    retreivebillofMaterial= ctx.BillOfMaterial.Where(p => p.Id == billOfMaterialdata.Id).FirstOrDefault();

                    retreivebillofMaterial.TotalQuantity += billOfMaterialdata.Quantity - retreivebillofMaterial.Quantity; // Narayan - Handle Total Quantity 
                    billOfMaterialdata.TotalQuantity = retreivebillofMaterial.TotalQuantity; // Narayan - Assign Total Quantity 

                    if (billOfMaterialdata.Id == retreivebillofMaterial.Id && billOfMaterialdata.MaterialCategoryID == retreivebillofMaterial.MaterialCategoryID && billOfMaterialdata.ProgramElementID == retreivebillofMaterial.ProgramElementID
                            && billOfMaterialdata.MaterialID == retreivebillofMaterial.MaterialID && billOfMaterialdata.Description == retreivebillofMaterial.Description && billOfMaterialdata.ManufacturerID == retreivebillofMaterial.ManufacturerID
                            && billOfMaterialdata.Quantity == retreivebillofMaterial.Quantity && billOfMaterialdata.Unit_Type == retreivebillofMaterial.Unit_Type && billOfMaterialdata.CostPerItem == retreivebillofMaterial.CostPerItem
                            && billOfMaterialdata.Contract_Warranty == retreivebillofMaterial.Contract_Warranty)
                    {
                        result += "No Changes Found.\n";
                    }
                    else if (duplicatebillofMaterial == null || (duplicatebillofMaterial != null && duplicatebillofMaterial.Id == billOfMaterialdata.Id))
                    {
                        CopyUtil.CopyFields<BillOfMaterial>(billOfMaterialdata, retreivebillofMaterial);
                        ctx.Entry(retreivebillofMaterial).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        result = "Bill of Material has been updated successfully.\n";
                    }
                    else
                    {
                        result += "Bill of Material Failed to Update its Already Exist.\n";
                    }





                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var line = stackTrace.GetFrame(0).GetFileLineNumber();
                Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);

            }
            return result;
           
        }

        public static List<BillOfMaterial> GetBillsofmaterialList(int PragramelementId,string SearchText)
        {
            var result = "";
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            List<BillOfMaterial> billsofmaterialList = new List<BillOfMaterial>();

            Program programid = new Program();
            try
            {
                using (var ctx = new CPPDbContext())
                {
                    billsofmaterialList = ctx.BillOfMaterial.Where(u => u.ProgramElementID == PragramelementId && u.Deleted == 0).ToList();
                    if (SearchText != null)
                    {
                        billsofmaterialList = billsofmaterialList.Where(x => x.MaterialCategoryName.ToLower().Contains(SearchText.ToLower())
                        || x.MaterialName.ToLower().Contains(SearchText.ToLower())
                        || x.Description.ToLower().Contains(SearchText.ToLower())
                        || x.Quantity.ToString().ToLower().Contains(SearchText.ToLower())
                        || x.CostPerItem.ToString().ToLower().Contains(SearchText.ToLower())
                        || x.ManufacturerName.ToLower().Contains(SearchText.ToLower())
                        || x.Unit_Type.ToLower().Contains(SearchText.ToLower())
                        || x.Contract_Warranty.ToLower().Contains(SearchText.ToLower())
                        ).ToList();

                    }

                    result = "success";
                    return billsofmaterialList;

                }
            }
            catch (Exception ex)
            {
                var info = ex.ToString();
                var stackTrace = new StackTrace(ex, true);
                var line = stackTrace.GetFrame(0).GetFileLineNumber();
                Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);
            }
            finally
            {
            }
            return billsofmaterialList;
        }

        public static string DeleteBillOfMaterial(BillOfMaterial billOfMaterialdata)
        {
            var status = "";

            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            String result = "";

            try
            {
                using (var ctx = new CPPDbContext())
                {
                    BillOfMaterial retreivebillofMaterial = new BillOfMaterial();
                    retreivebillofMaterial = ctx.BillOfMaterial.Where(p => p.Id == billOfMaterialdata.Id).FirstOrDefault();
                    billOfMaterialdata = ctx.BillOfMaterial.Where(p => p.Id == billOfMaterialdata.Id).FirstOrDefault();

                    if (retreivebillofMaterial != null)
                    {
                        if (retreivebillofMaterial.Quantity == retreivebillofMaterial.TotalQuantity) // Narayan - Check is bill of material is in use
                        {
                            billOfMaterialdata.Deleted = 1;
                            CopyUtil.CopyFields<BillOfMaterial>(billOfMaterialdata, retreivebillofMaterial);
                            ctx.Entry(retreivebillofMaterial).State = System.Data.Entity.EntityState.Modified;
                            ctx.SaveChanges();
                            result = "Bill of Material has been deleted successfully.\n";
                        }
                        else
                        {
                            result = "Material is in use.\n"; // Narayan - Show message bill of material is in use
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var line = stackTrace.GetFrame(0).GetFileLineNumber();
                Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);

            }
            return result;
            //return status;
        }

        // Aditya 26092022 :: Fetch Project Manager
        public static string getProjectManager(int PragramelementId)
        {

            var result = "";
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);

            String projectManager = "";

            Program programid = new Program();
            try
            {
                using (var ctx = new CPPDbContext())
                {
                    
                    ApprovalMatrix projectManagerApproval = ctx.ApprovalMatrix.Where(a => a.Role == "Project Manager").FirstOrDefault();
                    ProjectApproversDetails ApproversDetails = ctx.ProjectApproversDetails.Where(p => p.ProjectId == PragramelementId && p.ApproverMatrixId == projectManagerApproval.Id).FirstOrDefault();
                    //Employee employeData = ctx.Employee.Where(e => e.ID == ApproversDetails.EmpId).FirstOrDefault();
                    
                    if(ApproversDetails.EmpId != 10000) {
                        projectManager = ApproversDetails.EmpName;
                    }
                    else
                    {
                        projectManager = "TBD";
                    }
                }
            }
            catch (Exception ex)
            {
                var info = ex.ToString();
                var stackTrace = new StackTrace(ex, true);
                var line = stackTrace.GetFrame(0).GetFileLineNumber();
                Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);
            }
            finally
            {
            }
            return projectManager;
            
        }

    }
}