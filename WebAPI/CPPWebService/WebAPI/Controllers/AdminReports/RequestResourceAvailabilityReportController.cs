using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Controllers.AdminReports
{

    public class resourcereport
    {
       [NotMapped]
      public string PositionID { get; set; }
        [NotMapped]
        public string FromDate { get; set; }
        [NotMapped]
        public string ToDate { get; set; }
        [NotMapped]
        public string FileType { get; set; }
       
    }
    public class RequestResourceAvailabilityReportController : System.Web.Http.ApiController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HttpResponseMessage Post(resourcereport resourcereport)
        {
            //http://devapps2.birdi-inc.io/ReportServer/Pages/ReportViewer.aspx?%2fIMS%2finterface2rdl&rs:Command=Render
            log4net.Config.XmlConfigurator.Configure();
            HttpResponseMessage result = null;
            try
            {
                Byte[] bytes = { };
                bytes = generatePDF(resourcereport.PositionID, resourcereport.FromDate, resourcereport.ToDate, resourcereport.FileType, "ResourceReport");
                String base64txt = Convert.ToBase64String(bytes);
                result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StringContent(base64txt);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // new MediaTypeHeaderValue("application/pdf");
                //  new MediaTypeHeaderValue("application/octet-stream");
            }
            catch (Exception ex)
            {
                logger.Error(ex.StackTrace);
            }
            return result;
        }

        private Byte[] generatePDF(String PositionID, String FromDate, String ToDate, String FileType, String reportName)
        {

            SsrsReportService.ReportExecutionService rs = new SsrsReportService.ReportExecutionService();
            //rs.Url = "http://cpp.birdi-inc.io/reportserver/ReportExecution2005.asmx";
            //rs.Url = "http://birdi-dev02/reportserver/ReportExecution2005.asmx";

            String url = ConfigurationManager.AppSettings["SSRSReportUrl"];
            rs.Url = url;

            logger.Info("Connect to SSRS Server");
            try
            {

                Boolean isTestServer = ConfigurationManager.AppSettings["IsTestServer"] == "true";

                if (!isTestServer) //Enable Network Credential if test it locally
                {
                    String username = ConfigurationManager.AppSettings["SSRSReportNetworkCredentialUsername"];
                    String password = ConfigurationManager.AppSettings["SSRSReportNetworkCredentialPassword"];
                    rs.Credentials = new NetworkCredential(username, password);
                    //rs.Credentials = new NetworkCredential("Administrator", "Birdi@123");
                }
                else  //Enable this for deployment
                {
                    rs.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                }

                logger.Info(rs.Credentials.ToString());
                //new NetworkCredential("reporting", "Birdi@123");
            }
            catch (Exception ex)
            {
                logger.Error(ex.StackTrace);
            }



            // Render arguments
            byte[] result = null;
            string reportPath = ConfigurationManager.AppSettings["SSRSPathAdmin"] + reportName;
            string format = "PDF";
            string historyID = null;
            string devInfo = @"<DeviceInfo><Toolbar>False</Toolbar></DeviceInfo>";

            // Prepare report parameter.
            // SsrsReportService.ParameterValue[] parameters = new SsrsReportService.ParameterValue[0];

            SsrsReportService.DataSourceCredentials[] credentials = null;
            string showHideToggle = null;
            string encoding;
            string mimeType;
            string extension;
            SsrsReportService.Warning[] warnings = null;
            SsrsReportService.ParameterValue[] reportHistoryParameters = new SsrsReportService.ParameterValue[3];

            SsrsReportService.ParameterValue param1 = new SsrsReportService.ParameterValue();
            param1.Name = "PositionID";
            param1.Value = PositionID;

            SsrsReportService.ParameterValue param2 = new SsrsReportService.ParameterValue();
            param2.Name = "FromDate";
            param2.Value = FromDate;

            SsrsReportService.ParameterValue param3 = new SsrsReportService.ParameterValue();
            param3.Name = "ToDate";
            param3.Value = ToDate;

            reportHistoryParameters[0] = param1;
            reportHistoryParameters[1] = param2;
            reportHistoryParameters[2] = param3;

            string[] streamIDs = null;

            SsrsReportService.ExecutionInfo execInfo = new SsrsReportService.ExecutionInfo();
            SsrsReportService.ExecutionHeader execHeader = new SsrsReportService.ExecutionHeader();

            rs.ExecutionHeaderValue = execHeader;
            logger.Info("loading report...");
            try
            {
                execInfo = rs.LoadReport(reportPath, historyID);

            }
            catch (WebException ex)
            {
                logger.Error(ex.ToString());
                logger.Error(ex.StackTrace);
            }
            //  rs.SetExecutionParameters(parameters, "en-us");
            String SessionId = rs.ExecutionHeaderValue.ExecutionID;
            logger.Info("Session ID " + SessionId);
            Console.WriteLine("SessionID: {0}", rs.ExecutionHeaderValue.ExecutionID);
            //
            logger.Info("Rendering PDF report");
            try
            {
                //Set parameters
                rs.SetExecutionParameters(reportHistoryParameters, "en-us");
                result = rs.Render(FileType, devInfo, out extension, out encoding, out mimeType, out warnings, out streamIDs);

                execInfo = rs.GetExecutionInfo();

                Console.WriteLine("Execution date and time: {0}", execInfo.ExecutionDateTime);


            }
            catch (SoapException e)
            {
                logger.Error(e.StackTrace);
                Console.WriteLine(e.Detail.OuterXml);
            }

            return result;
        }
    }
}