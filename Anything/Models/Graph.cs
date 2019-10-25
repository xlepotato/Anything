using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anything.Models
{
    public class Graph
    {
        public string Title { get; set; }
        public List<string> ShortDate { get; set; }
        public List<double> Amount { get; set; }
        public List<double> RegressionY { get; set; }
    }
}