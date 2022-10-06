using System;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Http;

using WebAPI.Models;
using System.Diagnostics;

namespace WebAPI.Controllers
{
    [Authorize]
    public class RegisterCostController : System.Web.Http.ApiController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /RegisterFTECost/
        //public HttpResponseMessage Get(int Operation, FTECost.String ProgramID = "null", FTECost.String ProgramElementID = "null", FTECost.String ProjectID = "null", FTECost.String TrendNumber = "null", FTECost.String ActivityID = "null", FTECost.String FTECostID = "null", FTECost.String FTEStartDate = "null", FTECost.String FTEEndDate = "null", FTECost.String FTEPosition = "null", FTECost.String FTEValue = "null", FTECost.String FTEHourlyRate = "null", FTECost.String FTEHours = "null", FTECost.String FTECost = "null")
        public HttpResponseMessage Post([FromBody] List<Cost> CostRow)
        {
            TotalBudgetForecastValue objTotalBudgetForecastValue = new TotalBudgetForecastValue();
            //double TotalFTECost=0,TotalLCost=0,TotalMCost=0,TotalOCost=0;
            //int ContractId = Convert.ToInt32(CostRow[1].ProgramID);
            objTotalBudgetForecastValue.isLoborChanged = false;
            objTotalBudgetForecastValue.isSubcontractorChanged = false;
            objTotalBudgetForecastValue.isMaterialChanged = false;
            objTotalBudgetForecastValue.isODCChanged = false;
            objTotalBudgetForecastValue.ElementId = Convert.ToInt32(CostRow[0].ProjectID);//255 Element ID table project
            var ctx = new CPPDbContext();
            var proj = ctx.Project.Where(a => a.ProjectID == objTotalBudgetForecastValue.ElementId).FirstOrDefault();
            objTotalBudgetForecastValue.ProjectId = proj.ProgramElementID;
            //int ElementId = Convert.ToInt32(CostRow[1].ProjectID);
            objTotalBudgetForecastValue.ActivityId = Convert.ToInt32(CostRow[0].ActivityID);

            String status = "";
            Stopwatch clock = Stopwatch.StartNew();
            foreach (var cost in CostRow)
            {
                if (cost.Operation == 4)
                {
                    TotalBudgetForecastValue objTotalBudgetForecastValue2 = new TotalBudgetForecastValue();

                    //Labor
                    if (cost.CostType == "F")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostFTE.updateMultipleCostFTE(cost.Operation, cost.ProgramID, cost.ProgramElementID,
                                                                            cost.ProjectID, cost.TrendNumber, cost.ActivityID,
                                                                            cost.CostID, cost.StartDate, cost.EndDate,
                                                                            cost.Description, cost.TextBoxValue, cost.Base,
                                                                            cost.FTEHours, cost.FTECost, cost.Scale,
                                                                           cost.FTEIDList, cost.Drag_Direction, cost.NumberOfTextboxToBeRemoved, cost.EmployeeID);
                        objTotalBudgetForecastValue.TotalLaborCost = objTotalBudgetForecastValue2.TotalLaborCost;
                        objTotalBudgetForecastValue.isLoborChanged = true;
                    }
                    //Subcontractor
                    if (cost.CostType == "L")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostLumpsum.updateMultipleCostLumpsum(cost.Operation, cost.ProgramID, cost.ProgramElementID,
                                                                                cost.ProjectID, cost.TrendNumber, cost.ActivityID,
                                                                                cost.CostID, cost.StartDate, cost.EndDate,
                                                                                cost.Description, cost.TextBoxValue, cost.Scale,
                                                                                cost.FTEIDList, cost.Drag_Direction, cost.NumberOfTextboxToBeRemoved, 1, cost.CostLineItemID); //1 - CostTrackTypeID  //Manasi 31-08-2020
                        objTotalBudgetForecastValue.TotalSubcontractorCost = objTotalBudgetForecastValue2.TotalSubcontractorCost;
                        objTotalBudgetForecastValue.isSubcontractorChanged = true;
                    }
                    //Material
                    if (cost.CostType == "U")
                    { 
                    objTotalBudgetForecastValue2 = WebAPI.Models.CostUnit.updateMultipleCostUnit(cost.Operation, cost.ProgramID, cost.ProgramElementID,
                                                                            cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID,
                                                                            cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue,
                                                                            cost.Base, cost.Scale, cost.FTEIDList, cost.Drag_Direction, cost.UnitType, cost.NumberOfTextboxToBeRemoved, 1, cost.CostLineItemID);//1 CostTrackTypeID  //Manasi 31-08-2020
                    objTotalBudgetForecastValue.TotalMaterialCost = objTotalBudgetForecastValue2.TotalMaterialCost;
                    objTotalBudgetForecastValue.isMaterialChanged = true;
                    }
                    //ODC
                    if (cost.CostType == "ODC")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostODC.updateMultipleCostODC(cost.Operation, cost.ProgramID, cost.ProgramElementID,
                                                                                cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID,
                                                                                cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue,
                                                                                cost.Base, cost.Scale, cost.FTEIDList, cost.Drag_Direction, cost.UnitType, cost.NumberOfTextboxToBeRemoved, 1, cost.CostLineItemID);
                        objTotalBudgetForecastValue.TotalODCCost = objTotalBudgetForecastValue2.TotalODCCost;
                        objTotalBudgetForecastValue.isODCChanged = true;
                    }
                }
                else if (cost.Operation == 5)
                {
                    TotalBudgetForecastValue objTotalBudgetForecastValue2 = new TotalBudgetForecastValue();
                    //update on activity date change = (left-left and right-right)
                    if (cost.CostType == "F")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostFTE.updateCostFTELeftLeft(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Base, cost.FTEHours, cost.FTECost, cost.Scale, cost.FTEIDList, cost.EmployeeID, cost.CostLineItemID, cost.CostTrackTypeID);
                        objTotalBudgetForecastValue.TotalLaborCost = objTotalBudgetForecastValue2.TotalLaborCost;
                        objTotalBudgetForecastValue.isLoborChanged = true;
                    }
                    if (cost.CostType == "L")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostLumpsum.updateCostLumpsumLeftLeft(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Scale, cost.FTEIDList, cost.SubcontractorTypeID, cost.SubcontractorID, 1, cost.CostLineItemID); //1 - CostTrackTypeID
                        objTotalBudgetForecastValue.TotalSubcontractorCost = objTotalBudgetForecastValue2.TotalSubcontractorCost;
                        objTotalBudgetForecastValue.isSubcontractorChanged = true;
                    }

                    if (cost.CostType == "U")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostUnit.updateUnitCostLeftLeft(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Base, cost.Scale, cost.UnitType, cost.FTEIDList, cost.MaterialCategoryID, cost.MaterialID, cost.CostLineItemID, 1);//1 - CostTracTypeID
                        objTotalBudgetForecastValue.TotalMaterialCost = objTotalBudgetForecastValue2.TotalMaterialCost;
                        objTotalBudgetForecastValue.isMaterialChanged = true;
                    }
                    if (cost.CostType == "ODC")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostODC.updateODCCostLeftLeft(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Base, cost.Scale, cost.UnitType, cost.FTEIDList, cost.ODCTypeID, cost.CostLineItemID, cost.CostTrackTypeID);
                        objTotalBudgetForecastValue.TotalODCCost = objTotalBudgetForecastValue2.TotalODCCost;
                        objTotalBudgetForecastValue.isODCChanged = true;
                    }


                }


                else
                {
                    // var uniqueDuplicateErrorMessage = "";
                    TotalBudgetForecastValue objTotalBudgetForecastValue2 = new TotalBudgetForecastValue();
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (cost.CostType == "F")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostFTE.updateCostFTE(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Base, cost.FTEHours, cost.FTECost, cost.Scale, cost.FTEIDList, cost.EmployeeID, cost.CostTrackTypes, cost.CostLineItemID);
                        objTotalBudgetForecastValue.TotalLaborCost = objTotalBudgetForecastValue2.TotalLaborCost;
                        objTotalBudgetForecastValue.isLoborChanged = true;
                        //uniqueDuplicateErrorMessage = WebAPI.Models.CostFTE.updateCostFTE(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Base, cost.FTEHours, cost.FTECost, cost.Scale, cost.FTEIDList, cost.EmployeeID, cost.CostTrackTypes, cost.CostLineItemID);
                    }
                    if (cost.CostType == "L")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostLumpsum.updateCostLumpsum(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Scale, cost.FTEIDList, cost.SubcontractorTypeID, cost.SubcontractorID, cost.CostTrackTypes, cost.CostLineItemID);
                        objTotalBudgetForecastValue.TotalSubcontractorCost = objTotalBudgetForecastValue2.TotalSubcontractorCost;
                        objTotalBudgetForecastValue.isSubcontractorChanged = true;
                        //uniqueDuplicateErrorMessage = WebAPI.Models.CostLumpsum.updateCostLumpsum(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Scale, cost.FTEIDList, cost.SubcontractorTypeID, cost.SubcontractorID, cost.CostTrackTypes, cost.CostLineItemID);
                    }
                    if (cost.CostType == "U")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostUnit.updateCostUnit(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Base, cost.Scale, cost.UnitType, cost.FTEIDList, cost.MaterialCategoryID, cost.MaterialID, cost.CostTrackTypes, cost.CostLineItemID);
                        objTotalBudgetForecastValue.TotalMaterialCost = objTotalBudgetForecastValue2.TotalMaterialCost;
                        objTotalBudgetForecastValue.isMaterialChanged = true;
                        //uniqueDuplicateErrorMessage = WebAPI.Models.CostUnit.updateCostUnit(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Base, cost.Scale, cost.UnitType, cost.FTEIDList, cost.MaterialCategoryID, cost.MaterialID, cost.CostTrackTypes, cost.CostLineItemID);
                    }
                    if (cost.CostType == "ODC")
                    {
                        objTotalBudgetForecastValue2 = WebAPI.Models.CostODC.updateCostODC(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Base, cost.Scale, cost.UnitType, cost.FTEIDList, cost.CostTrackTypes, cost.CostLineItemID);
                        objTotalBudgetForecastValue.TotalODCCost = objTotalBudgetForecastValue2.TotalODCCost;
                        objTotalBudgetForecastValue.isODCChanged = true;
                        //uniqueDuplicateErrorMessage = WebAPI.Models.CostODC.updateCostODC(cost.Operation, cost.ProgramID, cost.ProgramElementID, cost.ProjectID, cost.TrendNumber, cost.ActivityID, cost.CostID, cost.StartDate, cost.EndDate, cost.Description, cost.TextBoxValue, cost.Base, cost.Scale, cost.UnitType, cost.FTEIDList, cost.CostTrackTypes, cost.CostLineItemID);
                    }

                    //if (!status.Contains(uniqueDuplicateErrorMessage))
                    //{
                    //    status += uniqueDuplicateErrorMessage;
                    //}
                    if (objTotalBudgetForecastValue2.update_result != null)
                    {
                        if (!status.Contains(objTotalBudgetForecastValue2.update_result))
                        {
                            status += objTotalBudgetForecastValue2.update_result;
                        }
                    }
                   
                    stopwatch.Stop();
                    logger.Debug("Save Cost elapsed time " + stopwatch.ElapsedMilliseconds + " for cost type " + cost.CostType);

                }

            }
            var jsonNew = new
            {
                result = status
            };
            TotalBudgetForecastValue.registerTotalBudgetForecastValue(objTotalBudgetForecastValue);

            clock.Stop();
            logger.Debug("Saving cost elapsed time : " + clock.Elapsed.TotalMilliseconds);
            return Request.CreateResponse(HttpStatusCode.OK, jsonNew);
        }
    }
}