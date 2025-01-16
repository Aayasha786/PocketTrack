using MudBlazor;
using ProjectTrack.Models;

namespace ProjectTrack.Components.Pages
{
    public partial class Dashboard
    {
        private IEnumerable<TransactionModel> TopTransactions = Enumerable.Empty<TransactionModel>();
        private IEnumerable<TransactionModel> PendingDebtTransactions = Enumerable.Empty<TransactionModel>();

        private DateTime? StartDate;
        private DateTime? EndDate;

        public List<ChartSeries> DynamicChartSeries { get; set; } = new();

        public string[] DynamicXAxisLabels { get; set; } = Array.Empty<string>();
        public string[] DynamicYAxisLabels { get; set; } = Array.Empty<string>();

        private List<TransactionModel> PendingDebtDetails = new();


        protected override void OnInitialized()
        {
            TransactionService.OnTransactionsChanged += LoadDashboardData;
            LoadDashboardData(); // Initial load
        }

        public void Dispose()
        {
            TransactionService.OnTransactionsChanged -= LoadDashboardData; // Unsubscribe when the component is disposed
        }

        private void UpdateDynamicChart()
        {
            // Prepare data for the bar chart
            DynamicYAxisLabels = TopTransactions
                .Select(t => t.Description.Length > 15
                    ? t.Description.Substring(0, 15) + "..." // Truncate long descriptions
                    : t.Description)
                .ToArray();

            DynamicChartSeries = new List<ChartSeries>
    {
        new ChartSeries
        {
            Name = "Amount",
            Data = TopTransactions.Select(t => (double)t.Amount).ToArray()
        }
    };
        }


        private void LoadDashboardData()
        {
            var filteredTransactions = TransactionService.Transactions;

            // Apply date range filter
            if (StartDate.HasValue)
                filteredTransactions = filteredTransactions.Where(t => t.Date >= StartDate.Value).ToList();

            if (EndDate.HasValue)
                filteredTransactions = filteredTransactions.Where(t => t.Date <= EndDate.Value).ToList();

            // Top Transactions
            TopTransactions = filteredTransactions.OrderByDescending(t => t.Amount).Take(5);

            // Fetch Pending Debt Details
            PendingDebtDetails = TransactionService.GetPendingDebtDetails();

            // Update Chart Data
            UpdateDynamicChart();
        }





        private void ApplyDateFilter()
        {
            LoadDashboardData();
        }
    }
}

