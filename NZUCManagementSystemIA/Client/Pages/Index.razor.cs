using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages
{
    public class IndexBase : ComponentBase
    {
        #region Budget Variables
        protected YearlyBudgetTable YearlyBudget = new YearlyBudgetTable();
        protected IList<YearlyBudgetTable> yearlyBudgets = new List<YearlyBudgetTable>();
        protected IList<OperatingIncomeExpense> operatingIncomeExpenses = new List<OperatingIncomeExpense>();
        protected string incomeBudgetName = string.Empty;
        protected int? totalIncomeYearBudget = 0;
        protected string IncomeBudgetDisplay = string.Empty;
        protected string operatingIncomeName = string.Empty;
        protected string operatingExpenseName = string.Empty;
        protected string Info;
        protected int? totalmonthoperatingIncome = 0;
        protected int? totalmonthoperatingExpense = 0;
        protected string operatingIncomeDisplay = string.Empty;
        protected string operatingExpenseDisplay = string.Empty;
        #endregion

        #region Employee Variables
        protected IList<EmployeeTable> employees = new List<EmployeeTable>();
        protected int totalEmployees = 0;
        #endregion

        #region Charts
        #region LineChart
        protected List<ChartSeries> Series = new List<ChartSeries>();
        protected string[] XAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private double JanIncome, febIncome, MarIncome, AprIncome, MayIncome, JunIncome, JulIncome, AugIncome, sepIncome, OctIncome, NovIncome, DecIncome;
        private double JanExp, febExp, MarExp, AprExp, MayExp, JunExp, JulExp, AugExp, sepExp, OctExp, NovExp, DecExp;
        protected double[] IncomePerMonth = new double[12];
        protected double[] ExpensePerMonth = new double[12];
        protected ChartSeries ExpensePerMonthSeries = new ChartSeries();
        protected ChartSeries IncomePerMonthSeries = new ChartSeries();
        protected int Index = -1;
        #endregion
        #region Dognut Chart
        protected double[] incomeExpensedata = new double[2];
        protected string IncomeDisplay = string.Empty;
        protected string ExpenseDisplay = string.Empty;
        public string[] labels = { "Income", "Expense", };
        #endregion
        #region PieChart
        protected IList<Departments_Table> _departments = new List<Departments_Table>();
        protected IList<ConferencesAndField> _conferences = new List<ConferencesAndField>();
        protected string[] ConferenceNames;
        protected int datasize = 0;
        protected double[] conferenceIncomeData;
        #endregion
        #region BarChart
        protected string[] DepartmentNames;
        protected int BarChartIndex = -1;
        protected List<ChartSeries> DepartmentSpendSeries = new List<ChartSeries>();
        protected ChartSeries DepartmentSalarySpendSeries = new ChartSeries();
        protected ChartSeries DepartmentTravelExpenseSeries = new ChartSeries();
        protected bool notEnoughData;
        //{
        //    new ChartSeries() { Name = "United States", Data = new double[] { 40, 20, 25, 27, 46, 60, 48, 80, 15 } },
        //    new ChartSeries() { Name = "Germany", Data = new double[] { 19, 24, 35, 13, 28, 15, 13, 16, 31 } },
        //    new ChartSeries() { Name = "Sweden", Data = new double[] { 8, 6, 11, 13, 4, 16, 10, 16, 18 } },
        //};

        #endregion
        #endregion

        #region Injections
        [Inject] IGenericRepositoryService _genericRepository { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationState { get; set; }
        #endregion



        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthenticationState.GetAuthenticationStateAsync()).User.Identity;
            if (user != null && user.IsAuthenticated)
            {
                await GetBudgets();
                await GetEmployeeCount();
                await GetMonthByMonthIncomeAndExpense();
                DonughtChart();
                await PieChart();
                await GetBarChartData();
                Delays();
            }
        }

        #region Functions
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
            operatingExpenseName = "Expenses";
            totalmonthoperatingIncome = operatingIncomeExpenses.Where(x => x.IncomeExpenseOperatingTypeNavigation.OperatingType1 == operatingIncomeName).Sum(x => x.Amount);
            operatingIncomeDisplay = $"{totalmonthoperatingIncome:n0}";
            totalmonthoperatingExpense = operatingIncomeExpenses.Where(x => x.IncomeExpenseOperatingTypeNavigation.OperatingType1 == operatingExpenseName).Sum(x => x.Amount);
            operatingExpenseDisplay = $"{totalmonthoperatingExpense:n0}";
        }
        private async Task GetEmployeeCount()
        {
            var result = await _genericRepository.GetAllAsync<EmployeeTable>("api/NZUCManagement/GetEmployees");
            employees = result.ToList();
            totalEmployees = employees.Count();
        }

        #region LineChart
        protected async Task GetMonthByMonthIncomeAndExpense()
        {
            var result = await _genericRepository.GetAllAsync<OperatingIncomeExpense>("api/NZUCManagement/GetOperatingIncomeExpense");
            operatingIncomeExpenses = result.ToList();

            //Get Monthly Income
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

            #region Convert All Income Amounts to Doubles
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

            //Get Monthly Expense
            int? JanEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 1 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? FebEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 2 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? MarEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 3 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? AprEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 4 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? MayEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 5 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? JunEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 6 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? JulEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 7 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? AugEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 8 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? SepEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 9 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? OctEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 10 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? NovEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 11 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);
            int? DecEx = operatingIncomeExpenses.Where(x => x.DateMonth.Value.Month == 12 && x.IncomeExpenseOperatingType == 2).Sum(x => x.Amount);

            #region Convert All Expense Amounts to Doubles
            JanExp = Convert.ToDouble(JanEx);
            febExp = Convert.ToDouble(FebEx);
            MarExp = Convert.ToDouble(MarEx);
            AprExp = Convert.ToDouble(AprEx);
            MayExp = Convert.ToDouble(MayEx);
            JunExp = Convert.ToDouble(JunEx);
            JulExp = Convert.ToDouble(JulEx);
            AugExp = Convert.ToDouble(AugEx);
            sepExp = Convert.ToDouble(SepEx);
            OctExp = Convert.ToDouble(OctEx);
            NovExp = Convert.ToDouble(NovEx);
            DecExp = Convert.ToDouble(DecEx);
            #endregion
            ExpensePerMonth[0] = JanExp;
            ExpensePerMonth[1] = febExp;
            ExpensePerMonth[2] = MarExp;
            ExpensePerMonth[3] = AprExp;
            ExpensePerMonth[4] = MayExp;
            ExpensePerMonth[5] = JunExp;
            ExpensePerMonth[6] = JulExp;
            ExpensePerMonth[7] = AugExp;
            ExpensePerMonth[8] = sepExp;
            ExpensePerMonth[9] = OctExp;
            ExpensePerMonth[10] = NovExp;
            ExpensePerMonth[11] = DecExp;

            FillLineChart();
        }
        protected void FillLineChart()
        {
            ExpensePerMonthSeries.Name = "Expense";
            ExpensePerMonthSeries.Data = new double[12];
            ExpensePerMonthSeries.Data[0] = ExpensePerMonth[0];
            ExpensePerMonthSeries.Data[1] = ExpensePerMonth[1];
            ExpensePerMonthSeries.Data[2] = ExpensePerMonth[2];
            ExpensePerMonthSeries.Data[3] = ExpensePerMonth[3];
            ExpensePerMonthSeries.Data[4] = ExpensePerMonth[4];
            ExpensePerMonthSeries.Data[5] = ExpensePerMonth[5];
            ExpensePerMonthSeries.Data[6] = ExpensePerMonth[6];
            ExpensePerMonthSeries.Data[7] = ExpensePerMonth[7];
            ExpensePerMonthSeries.Data[8] = ExpensePerMonth[8];
            ExpensePerMonthSeries.Data[9] = ExpensePerMonth[9];
            ExpensePerMonthSeries.Data[10] = ExpensePerMonth[10];
            ExpensePerMonthSeries.Data[11] = ExpensePerMonth[11];

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

            Series.Add(IncomePerMonthSeries);
            Series.Add(ExpensePerMonthSeries);
        }
        #endregion

        #region Dognut Chart
        private void DonughtChart()
        {

            var doubleinc = Convert.ToDouble(totalmonthoperatingIncome);
            var doubleExp = Convert.ToDouble(totalmonthoperatingExpense);
            incomeExpensedata[0] = doubleinc;
            incomeExpensedata[1] = doubleExp;
            IncomeDisplay = $"{doubleinc:n0}";
            ExpenseDisplay = $"{doubleExp:n0}";
        }

        private async Task PieChart()
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
            datasize = 6;
            conferenceIncomeData = new double[_conferences.Count()];
            conferenceIncomeData[0] = Convert.ToDouble(operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[0]).Sum(x => x.Amount));
            conferenceIncomeData[1] = Convert.ToDouble(operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[1]).Sum(x => x.Amount));
            conferenceIncomeData[2] = Convert.ToDouble(operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[2]).Sum(x => x.Amount));
            conferenceIncomeData[3] = Convert.ToDouble(operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[3]).Sum(x => x.Amount));
            conferenceIncomeData[4] = Convert.ToDouble(operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[4]).Sum(x => x.Amount));
            conferenceIncomeData[5] = Convert.ToDouble(operatingIncomeExpenses.Where(x => x.Conference.ConferenceName == ConferenceNames[5]).Sum(x => x.Amount));
        }
        #endregion
        #region BarChart

        protected async Task GetBarChartData()
        {
            var result = await _genericRepository.GetAllAsync<Departments_Table>("api/NZUCManagement/GetDepartments");
            _departments = result.ToList();
            List<string> Names = new List<string>();
            foreach (var item in _departments)
            {
                Names.Add(item.DepartmentName);
            }
            DepartmentNames = Names.ToArray();
            DepartmentSalarySpendSeries.Name = "Department Costs";
            DepartmentSalarySpendSeries.Data = new double[_departments.Count()];
            if (operatingIncomeExpenses.Any(x => x.Department == null))
            {
                DepartmentSalarySpendSeries.Data[0] = 0;
                DepartmentSalarySpendSeries.Data[1] = 0;
                DepartmentSalarySpendSeries.Data[2] = 0;
                DepartmentSalarySpendSeries.Data[3] = 0;
                DepartmentSalarySpendSeries.Data[4] = 0;
                DepartmentSalarySpendSeries.Data[5] = 0;
                DepartmentSalarySpendSeries.Data[6] = 0;
                DepartmentSalarySpendSeries.Data[7] = 0;
                

            }
            else
            {
                DepartmentSalarySpendSeries.Data[0] = operatingIncomeExpenses.Where(x => x.Department.DepartmentName == DepartmentNames[0] && x.IncomeExpenseOperatingType == 2).Sum(x => Convert.ToDouble(x.Amount));
                DepartmentSalarySpendSeries.Data[1] = operatingIncomeExpenses.Where(x => x.Department.DepartmentName == DepartmentNames[1] && x.IncomeExpenseOperatingType == 2).Sum(x => Convert.ToDouble(x.Amount));
                DepartmentSalarySpendSeries.Data[2] = operatingIncomeExpenses.Where(x => x.Department.DepartmentName == DepartmentNames[2] && x.IncomeExpenseOperatingType == 2).Sum(x => Convert.ToDouble(x.Amount));
                DepartmentSalarySpendSeries.Data[3] = operatingIncomeExpenses.Where(x => x.Department.DepartmentName == DepartmentNames[3] && x.IncomeExpenseOperatingType == 2).Sum(x => Convert.ToDouble(x.Amount));
                DepartmentSalarySpendSeries.Data[4] = operatingIncomeExpenses.Where(x => x.Department.DepartmentName == DepartmentNames[4] && x.IncomeExpenseOperatingType == 2).Sum(x => Convert.ToDouble(x.Amount));
                DepartmentSalarySpendSeries.Data[5] = operatingIncomeExpenses.Where(x => x.Department.DepartmentName == DepartmentNames[5] && x.IncomeExpenseOperatingType == 2).Sum(x => Convert.ToDouble(x.Amount));
                DepartmentSalarySpendSeries.Data[6] = operatingIncomeExpenses.Where(x => x.Department.DepartmentName == DepartmentNames[6] && x.IncomeExpenseOperatingType == 2).Sum(x => Convert.ToDouble(x.Amount));
                DepartmentSalarySpendSeries.Data[7] = operatingIncomeExpenses.Where(x => x.Department.DepartmentName == DepartmentNames[7] && x.IncomeExpenseOperatingType == 2).Sum(x => Convert.ToDouble(x.Amount));
            }
            DepartmentSpendSeries.Add(DepartmentSalarySpendSeries);
        }

        #endregion
        protected void Delays()
        {
            Thread.Sleep(10000);
            Info = "No DataFound!!";

        }
        #endregion
    }
}
