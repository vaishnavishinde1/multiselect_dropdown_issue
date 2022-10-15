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
    [Table("tbltotalbudgetforecastvalue")]
    public class TotalBudgetForecastValue
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTblTot { get; set; }
        public int ContractId { get; set; }
        public int ProjectId { get; set; }
        public int ElementId { get; set; }
        public int ActivityId { get; set; }
        public double TotalBudgetValue { get; set; }
        public double TotalForecastValue { get; set; }

        public double TotalLaborCost { get; set; }
        
        public double TotalMaterialCost { get; set; }

        public double TotalODCCost { get; set; }

        public double TotalSubcontractorCost { get; set; }

        [NotMapped]
        public string update_result { get; set; }

        [NotMapped]
        public bool isLoborChanged { get; set; }

        [NotMapped]
        public bool isMaterialChanged { get; set; }

        [NotMapped]
        public bool isSubcontractorChanged { get; set; }

        [NotMapped]
        public bool isODCChanged { get; set; }
        public static void registerTotalBudgetForecastValue(TotalBudgetForecastValue totalBudgetForecastValue)
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            try
            {
                using (var ctx = new CPPDbContext())
                {
                    TotalBudgetForecastValue retreivedTotalBudgetForecastValue = new TotalBudgetForecastValue();
                    retreivedTotalBudgetForecastValue = ctx.TotalBudgetForecastValue.Where(u => u.ProjectId == totalBudgetForecastValue.ProjectId && u.ActivityId == totalBudgetForecastValue.ActivityId).FirstOrDefault();
                    var costfte = ctx.CostFte.Where(f => f.ActivityID == totalBudgetForecastValue.ActivityId && f.Granularity == "week").ToList();
                    var costlumpsum = ctx.CostLumpsum.Where(f => f.ActivityID == totalBudgetForecastValue.ActivityId && f.Granularity == "week").ToList();
                    var costodc = ctx.CostODC.Where(f => f.ActivityID == totalBudgetForecastValue.ActivityId && f.Granularity == "week").ToList();
                    var costunitcost = ctx.CostUnit.Where(f => f.ActivityID == totalBudgetForecastValue.ActivityId && f.Granularity == "week").ToList();

                    var Project = ctx.Project.Where(p => p.ProjectID == totalBudgetForecastValue.ProjectId).FirstOrDefault();
                    if (retreivedTotalBudgetForecastValue == null)
                    {
                        totalBudgetForecastValue.ContractId = Project.ProgramID;

                        if (totalBudgetForecastValue.isLoborChanged)
                        {
                            double sumOFFTE = costfte.Sum(f => Convert.ToDouble(f.FTEValue));
                            double fteHour = (costfte.Sum(f => Convert.ToDouble(f.FTEHours)))/sumOFFTE;
                            //double fteHour = Convert.ToDouble(costfte.Select(d => d.FTEHours).FirstOrDefault());
                            double fteHourlyRate = Convert.ToDouble(costfte.Select(d => d.RawFTEHourlyRate).FirstOrDefault());
                            double totalFteHour = sumOFFTE * (8 * fteHour);
                            totalBudgetForecastValue.TotalForecastValue = totalFteHour * fteHourlyRate;
                           // totalBudgetForecastValue.TotalForecastValue += totalBudgetForecastValue.TotalLaborCost;
                        }
                        if (totalBudgetForecastValue.isMaterialChanged)
                        {
                            totalBudgetForecastValue.TotalMaterialCost = costunitcost.Sum(c => Convert.ToDouble(c.OriginalCost));
                            //totalBudgetForecastValue.TotalForecastValue += totalBudgetForecastValue.TotalMaterialCost;
                        }
                        if (totalBudgetForecastValue.isODCChanged)
                        {
                            totalBudgetForecastValue.TotalODCCost = costodc.Sum(c => Convert.ToDouble(c.OriginalCost));
                           // totalBudgetForecastValue.TotalODCCost += totalBudgetForecastValue.TotalODCCost;
                        }
                        if (totalBudgetForecastValue.isSubcontractorChanged)
                        {
                            totalBudgetForecastValue.TotalSubcontractorCost = costlumpsum.Sum(c => Convert.ToDouble(c.OriginalCost));
                            //totalBudgetForecastValue.TotalForecastValue += totalBudgetForecastValue.TotalSubcontractorCost;
                        }
                        //register
                        totalBudgetForecastValue.TotalForecastValue = totalBudgetForecastValue.TotalLaborCost + totalBudgetForecastValue.TotalMaterialCost + totalBudgetForecastValue.TotalODCCost + totalBudgetForecastValue.TotalSubcontractorCost;
                        //totalBudgetForecastValue.ActivityId = 0;
                        ctx.TotalBudgetForecastValue.Add(totalBudgetForecastValue);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        if (totalBudgetForecastValue.isLoborChanged)
                        {
                            double sumOFFTE = costfte.Sum(f => Convert.ToDouble(f.FTEValue));
                            double fteHour = (costfte.Sum(f => Convert.ToDouble(f.FTEHours))) / sumOFFTE;
                            //double fteHour = Convert.ToDouble(costfte.Select(d => d.FTEHours).FirstOrDefault());
                            double fteHourlyRate = Convert.ToDouble(costfte.Select(d => d.RawFTEHourlyRate).FirstOrDefault());
                            double totalFteHour = sumOFFTE * (8 * fteHour);
                            retreivedTotalBudgetForecastValue.TotalLaborCost = totalFteHour * fteHourlyRate;
                            //retreivedTotalBudgetForecastValue.TotalLaborCost += totalBudgetForecastValue.TotalLaborCost;
                        }
                        if (totalBudgetForecastValue.isMaterialChanged)
                        {
                            retreivedTotalBudgetForecastValue.TotalMaterialCost = costunitcost.Sum(c => Convert.ToDouble(c.OriginalCost));
                            //totalBudgetForecastValue.TotalMaterialCost = costlumpsum.Sum(c => Convert.ToDouble(c.OriginalCost));
                        }
                        if (totalBudgetForecastValue.isODCChanged)
                        {
                            retreivedTotalBudgetForecastValue.TotalODCCost = costodc.Sum(c => Convert.ToDouble(c.OriginalCost)); 
                        }
                        if (totalBudgetForecastValue.isSubcontractorChanged)
                        {
                            retreivedTotalBudgetForecastValue.TotalSubcontractorCost = costlumpsum.Sum(c => Convert.ToDouble(c.OriginalCost));
                            //retreivedTotalBudgetForecastValue.TotalSubcontractorCost += totalBudgetForecastValue.TotalSubcontractorCost;
                        }
                        retreivedTotalBudgetForecastValue.TotalForecastValue = retreivedTotalBudgetForecastValue.TotalLaborCost + retreivedTotalBudgetForecastValue.TotalMaterialCost + retreivedTotalBudgetForecastValue.TotalODCCost + retreivedTotalBudgetForecastValue.TotalSubcontractorCost;
                        CopyUtil.CopyFields<TotalBudgetForecastValue>(retreivedTotalBudgetForecastValue,totalBudgetForecastValue);
                        ctx.Entry(retreivedTotalBudgetForecastValue).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static string GetProjectForecastValue(int ProjectId)
        {
            string returnForecastVal = "0.00";
            //double forecastValue = 0.00;
            List<TotalBudgetForecastValue> totalBudgetForecastValue = new List<TotalBudgetForecastValue>();
            try
            {
                using (var ctx = new CPPDbContext())
                {
                    totalBudgetForecastValue = ctx.TotalBudgetForecastValue.Where(u => u.ProjectId == ProjectId).ToList();
                    if (totalBudgetForecastValue != null && totalBudgetForecastValue.Count !=0)
                    {
                        returnForecastVal = totalBudgetForecastValue.Sum(a => Convert.ToDouble(a.TotalForecastValue)).ToString();
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
            return returnForecastVal;
        }
    }
}