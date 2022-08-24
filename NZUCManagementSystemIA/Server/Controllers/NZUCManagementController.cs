using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZUC_Domain.Repositories;
using NZUCManagementSystemIA.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NZUCManagementSystemIA.Server.Controllers
{
   [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NZUCManagementController : ControllerBase
    {
        private readonly NZUCManagementRepository NZUCRepo;
        public NZUCManagementController(NZUCManagementRepository repository)
        {
            NZUCRepo = repository;
        }


        #region Get-Without-Id
        [HttpGet("GetAuthorizers")]
        public async Task<IActionResult> GetAuthorizers()
        {
            var result = await NZUCRepo.GetAllAuthorizers();
            return Ok(result);
        }
        [HttpGet("GetReviewerTransactions")]
        public async Task<IActionResult> GetReviewerTransactions()
        {
            var result = await NZUCRepo.GetAllTransactions();
            return Ok(result);
        }
        
        [HttpGet("GetPaymentMethods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var result = await NZUCRepo.GetAllAsync<PaymentMethodTable>();
            return Ok(result);
        }
        [HttpGet("GetPaymentTypes")]
        public async Task<IActionResult> GetPaymentTypes()
        {
            var result = await NZUCRepo.GetAllAsync<PaymentType>();
            return Ok(result);
        }
        [HttpGet("GetConferences")]
        public async Task<IActionResult> GetConferences()
        {
            var result = await NZUCRepo.GetAllAsync<ConferencesAndField>();
            return Ok(result);
        }
        [HttpGet("GetDepartments")]
        public async Task<IActionResult> GetDepartments()
        {
            var result = await NZUCRepo.GetAllAsync<Departments_Table>();
            return Ok(result);
        }

        [HttpGet("GetEmployees")]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await NZUCRepo.GetAllEmployees();
            return Ok(result);
        }
        [HttpGet("GetGenders")]
        public async Task<IActionResult> GetGender()
        {
            var result = await NZUCRepo.GetAllAsync<GenderTable>();
            return Ok(result);
        }

        [HttpGet("GetBudgetTypes")]
        public async Task<IActionResult> GetBudgetTypes()
        {
            var result = await NZUCRepo.GetAllAsync<BudgetTypeTable>();
            return Ok(result);
        }
        [HttpGet("GetReviewers")]
        public async Task<IActionResult> GetReviewers()
        {
            var result = await NZUCRepo.GetReviewers();
            return Ok(result);
        }
        [HttpGet("GetSalaries")]
        public async Task<IActionResult> GetSalaries()
        {
            var result = await NZUCRepo.GetAllAsync<SalaryTable>();
            return Ok(result);
        }
        [HttpGet("GetBudgets")]
        public async Task<IActionResult> GetBudgets()
        {
            var result = await NZUCRepo.GetYearlyBudgets();
            return Ok(result);
        }
        [HttpGet("GetOperatingTypes")]
        public async Task<IActionResult> GetOperationgTypes()
        {
            var result = await NZUCRepo.GetAllAsync<OperatingType>();
            return Ok(result);
        }
        [HttpGet("GetOperatingIncomeExpense")]
        public async Task<IActionResult> GetOperatingIncomeExpense()
        {
            var result = await NZUCRepo.GetoperatingIncomeEX();
            return Ok(result);
        }
        [HttpGet("GetStatuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var result = await NZUCRepo.GetAllAsync<TransactionStatus>();
            return Ok(result);
        }

        #endregion
        #region GetByForeignId
        [HttpGet]
        [Route("GetIncomeExpenseByBudgetTypeId/{id}")]
        public IEnumerable<IncomeExpenseBudgetType> GetIncomeExpenseById(int id)
        {

            var result = NZUCRepo.GetIncomeExpenseBudgets(id);
            return result;
        }
        [HttpGet]
        [Route("GetIncomeExpenseByOperatingTypeId/{id}")]
        public IEnumerable<IncomeExpensesOperatingType> GetIncomeExpenseOperatingById(int id)
        {

            var result = NZUCRepo.GetIncomeExpenseOperatingTypes(id);
            return result;
        }

        #endregion

        #region Post to Database
        
        [HttpPost("SaveEmployee")]
        public async Task<IActionResult> PostAreaAsync(EmployeeTable employee)
        {
            var result = await NZUCRepo.SaveAsync(employee);
            return Ok(result);
        }
        [HttpPost("SavePermanentTransaction")]
        public async Task<IActionResult> PostPermanentTransactionAsync(TransactionTable employee)
        {
            var result = await NZUCRepo.SaveAsync(employee);
            return Ok(result);
        }
        [HttpPost("SaveAuthorizer")]
        public async Task<IActionResult> PostAuthorizerAsync(AuthorizerTable authorizer)
        {
            var result = await NZUCRepo.SaveAsync(authorizer);
            return Ok(result);
        }
        [HttpPost("SaveReviewerTransaction")]
        public async Task<IActionResult> PostReviewerTransactionAsync(ReviewTransactionTable reviewTransaction)
        {
            var result = await NZUCRepo.SaveAsync(reviewTransaction);
            return Ok(result);
        }
        [HttpPost("SaveOperatingIncomeExpense")]
        public async Task<IActionResult> PostOperatingIncomeExpenseAsync(OperatingIncomeExpense operatingIncomeExpense)
        {
            var result = await NZUCRepo.SaveAsync(operatingIncomeExpense);
            return Ok(result);
        }
        [HttpPost("SaveReviewer")]
        public async Task<IActionResult> PostReviewerAsync(ReviewersTable employee)
        {
            var result = await NZUCRepo.SaveAsync(employee);
            return Ok(result);
        }
        [HttpPost("SaveYearlyBudget")]
        public async Task<IActionResult> PostYearlyBudgetAsync(YearlyBudgetTable yearlyBudget)
        {
            var result = await NZUCRepo.SaveAsync(yearlyBudget);
            return Ok(result);
        }
        [HttpPost("SaveConference")]
        public async Task<IActionResult> PostConferenceAsync(ConferencesAndField conferencesAndField)
        {
            var result = await NZUCRepo.SaveAsync(conferencesAndField);
            return Ok(result);
        }
        #endregion

        #region Update 
        
        [HttpPut("UpdateConference")]
        public async Task<IActionResult> PutAreaAsync(ConferencesAndField ConferenceUpdate)
        {
            var res = await NZUCRepo.UpdateAsync(ConferenceUpdate);

            return Ok(res);
        }
        [HttpPut("UpdateTransaction")]
        public async Task<IActionResult> PutTransactionAsync(ReviewTransactionTable TransactionUpdate)
        {
            var res = await NZUCRepo.UpdateAsync(TransactionUpdate);

            return Ok(res);
        }
        #endregion

        #region Delete Functions
        [HttpDelete("DeleteSalaryScale/{id}")]
        public async Task<IActionResult> DeleteSalaryScaleAsync(int id)
        {

            await NZUCRepo.DeleteAllAsync(id);
            return NoContent();
        }
        [HttpDelete("DeleteReviewTransaction/{id}")]
        public async Task<IActionResult> DeleteReviewTransactionAsync(int id)
        {

            await NZUCRepo.DeleteReviewTransactionAsync(id);
            return NoContent();
        }

        [HttpDelete("DeleteDepartment/{id}")]
        public async Task<IActionResult> DeleteDepartmentAsync(int id)
        {

            await NZUCRepo.DeleteDepartmentAsync(id);
            return NoContent();
        }
        [HttpDelete("DeleteConference/{id}")]
        public async Task<IActionResult> DeleteConferenceAsync(int id)
        {

            await NZUCRepo.DeleteConferenceAsync(id);
            return NoContent();
        }
        [HttpDelete("DeleteAuthorizer/{id}")]
        public async Task<IActionResult> DeleteAuthorizerAsync(int id)
        {

            await NZUCRepo.DeleteAuthorizerAsync(id);
            return NoContent();
        }
        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(int id)
        {

            await NZUCRepo.DeleteEmployeeAsync(id);
            return NoContent();
        }
        [HttpDelete("DeleteBudget/{id}")]
        public async Task<IActionResult> DeleteBudgetAsync(int id)
        {

            await NZUCRepo.DeleteBudgetAsync(id);
            return NoContent();
        }
        
        [HttpDelete("DeleteOperatingIncomeExp/{id}")]
        public async Task<IActionResult> DeleteOperatingIncomeExpAsync(int id)
        {

            await NZUCRepo.DeleteOperatingIncomeExpAsync(id);
            return NoContent();
        }
        #endregion
    }
}
