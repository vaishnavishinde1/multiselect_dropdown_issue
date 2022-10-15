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
    [Table("admin_approval")]
    public class AdminApproval:Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int DepartmentID { get; set; }
       // public int User_Role_ID { get; set; }
        public int DeptManagerID { get; set; }
        public int OpeManagerID { get; set; }
       
    }
}