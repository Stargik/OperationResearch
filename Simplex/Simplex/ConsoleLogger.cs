using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex
{
    internal class ConsoleLogger
    {
        readonly StringBuilder log = new StringBuilder();
        public void AddStep(Problem problem, string message)
        {
            log.AppendLine(message);
            log.Append("Function: ");
            foreach (var (item, index) in problem.functionVariables.Select((value, i) => (value, i)))
            {
                string varName = index >= problem.functionVariables.Count - problem.MVariablesCount ? "y" : "x"; 
                log.Append(item + varName + (index + 1) + " ");
            }
            log.AppendLine("-> " + problem.OptimalType);
            log.AppendLine("Constraints: ");

            foreach (var constraint in problem.Constraints)
            {
                for (int i = 0; i < constraint.Variables.Count - 1; i++)
                {
                    string varName = i >= problem.functionVariables.Count - problem.MVariablesCount ? "y" : "x";
                    log.Append(constraint.Variables[i] + varName + (i + 1) + " ");
                }
                switch (constraint.ConstraintType)
                {
                    case ConstraintType.Equal:
                        log.AppendLine("= " + constraint.Variables[constraint.Variables.Count - 1]);
                        break;
                    case ConstraintType.LessOrEqual:
                        log.AppendLine("<= " + constraint.Variables[constraint.Variables.Count - 1]);
                        break;
                    case ConstraintType.MoreOrEqual:
                        log.AppendLine(">= " + constraint.Variables[constraint.Variables.Count - 1]);
                        break;
                    default:
                        break;
                }
            }
            log.AppendLine(new string('-', 30));
        }
        public void AddStep(double[,] matrix)
        {
            for (int i = 1; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (j == 1 && i >= 1 && i < matrix.GetLength(0) - 1)
                    {
                        string varName = matrix[i, j] >= matrix.GetLength(1) - 4 - matrix[matrix.GetLength(0) - 1, 0] ? "y" : "x";
                        DrawCell(varName + (matrix[i, j] + 1));
                    }else if ( j == 1 && i == matrix.GetLength(0) - 1)
                    {
                        DrawCell("Delta");
                    }
                    else if ((i == matrix.GetLength(0) - 1 && (j < 2 || j == matrix.GetLength(1) - 1)) || (j == matrix.GetLength(1) - 1 && matrix[i, j] < 0))
                    {
                        DrawCell(" ");
                    }
                    else
                    {
                        DrawCell(matrix[i, j]);
                    }
                }
                log.AppendLine("|");
            }
            log.AppendLine(new string('-', 100));
        }
        public void Show()
        {
            Console.WriteLine(log);
        }
        public void DrawMatrixHeader(double[,] matrix, string message)
        {
            log.AppendLine(message);
            DrawCell("Cb");
            DrawCell("Xb");
            for (int i = 0; i < matrix.GetLength(1) - 4; i++)
            {
                if (i >= matrix.GetLength(1) - 4 - matrix[matrix.GetLength(0) - 1, 0])
                {
                    DrawCell("A" + (i + 1) + "(M)");
                }
                else
                {
                    DrawCell("A" + (i + 1) + "(" + matrix[0, i + 2] + ")");
                }
                
            }
            DrawCell("B");
            DrawCell("Q");
            log.AppendLine("|");
        }
        private void DrawCell(double value)
        {

            log.Append(string.Format($"|{Math.Round(value, 2),8}"));
        }
        private void DrawCell(string value)
        {

            log.Append(string.Format($"|{value,8}"));
        }
        public void AddResult(string message)
        {
            log.AppendLine(message);
        }
        public void AddResult(List<double> vector, double value, string message)
        {
            log.Append(message + "L (");
            for (int i = 0; i < vector.Count; i++)
            {
                if (i != vector.Count - 1)
                {
                    log.Append(Math.Round(vector[i], 2) + ", ");
                }
                else
                {
                    log.Append(Math.Round(vector[i], 2) + ") = ");
                }
            }
            log.AppendLine(Math.Round(value, 2).ToString());
        }
    }

}
