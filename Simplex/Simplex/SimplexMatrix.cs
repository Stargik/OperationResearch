using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex
{
    internal class SimplexMatrix
    {
        double[,] matrix;
        public SimplexMatrix(Problem problem)
        {
            matrix = new double[problem.Constraints.Count, problem.functionVariables.Count + 4];

        }
    }
}
