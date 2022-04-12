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
    [Table("contract_insurance")]
    public class ContractInsurance
    {
        [NotMapped]
        public int Operation;
        public int Id { get; set; }
        public string Type { get; set; }
        public string Limit { get; set; }
        public int ProgramID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public static void SaveInsurance(ContractInsurance contractInsurance)
        {

            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            List<ContractInsurance> ContractInsurance = new List<ContractInsurance>();

            try
            {
                using (var ctx = new CPPDbContext())
                {
                    contractInsurance.CreatedDate = DateTime.Now;
                    ctx.ContractInsurances.Add(contractInsurance);
                    ctx.SaveChanges();

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
        }

        public static List<ContractInsurance> GetContractInsurances(int programId)
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            List<ContractInsurance> contractInsuranceList = new List<ContractInsurance>();

            Program programid = new Program();
            try
            {
                using (var ctx = new CPPDbContext())
                {

                    contractInsuranceList = ctx.ContractInsurances.Where(u => u.ProgramID == programId).ToList();


                    return contractInsuranceList;

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
            return contractInsuranceList;
        }
    }
}