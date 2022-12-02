using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex
{

    enum OptimalType
    {
        Min,
        Max
    }
    internal class Problem
    {
        public int AdditionalVariablesCount { get; set; }
        public bool IsConvertedToMin { get; set; }
        public int MVariablesCount { get; set; }
        public Problem()
        {
            IsConvertedToMin = false;
            AdditionalVariablesCount = 0;
            MVariablesCount = 0;
            functionVariables = new List<double>();
            Constraints =  new List<Constraint>();
        }

        public List<double> functionVariables { get; set; }
        public OptimalType OptimalType { get; set; }
        public List<Constraint> Constraints { get; set; }
    }
}
