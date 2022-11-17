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
    [Table("specialinstructions")]
    public class SpecialInstructions : Audit
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [NotMapped]
        public int Operation;

        public int ID { get; set; }

        public int ProgramElementID { get; set; }

        public string specialInstructions_desc { get; set; }

        public int Deleted { get; set; }

        public static List<SpecialInstructions> getSpecialInstructions(int ProgramElementID)
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            List<SpecialInstructions> MatchedSpecialInstructionsList = new List<SpecialInstructions>();
            using (var ctx = new CPPDbContext())
            { 
                try

                {
                    MatchedSpecialInstructionsList = ctx.SpecialInstructions.Where(a => a.ProgramElementID == ProgramElementID).ToList();
                    //MatchedProgramList = test.Select(a => new Program { ProgramElementID = a.ProgramElementID, ProgramName = a.ProgramName }).ToList();
                }

            
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return MatchedSpecialInstructionsList;
                }
        }

        public static string updateSpecialInstructions(SpecialInstructions specialInstructions)
        {

            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            String result = "";

            try
            {
                using (var ctx = new CPPDbContext())
                {
                    SpecialInstructions specialInstructionsLs = new SpecialInstructions();
                    SpecialInstructions retreivespecialInstructions = new SpecialInstructions();
                    retreivespecialInstructions = ctx.SpecialInstructions.Where(p => /*p.ID == specialInstructions.ID &&*/ p.ProgramElementID == specialInstructions.ProgramElementID).FirstOrDefault();
                    if (retreivespecialInstructions != null)
                    {
                        retreivespecialInstructions.specialInstructions_desc = specialInstructions.specialInstructions_desc;
                        ctx.SaveChanges();
                        result = "Updated successfully";
                    }
                    else if(retreivespecialInstructions == null)
                    {
                        specialInstructionsLs.ProgramElementID = specialInstructions.ProgramElementID;
                        specialInstructionsLs.specialInstructions_desc = specialInstructions.specialInstructions_desc;
                        ctx.SpecialInstructions.Add(specialInstructionsLs);
                        ctx.SaveChanges();
                        result = "Created successfully";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var line = stackTrace.GetFrame(0).GetFileLineNumber();
                result = ex.Message;
                Logger.LogExceptions(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, line.ToString(), Logger.logLevel.Exception);

            }
            return result;

        }

        public static string DeleteSpecialInstructions(SpecialInstructions specialInstructions)
        {
            var status = "";

            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            String result = "";

            try
            {
                using (var ctx = new CPPDbContext())
                {
                    SpecialInstructions retreivespecialInstructions = new SpecialInstructions();
                    retreivespecialInstructions = ctx.SpecialInstructions.Where(p => p.ID == specialInstructions.ID).FirstOrDefault();

                    ctx.SpecialInstructions.Remove(retreivespecialInstructions);
                    ctx.SaveChanges();
                    result = "success";

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

    }
}