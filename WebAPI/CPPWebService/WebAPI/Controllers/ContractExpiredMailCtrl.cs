﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ContractExpiredMailCtrl : Controller
    {
        DateTime ProgramCurrentDate = DateTime.Now;
        public void Get()
        {
            List<Program> pgList = new List<Program>();
            List<ProgramElement> projList = new List<ProgramElement>();
            List<Program> ExpiredDates = new List<Program>();
            List<String> to = new List<String>();


            List<User> userTable = new List<User>();
            List<ProgramElement> projManager = new List<ProgramElement>();


            using (var ctx = new CPPDbContext())
            {

                string sub = "";
                pgList = ctx.Program.ToList();
                projList = ctx.ProgramElement.ToList();
                List<Program> currentDateList = ctx.Program.Where(p => (p.CurrentEndDate != null) && (p.CurrentEndDate > ProgramCurrentDate)).ToList();

                for (int i = 0; i < currentDateList.Count; i++)
                {
                    var v = currentDateList[i].CurrentEndDate - ProgramCurrentDate;
                    var days = v.Value.Days;
                    if (days == 30 && currentDateList[i].ProjectManagerEmail != "")
                    {
                        to.Add(currentDateList[i].ProjectManagerEmail);
                        WebAPI.Services.MailServices.SendReminderEmailContractExp(currentDateList[i].ProjectManagerEmail, currentDateList[i].ProjectManager, currentDateList[i].ProgramName, currentDateList[i].ContractNumber, currentDateList[i].CurrentEndDate.ToString(), sub);
                        for (int j = 0; j < projList.Count; j++)
                        {
                            var projectID = projList[j].ProgramElementID;
                            if (currentDateList[i].ProgramID == projList[j].ProgramID && projList[j].ProgramElementManager != null)
                            {

                                ProjectApproversDetails ApproverProjectManager = ctx.ProjectApproversDetails.Where(a => a.ApproverMatrixId == 4 && a.ProjectId == projectID).FirstOrDefault();
                                var projectManagerID = ApproverProjectManager.EmpId;
                                User user = ctx.User.Where(u => u.EmployeeID == projectManagerID).FirstOrDefault();
                                WebAPI.Services.MailServices.SendReminderEmailContractExp(user.Email, user.FirstName + " " + user.LastName, currentDateList[i].ProgramName, currentDateList[i].ContractNumber, currentDateList[i].CurrentEndDate.ToString(), sub);
                            }

                        }
                        ExpiredDates.Add(currentDateList[i]);
                    }

                }
                System.Diagnostics.Debug.WriteLine(ExpiredDates);

            }



        }
    }
}