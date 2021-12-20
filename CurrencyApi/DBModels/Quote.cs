using System;
using System.Collections.Generic;

namespace CurrencyApi.DBModels
{
    public partial class Quote
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Valuteid { get; set; }
        public int Numcode { get; set; }
        public string Charcode { get; set; }
        public int Nominal { get; set; }
        public decimal Value { get; set; }
    }
}
