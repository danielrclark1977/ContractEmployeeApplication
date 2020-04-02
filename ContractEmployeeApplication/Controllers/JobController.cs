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
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;
        private ContractEmployeeDBContext _dbContext;
        public JobController(ILogger<JobController> logger, ContractEmployeeDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        [HttpGet]
        public IEnumerable<Jobs> Get()
        {
            List<Jobs> jobs = _dbContext.Jobs.Where(x => x.IsActive == true).ToList();
            _logger.LogInformation("Jobs were requested");
            return jobs;
        }

        [HttpPut]
        public void Put([FromBody] Jobs job)
        {
            //ActionResult
            if (_dbContext.Jobs.Where(x => x.Id == job.Id).Any())
            {
                updateCurrentEmployee(job);
                _logger.LogInformation("Job [0] was updated", job.Id);
            }
            else
            {
                job.Id = 1 + _dbContext.Jobs.OrderBy(x => x.Id).Last().Id;
                _dbContext.Jobs.Add(job);
                _logger.LogInformation("Job [0] was added", job.Id);
            }
            if (_dbContext.ChangeTracker.HasChanges())
            {
                _dbContext.SaveChangesAsync();
            }
            //return System.Web.Helpers.Json(data, JsonRequestBehavior.AllowGet);
        }
        private bool updateCurrentEmployee(Jobs job)
        {
            var currentEmployee = _dbContext.Jobs.Where(x => x.Id == job.Id).FirstOrDefault();
            var currentEmployeeProperties = currentEmployee.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo propertyInfo in currentEmployeeProperties)
            {
                if (!(job.GetType().GetProperty(propertyInfo.Name).GetValue(job, null).ToString() == currentEmployee.GetType().GetProperty(propertyInfo.Name).GetValue(currentEmployee, null).ToString()))
                {
                    currentEmployee.GetType().GetProperty(propertyInfo.Name).SetValue(currentEmployee, job.GetType().GetProperty(propertyInfo.Name).GetValue(job, null));
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
