
using System;

namespace ProjectTrack.Models
{
    public class TransactionModel
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }    
        public List<string> Tags { get; set; } = new List<string>(); 
        public string Note { get; set; } // Optional note
    }

}
