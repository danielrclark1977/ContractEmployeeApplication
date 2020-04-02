using System.Collections.Generic;
using System.Linq;
using EmployeeApplication.Data;
using EmployeeApplicationModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly ILogger<SkillController> _logger;
        private ContractEmployeeDBContext _dbContext;
        public SkillController(ILogger<SkillController> logger, ContractEmployeeDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Skills> Get(int employeeId)
        
        {
            
            List<EmployeeSkillLink> employeeSkillLinks = _dbContext.EmployeeSkillLink.Where(x => x.Employee == employeeId).ToList();
            List<Skills> employeeSkills = new List<Skills>();
            foreach (EmployeeSkillLink employeeSkillLink in employeeSkillLinks)
            {
                var skill = _dbContext.Skills.Where(x => x.Id == employeeSkillLink.Skill).FirstOrDefault();
                if (skill != null)
                {
                    employeeSkills.Add(skill);
                }

            }
            _logger.LogInformation("Skills were requested for "+ employeeId);
            return employeeSkills;
        }

        //[HttpPut]
        //public void Put([FromBody] Skills skill)
        //{
        //    //ActionResult
        //    var currentEmployee = _dbContext.Skills.Where(x => (x.IsActive == true || x.IsActive == false) && x.Id == skill.Id).FirstOrDefault();
        //    if (currentEmployee != null)
        //    {
        //        updateCurrentEmployee(currentEmployee);
        //        _logger.LogInformation("skill [0] was updated", skill.Id);
        //    } 
        //    else
        //    {
        //        _dbContext.Skills.Add(currentEmployee);
        //        _logger.LogInformation("skill [0] was added", skill.Id);
        //    }
        //    if (_dbContext.ChangeTracker.HasChanges())
        //    {
        //        _dbContext.SaveChangesAsync();
        //    }
        //    //return System.Web.Helpers.Json(data, JsonRequestBehavior.AllowGet);
        //}

        private bool updateCurrentEmployee(EmployeeApplicationModel.Models.Skills skill)
        {
            var currentEmployee = _dbContext.Skills.Where(x => x.Id == skill.Id).FirstOrDefault();
            var currentEmployeeProperties = currentEmployee.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo propertyInfo in currentEmployeeProperties)
            {
                if (!(skill.GetType().GetProperty(propertyInfo.Name).GetValue(skill, null).ToString() == currentEmployee.GetType().GetProperty(propertyInfo.Name).GetValue(currentEmployee, null).ToString()))
                {
                    currentEmployee.GetType().GetProperty(propertyInfo.Name).SetValue(currentEmployee, skill.GetType().GetProperty(propertyInfo.Name).GetValue(skill, null));
                }
                // do stuff here
            }
            if (_dbContext.ChangeTracker.HasChanges())
            {
                _dbContext.SaveChangesAsync();
            }
            return true;
        }
    }
}
