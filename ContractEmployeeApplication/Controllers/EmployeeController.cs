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
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private ContractEmployeeDBContext _dbContext;
        public EmployeeController(ILogger<EmployeeController> logger, ContractEmployeeDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Employees> Get()
        
        {
            
            List<Employees> employees = _dbContext.Employees.Where(x => x.IsActive == true).ToList();
            foreach (var employee in employees)
            {
                if (employee.EmployeeSkillLink.Count > 0)
                {
                    var employeeSkillLink = employee.EmployeeSkillLink.First();
                    var currentEmployeeSkills = _dbContext.Skills.Where(x => x.Id == employeeSkillLink.Skill).ToList();
                    employee.Skills = currentEmployeeSkills.Select(x => x.Id).ToArray<int>();
                }
                if (employee.Skills == null)
                {
                    employee.Skills = new int[1] {0};
                }
            }
            
            _logger.LogInformation("Employees were requested");
            return employees;
        }

        [HttpPut]
        public void Put([FromBody] Employees employee)
        {
            //ActionResult
            var currentEmployee = _dbContext.Employees.Where(x => (x.IsActive == true || x.IsActive == false) && x.Id == employee.Id).FirstOrDefault();
            if (currentEmployee != null)
            {
                updateCurrentEmployee(currentEmployee);
                _logger.LogInformation("employee [0] was updated", employee.Id);
            } 
            else
            {
                _dbContext.Employees.Add(currentEmployee);
                _logger.LogInformation("employee [0] was added", employee.Id);
            }
            if (_dbContext.ChangeTracker.HasChanges())
            {
                _dbContext.SaveChangesAsync();
            }
            //return System.Web.Helpers.Json(data, JsonRequestBehavior.AllowGet);
        }

        private bool updateCurrentEmployee(EmployeeApplicationModel.Models.Employees employee)
        {
            var currentEmployee = _dbContext.Employees.Where(x => x.Id == employee.Id).FirstOrDefault();
            var currentEmployeeProperties = currentEmployee.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo propertyInfo in currentEmployeeProperties)
            {
                if (!(employee.GetType().GetProperty(propertyInfo.Name).GetValue(employee, null).ToString() == currentEmployee.GetType().GetProperty(propertyInfo.Name).GetValue(currentEmployee, null).ToString()))
                {
                    currentEmployee.GetType().GetProperty(propertyInfo.Name).SetValue(currentEmployee, employee.GetType().GetProperty(propertyInfo.Name).GetValue(employee, null));
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
