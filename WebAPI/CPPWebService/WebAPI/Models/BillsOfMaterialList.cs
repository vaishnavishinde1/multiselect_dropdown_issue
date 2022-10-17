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
    [Table("billsofmateriallist")]
    public class BillsOfMaterialList:Audit
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [NotMapped]
        public int Operation;

        public int Id { get; set; }

        public String MaterialName { get; set; }
        public int ProgramElementID { get; set; }

        public String MaterialCategoryName { get; set; }

        public DateTime UnitCostStartDate { get; set; }
        public DateTime UnitCostEndDate { get; set; }
        public DateTime Make_Available_by { get; set; }
        public String UnitQuantity { get; set; }
        
        public String Status { get; set; }

        public String Note { get; set; }


        public static List<BillsOfMaterialList> WeeklyBillsofmaterialList(int ProgramElementID)
        {
            var result = "";
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            List<BillsOfMaterialList> billOfMaterialslist = new List<BillsOfMaterialList>();
           

            try
            {
                using (var ctx = new CPPDbContext())
                {
                    CPPDbContext db = new CPPDbContext();



                    MySqlConnection conn = null;
                    MySqlDataReader reader = null;
                    conn = ConnectionManager.getConnection();
                    conn.Open();
                    String query = "get_bill_of_material_list_weekly";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.CommandType = CommandType.StoredProcedure;
                    //command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("_ProgramElementID", ProgramElementID);

                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BillsOfMaterialList billOfMaterials = new BillsOfMaterialList();

                            billOfMaterials.UnitQuantity = reader["UnitQuantity"].ToString();
                            billOfMaterials.UnitCostStartDate = DateTime.Parse(reader["UnitCostStartDate"].ToString());
                            billOfMaterials.UnitCostEndDate = DateTime.Parse(reader["UnitCostEndDate"].ToString());
                            billOfMaterials.Make_Available_by = DateTime.Parse(reader["Make_Available_by"].ToString());
                            billOfMaterials.MaterialCategoryName = reader["MaterialCategoryName"].ToString();
                            billOfMaterials.MaterialName = reader["MaterialName"].ToString();

                            billOfMaterialslist.Add(billOfMaterials);


                        }
                    }


                    foreach (var i in billOfMaterialslist)
                    {

                        BillsOfMaterialList billsOfMaterialList = new BillsOfMaterialList();
                        BillsOfMaterialList isMaterialavailable = new BillsOfMaterialList();
                        isMaterialavailable = ctx.BillsOfMaterialList.Where(b => b.ProgramElementID == ProgramElementID && b.MaterialName == i.MaterialName && b.UnitCostStartDate == i.UnitCostStartDate && b.UnitCostEndDate == i.UnitCostEndDate).FirstOrDefault();
                        billsOfMaterialList.ProgramElementID = ProgramElementID;
                        billsOfMaterialList.MaterialName = i.MaterialName;
                        billsOfMaterialList.UnitCostStartDate = i.UnitCostStartDate;
                        billsOfMaterialList.UnitCostEndDate = i.UnitCostEndDate;
                        billsOfMaterialList.UnitQuantity = i.UnitQuantity;
                        billsOfMaterialList.Make_Available_by = i.Make_Available_by;
                        billsOfMaterialList.Status = "Pending";
                        if (isMaterialavailable == null)
                        {
                            ctx.BillsOfMaterialList.Add(billsOfMaterialList);
                        }
                        if (isMaterialavailable != null)
                        {
                            if (isMaterialavailable.Status == "Pending" && isMaterialavailable.UnitQuantity != i.UnitQuantity)
                            {
                                isMaterialavailable.UnitQuantity = i.UnitQuantity;

                            }
                        }

                    }
                    ctx.SaveChanges();

                    billOfMaterialslist = ctx.BillsOfMaterialList.Where(b => b.ProgramElementID == ProgramElementID && b.Status!= "Fully Received").ToList();




                    result = "success";
                    return billOfMaterialslist;

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
            return billOfMaterialslist;
        }

        public static String updateMaterial(BillsOfMaterialList billsOfMaterialList)
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            String result = "";
            List<BillsOfMaterialList> billsOfMaterialLists = new List<BillsOfMaterialList>();

            try
            {
                using (var ctx = new CPPDbContext())
                {
                    BillsOfMaterialList retrievebillofmaterial = new BillsOfMaterialList();
                    retrievebillofmaterial= ctx.BillsOfMaterialList.Where(u => u.Id == billsOfMaterialList.Id).FirstOrDefault();

                    if (retrievebillofmaterial != null)
                    {
                        retrievebillofmaterial.Status = billsOfMaterialList.Status;
                        retrievebillofmaterial.Note = billsOfMaterialList.Note;
                        ctx.SaveChanges();
                        result += retrievebillofmaterial.MaterialName + " has been updated successfully.\n";
                    }
                  

                }
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
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Exit Point", Logger.logLevel.Debug);

            return result;

        }

    }
}