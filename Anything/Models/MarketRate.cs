using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anything.Models
{
    public class MarketRate
    {
        public bool success { get; set; }
        public int timestamp { get; set; }
        public DateTime date {get;set;}
        public Dictionary<string, float> rates { get; set; }
    }
    public class rate
    {
        public string curr { get; set; }
        public float amt { get; set; }

    }
}