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
    [Table("prelimnary_notice")]
    public class PrelimnaryNotice
    {
        [NotMapped]
        public int Operation;
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }
        public int ProgramID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

		public static void SaveNotice(PrelimnaryNotice prelimnaryNotice)
		{

			Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
			Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
			List<PrelimnaryNotice> PrelimnaryNotice = new List<PrelimnaryNotice>();

			try
			{
				using (var ctx = new CPPDbContext())
				{
					prelimnaryNotice.CreatedDate = DateTime.Now;
					ctx.PrelimnaryNotices.Add(prelimnaryNotice);
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

        public static List<PrelimnaryNotice> GetPrelimnaryNoticeList(int programId)
        {
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "Entry Point", Logger.logLevel.Info);
            Logger.LogDebug(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, "", Logger.logLevel.Debug);
            List<PrelimnaryNotice> prelimnaryNoticesList = new List<PrelimnaryNotice>();

            Program programid = new Program();
            try
            {
                using (var ctx = new CPPDbContext())
                {

                    prelimnaryNoticesList = ctx.PrelimnaryNotices.Where(u => u.ProgramID == programId).ToList();


                    return prelimnaryNoticesList;

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
            return prelimnaryNoticesList;
        }

    }
}