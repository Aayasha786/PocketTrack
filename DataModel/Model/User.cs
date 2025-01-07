namespace DataModel.Model
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PreferredCurrency { get; set; } // Added PreferredCurrency property
    }



public class Transaction
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // Income, Expense, or Debt
    }
}
