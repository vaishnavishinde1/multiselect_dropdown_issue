
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.StoredProcedure
{
    public class PurchaseOrderDetailSP : CostDetails
    {
        public string CostType { get; set; }
        public int ProjectElementId { get; set; }
        public string PhaseDescription { get; set; }
        public string BudgetCategory { get; set; }
        public string BudgetSubCategory { get; set; }
        public string CostLineItemID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UniqueIdentityNumber { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string UnitPrice { get; set; }
        public string ClassRefFullName { get; set; }
        public string CustomerRefFullName { get; set; }
        public int? PurchaseOrderID { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public int[] EmployeeID { get; set; }
        //public string EmployeeName { get; set; }


        public static List<PurchaseOrderDetailSP> getPurchaseOrderDetail(int projectID)
        {
            try
            {
                List<PurchaseOrderDetailSP> poDetails = new List<PurchaseOrderDetailSP>();
                List<PurchaseOrderDetail> podetailslist = new List<PurchaseOrderDetail>();
                List<PurchaseOrder> polist = new List<PurchaseOrder>();

                using (var ctx = new CPPDbContext())
                {
                    poDetails = ctx.Database.SqlQuery<PurchaseOrderDetailSP>("call get_purchase_order(@ProjectID)",
                                                   new MySql.Data.MySqlClient.MySqlParameter("@ProjectID", projectID)
                                                   ).ToList();


                    foreach (var j in poDetails)
                    {
                        podetailslist = ctx.PurchaseOrderDetail.Where(x => x.CostCode == j.CostLineItemID && x.Deleted == 1).ToList();

                        string sum = podetailslist.Sum(x => x.Quantity).ToString();
                        string amount = podetailslist.Sum(x => x.Amount).ToString();

                        int convertsum = Convert.ToInt32(sum);
                        int convertamount = Convert.ToInt32(amount);

                        if (convertsum != 0)
                        {
                            j.BalancedAmountOrQuantity = j.BalancedAmountOrQuantity + convertsum;
                        }
                        else if (convertamount != 0)
                        {
                            j.BalancedAmountOrQuantity = j.BalancedAmountOrQuantity + convertamount;
                        }

                    }

                    /*dynamic Results = ctx
                            .MultipleResults($"CALL get_purchase_order ({projectID})")
                            .AddResult<PurchaseOrderDetailSP>()
                            .AddResult<CostDetails>()
                            .Execute();

                    poDetails.AddRange(Results[0]);
                    costDetails.AddRange(Results[1]);

                    for (int i = 0; i < poDetails.Count; i++)
                    {
                        List<CostDetails> details = costDetails.Where(x => x.CostCode == poDetails[i].CostLineItemID).ToList();
                        poDetails[i].CostDetails = details;
                    }*/

                }
                return poDetails;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        public static List<PurchaseOrderDetailSP> getPurchaseOrderIDDetail(int purchaseOrderID)
        {
            try
            {
                List<PurchaseOrderDetailSP> poDetails = new List<PurchaseOrderDetailSP>();
                using (var ctx = new CPPDbContext())
                {
                    poDetails = ctx.Database.SqlQuery<PurchaseOrderDetailSP>("call get_purchase_order_details(@PurchaseOrderID)",
                                                   new MySql.Data.MySqlClient.MySqlParameter("@PurchaseOrderID", purchaseOrderID)
                                                   ).ToList();

                    List<PurchaseOrderDetail> podetailslist = new List<PurchaseOrderDetail>();
                    foreach (var item in poDetails)
                    {
                        podetailslist = ctx.PurchaseOrderDetail.Where(x => x.CostCode == item.CostLineItemID && x.Deleted == 1).ToList();

                        string sum = podetailslist.Sum(x => x.Quantity).ToString();
                        string amount = podetailslist.Sum(x => x.Amount).ToString();

                        int convertsum = Convert.ToInt32(sum);
                        int convertamount = Convert.ToInt32(amount);

                        if (convertsum != 0)
                        {
                            item.BalancedAmountOrQuantity = item.BalancedAmountOrQuantity + convertsum;
                        }
                        else if (convertamount != 0)
                        {
                            item.BalancedAmountOrQuantity = item.BalancedAmountOrQuantity + convertamount;
                        }


                        if (item.CostType == "ODC")
                        {
                            List<int> poList =
                            (from pod in ctx.PurchaseOrderDetail
                             join poe in ctx.POODCEmployeeDetails on pod.ID equals poe.PODetailsID
                             where pod.PurchaseOrderID == item.PurchaseOrderID && pod.CostCode == item.CostLineItemID
                             select
                                 poe.EmpID
                             ).ToList();

                            item.EmployeeID = poList.ToArray();
                        }
                    }

                    /*dynamic Results = ctx
                            .MultipleResults($"CALL get_purchase_order ({projectID})")
                            .AddResult<PurchaseOrderDetailSP>()
                            .AddResult<CostDetails>()
                            .Execute();

                    poDetails.AddRange(Results[0]);
                    costDetails.AddRange(Results[1]);

                    for (int i = 0; i < poDetails.Count; i++)
                    {
                        List<CostDetails> details = costDetails.Where(x => x.CostCode == poDetails[i].CostLineItemID).ToList();
                        poDetails[i].CostDetails = details;
                    }*/

                }
                return poDetails;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
    }
}