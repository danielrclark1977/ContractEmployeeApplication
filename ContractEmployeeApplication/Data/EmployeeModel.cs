using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeApplication.Data
{
    public class EmployeeModel : Employee
    {
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int SkillsId { get; set; }
    }
}
