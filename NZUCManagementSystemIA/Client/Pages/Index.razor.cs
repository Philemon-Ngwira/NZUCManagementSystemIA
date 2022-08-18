using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages
{
    public class IndexBase : ComponentBase
    {
        protected IList<EmployeeTable> _employeeslist = new List<EmployeeTable>();
        protected IList<Departments_Table> _departments = new List<Departments_Table>();
        protected IList<YearlyBudgetTable> yearlyBudgets = new List<YearlyBudgetTable>();
        protected IList<OperatingIncomeExpense> operatingIncomeExpenses = new List<OperatingIncomeExpense>();
        protected IList<ConferencesAndField> _conferences = new List<ConferencesAndField>();
        protected IList<ReviewTransactionTable> _transactions = new List<ReviewTransactionTable>();
        protected int? totalIncomeYearBudget;
        protected int? totalmonthoperatingIncome;
        string incomeBudgetName = "";
        string operatingIncomeName = "";
        protected string IncomeBudgetDisplay;
        protected string operatingIncomeDisplay;
        protected string[] XAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        [Inject] IGenericRepositoryService _genericRepository { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationState { get; set; }

        protected int totalEmployees;
        protected int Index = -1;
        protected double[] IncomePerMonth = new double[12];
        protected ChartSeries ExpensePerMonthSeries = new ChartSeries();
        protected ChartSeries IncomePerMonthSeries = new ChartSeries();
        private double JanIncome, febIncome, MarIncome, AprIncome, MayIncome, JunIncome, JulIncome, AugIncome, sepIncome, OctIncome, NovIncome, DecIncome;
        protected List<ChartSeries> Series = new List<ChartSeries>();
        protected ChartOptions ChartOptions { get; set; } = new ChartOptions();
        protected string[] departments;
        protected List<string> deptNames = new List<string>();
        protected int? DepartmentIncome;
        public string[] labels = { "Income", "Expense", };
        protected string IncomeDisplay;
        protected string ExpenseDisplay;
        protected double[] incomeExpensedata = new double[2];
        protected int chartindex = -1;
        protected int datasize;
        protected string[] ConferenceNames;
        protected double[] conferenceIncomeData;
        protected double incomeConfData1, incomeConfData2, incomeConfData3, incomeConfData4, incomeConfData5, incomeConfData6;




        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthenticationState.GetAuthenticationStateAsync()).User.Identity;
            if (user != null && user.IsAuthenticated)
            {
                await GetTransactions();
                await GetBudgets();
                PieChart();
                await GetEmployees();
                totalEmployees = _employeeslist.Count();
                getMonthlyIncomeData();
                getPieChartData();
                DonughtChart();
                Series.Add(IncomePerMonthSeries);
                Series.Add(ExpensePerMonthSeries);
            }

        }
        protected async Task GetTransactions()
        {
            var result = await _genericRepository.GetAllAsync<ReviewTransactionTable>("api/NZUCManagement/GetReviewerTransactions");
            _transactions = result.ToList();
        }
        private async Task GetEmployees()
        {
            var result = await _genericRepository.GetAllAsync<EmployeeTable>("api/NZUCManagement/GetEmployees");
            _employeeslist = result.ToList();
        }
        protected async Task GetBudgets()
        {
            var result = await _genericRepository.GetAllAsync<YearlyBudgetTable>("api/NZUCManagement/GetBudgets");
            yearlyBudgets = result.ToList();
            incomeBudgetName = "Income Budget";
            totalIncomeYearBudget = yearlyBudgets.Where(x => x.IncomeExpesense.TypeofBudget.BudgetType == incomeBudgetName).Sum(x => x.ActualBudget);
            IncomeBudgetDisplay = $"{totalIncomeYearBudget:n0}";
            var operatings = await _genericRepository.GetAllAsync<OperatingIncomeExpense>("api/NZUCManagement/GetOperatingIncomeExpense");
            operatingIncomeExpenses = operatings.ToList();
            operatingIncomeName = "Income";
            totalmonthoperatingIncome = operatingIncomeExpenses.Where(x => x.IncomeExpenseOperatingTypeNavigation.OperatingType1 == operatingIncomeName).Sum(x => x.Amount);
            DepartmentIncome = totalmonthoperatingIncome;
            operatingIncomeDisplay = $"{totalmonthoperatingIncome:n0}";
        }

        protected void fillghraph()
        {
            ExpensePerMonthSeries.Name = "Expense";
            ExpensePerMonthSeries.Data = new double[] { 1000, 402000, 200000, 4000, 5000, 6000, 7000, 5000, 3000, 25000, 4500, 9000 };
            //Index Zero Is January and so on!
            #region IncomeChartSeries
            IncomePerMonthSeries.Name = "Income";
            IncomePerMonthSeries.Data = new double[12];
            IncomePerMonthSeries.Data[0] = IncomePerMonth[0];
            IncomePerMonthSeries.Data[1] = IncomePerMonth[1];
            IncomePerMonthSeries.Data[2] = IncomePerMonth[2];
            IncomePerMonthSeries.Data[3] = IncomePerMonth[3];
            IncomePerMonthSeries.Data[4] = IncomePerMonth[4];
            IncomePerMonthSeries.Data[5] = IncomePerMonth[5];
            IncomePerMonthSeries.Data[6] = IncomePerMonth[6];
            IncomePerMonthSeries.Data[7] = IncomePerMonth[7];
            IncomePerMonthSeries.Data[8] = IncomePerMonth[8];
            IncomePerMonthSeries.Data[9] = IncomePerMonth[9];
            IncomePerMonthSeries.Data[10] = IncomePerMonth[10];
            IncomePerMonthSeries.Data[11] = IncomePerMonth[11];
            #endregion 



        }

        private void getMonthlyIncomeData()
        {
            #region Getting Int Income Amounts
            int? JanIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 1 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? FebIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 2 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? MarIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 3 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? AprIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 4 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? MayIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 5 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? JunIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 6 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? JulIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 7 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? AugIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 8 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? SepIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 9 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? OctIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 10 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? NovIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 11 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            int? DecIn = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 12 && x.IncomeExpenseOperatingType == 1).Sum(x => x.Amount);
            #endregion

            #region Convert All Amounts to Doubles
            JanIncome = Convert.ToDouble(JanIn);
            febIncome = Convert.ToDouble(FebIn);
            MarIncome = Convert.ToDouble(MarIn);
            AprIncome = Convert.ToDouble(AprIn);
            MayIncome = Convert.ToDouble(MayIn);
            JunIncome = Convert.ToDouble(JunIn);
            JulIncome = Convert.ToDouble(JulIn);
            AugIncome = Convert.ToDouble(AugIn);
            sepIncome = Convert.ToDouble(SepIn);
            OctIncome = Convert.ToDouble(OctIn);
            NovIncome = Convert.ToDouble(NovIn);
            DecIncome = Convert.ToDouble(DecIn);
            #endregion

            #region Assign Amounts to Array Indecies
            IncomePerMonth[0] = JanIncome;
            IncomePerMonth[1] = febIncome;
            IncomePerMonth[2] = MarIncome;
            IncomePerMonth[3] = AprIncome;
            IncomePerMonth[4] = MayIncome;
            IncomePerMonth[5] = JunIncome;
            IncomePerMonth[6] = JulIncome;
            IncomePerMonth[7] = AugIncome;
            IncomePerMonth[8] = sepIncome;
            IncomePerMonth[9] = OctIncome;
            IncomePerMonth[10] = NovIncome;
            IncomePerMonth[11] = DecIncome;
            #endregion

            fillghraph();

        }
        private async void getPieChartData()
        {
            var data = await _genericRepository.GetAllAsync<Departments_Table>("api/NZUCManagement/GetDepartments");
            _departments = data.ToList();
            foreach (var item in _departments)
            {


                deptNames.Add(item.DepartmentName);

            }
            deptNames.ToArray();
            departments = deptNames.ToArray();
        }
        private void DonughtChart()
        {
            //everywhere with 400000 goes expense
            var doubleinc = Convert.ToDouble(DepartmentIncome);
            incomeExpensedata[0] = doubleinc;
            incomeExpensedata[1] = 400000;
            IncomeDisplay = $"{doubleinc:n0}";
            ExpenseDisplay = $"{400000:n0}";
        }
        private async void PieChart()
        {
            List<double> Amounts = new List<double>();
            List<string> Names = new List<string>();
            var res = await _genericRepository.GetAllAsync<ConferencesAndField>("api/NZUCManagement/GetConferences");
            _conferences = res.ToList();
            foreach (var item in _conferences)
            {
                Names.Add(item.ConferenceName);
            }
            ConferenceNames = Names.ToArray();
            var one = operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[0]).Sum(x => x.Amount);
            var two = operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[1]).Sum(x => x.Amount);
            var three = operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[2]).Sum(x => x.Amount);
            var four = operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[3]).Sum(x => x.Amount);
            var five = operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[4]).Sum(x => x.Amount);
            var six = operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[5]).Sum(x => x.Amount);

            datasize = 6;
            conferenceIncomeData = new double[_conferences.Count()];
            conferenceIncomeData[0] = Convert.ToDouble(one);
            conferenceIncomeData[1] = Convert.ToDouble(two);
            conferenceIncomeData[2] = Convert.ToDouble(three);
            conferenceIncomeData[3] = Convert.ToDouble(four);
            conferenceIncomeData[4] = Convert.ToDouble(five);
            conferenceIncomeData[5] = Convert.ToDouble(six);
        }

    }

}
