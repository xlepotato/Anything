using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anything.Models
{
    public class HistoricalRates
    {
        public string Title { get; set; }
        public List<string> ShortDate { get; set; }
        public List<float> Amount { get; set; }
    }
}