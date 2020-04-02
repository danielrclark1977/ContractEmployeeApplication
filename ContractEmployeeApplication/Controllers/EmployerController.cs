using System.Collections.Generic;
using System.Linq;
using EmployeeApplicationModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeApplication.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class EmployerController : Controller
    {
        private readonly ILogger<EmployerController> _logger;
        private ContractEmployeeDBContext _dbContext;
        public EmployerController(ILogger<EmployerController> logger, ContractEmployeeDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Employers> Get()
        {
            List<Employers> employers = _dbContext.Employers.Where(x => x.IsActive == true).ToList();
            _logger.LogInformation("Employers were requested");
            return employers;
        }
        [HttpPut]
        public void Put([FromBody] Employers employer)
        {
            var currentEmployer = _dbContext.Employers.Where(x => x.Id == employer.Id).FirstOrDefault();
            //ActionResult
            if (currentEmployer != null)
            {
                updateCurrentEmployee(employer);
                _logger.LogInformation("employer [0] was updated", employer.Id);
            }
            else
            {
                _dbContext.Employers.Add(employer);
                _logger.LogInformation("employer [0] was added", employer.Id);
            }
            if (_dbContext.ChangeTracker.HasChanges())
            {
                _dbContext.SaveChanges();
            }
            //return System.Web.Helpers.Json(data, JsonRequestBehavior.AllowGet);
        }

        private bool updateCurrentEmployee(Employers employer)
        {
            var currentEmployer = _dbContext.Employers.Where(x => x.Id == employer.Id).FirstOrDefault();
            var currentEmployerProperties = currentEmployer.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo propertyInfo in currentEmployerProperties)
            {
                if (!(employer.GetType().GetProperty(propertyInfo.Name).GetValue(employer, null).ToString() == currentEmployer.GetType().GetProperty(propertyInfo.Name).GetValue(currentEmployer, null).ToString()))
                {
                    currentEmployer.GetType().GetProperty(propertyInfo.Name).SetValue(currentEmployer, employer.GetType().GetProperty(propertyInfo.Name).GetValue(employer, null));
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
