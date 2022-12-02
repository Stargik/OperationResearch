using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex
{
    internal class SimplexCalculator
    {
        private double M;
        readonly ConsoleLogger logger;
        public SimplexCalculator(ConsoleLogger logger)
        {
            this.logger = logger;
            M = double.MaxValue;
        }


        public Problem ConverteToCanonicalForm(Problem problem)
        {
            logger.AddStep(problem, "Problem:");
            Problem CanonicalProblem = new Problem();
            if (problem.OptimalType == OptimalType.Max)
            {
                CanonicalProblem.IsConvertedToMin = true;
            }
            CanonicalProblem.OptimalType = OptimalType.Min;
            if (problem.OptimalType == OptimalType.Max)
            {
                foreach (var item in problem.functionVariables)
                {
                    CanonicalProblem.functionVariables.Add(-item);
                }
            }
            else
            {
                foreach (var item in problem.functionVariables)
                {
                    CanonicalProblem.functionVariables.Add(item);
                }
            }
            foreach (var constraint in problem.Constraints)
            {
                Constraint newConstraint = new Constraint();
                newConstraint.ConstraintType = ConstraintType.Equal;
                if (constraint.Variables[constraint.Variables.Count - 1] < 0)
                {
                    foreach (var item in constraint.Variables)
                    {
                        newConstraint.Variables.Add(-item);
                    }
                }
                else
                {
                    foreach (var item in constraint.Variables)
                    {
                        newConstraint.Variables.Add(item);
                    }
                }
                CanonicalProblem.Constraints.Add(newConstraint);
            }
            for (int i = 0; i < problem.Constraints.Count; i++)
            {
                var constraint = problem.Constraints[i];


                if (constraint.Variables[constraint.Variables.Count - 1] < 0 && constraint.ConstraintType != ConstraintType.Equal)
                {
                    for (int j = 0; j < constraint.Variables.Count; j++)
                    {
                        constraint.Variables[j] = -constraint.Variables[j];
                    }
                    constraint.ConstraintType = constraint.ConstraintType == ConstraintType.LessOrEqual ? ConstraintType.MoreOrEqual : ConstraintType.LessOrEqual;
                }


                if (constraint.ConstraintType == ConstraintType.LessOrEqual)
                {
                    CanonicalProblem.Constraints[i].Variables.Insert(CanonicalProblem.Constraints[i].Variables.Count - 1, 1);
                    CanonicalProblem.AdditionalVariablesCount++;
                    for (int j = 0; j < CanonicalProblem.Constraints.Count; j++)
                    {
                        if (i != j)
                        {
                            CanonicalProblem.Constraints[j].Variables.Insert(CanonicalProblem.Constraints[j].Variables.Count - 1, 0);
                        }
                    }
                    CanonicalProblem.functionVariables.Add(0);
                }
                else if (constraint.ConstraintType == ConstraintType.MoreOrEqual)
                {
                    CanonicalProblem.Constraints[i].Variables.Insert(CanonicalProblem.Constraints[i].Variables.Count - 1, -1);
                    CanonicalProblem.AdditionalVariablesCount++;
                    for (int j = 0; j < CanonicalProblem.Constraints.Count; j++)
                    {
                        if (i != j)
                        {
                            CanonicalProblem.Constraints[j].Variables.Insert(CanonicalProblem.Constraints[j].Variables.Count - 1, 0);
                        }
                    }
                    CanonicalProblem.functionVariables.Add(0);
                }
            }
            logger.AddStep(CanonicalProblem, "Canonical problem:");
            return CanonicalProblem;
        }
        public Problem ConvertToMProblem(Problem problem)
        {
            double abMax = Math.Abs(problem.Constraints.SelectMany(c => c.Variables).OrderByDescending(c => Math.Abs(c)).First());
            double cMax = Math.Abs(problem.functionVariables.OrderByDescending(c => Math.Abs(c)).First());
            M = abMax > cMax ? abMax + 10 : cMax + 10;
            Problem MProblem = new Problem();
            MProblem.AdditionalVariablesCount = problem.AdditionalVariablesCount;
            MProblem.OptimalType = OptimalType.Min;
            MProblem.IsConvertedToMin = problem.IsConvertedToMin;
            foreach (var item in problem.functionVariables)
            {
                MProblem.functionVariables.Add(item);
            }
            foreach (var constraint in problem.Constraints)
            {
                Constraint newConstraint = new Constraint();
                newConstraint.ConstraintType = ConstraintType.Equal;
                foreach (var item in constraint.Variables)
                {
                    newConstraint.Variables.Add(item);
                }
                MProblem.Constraints.Add(newConstraint);
            }

            for (int i = 0; i < MProblem.Constraints.Count; i++)
            {
                bool findBasicVector = false;
                for (int j = 0; j < MProblem.Constraints[i].Variables.Count; j++)
                {

                    if (MProblem.Constraints[i].Variables[j] > 0)
                    {
                        bool isZero = true;
                        double basicValue = MProblem.Constraints[i].Variables[j];
                        for (int k = 0; k < MProblem.Constraints.Count; k++)
                        {
                            if (MProblem.Constraints[k].Variables[j] != 0 && k != i)
                            {
                                isZero = false;
                            }
                        }
                        if (isZero)
                        {
                            for (int l = 0; l < MProblem.Constraints[i].Variables.Count; l++)
                            {
                                MProblem.Constraints[i].Variables[l] /= basicValue;
                            }
                            findBasicVector = true;
                            break;
                        }
                    }
                }
                if (!findBasicVector)
                {
                    MProblem.Constraints[i].Variables.Insert(MProblem.Constraints[i].Variables.Count - 1, 1);
                    MProblem.functionVariables.Add(M);
                    MProblem.MVariablesCount++;
                    for (int k = 0; k < MProblem.Constraints.Count; k++)
                    {
                        if (i != k)
                        {
                            MProblem.Constraints[k].Variables.Insert(MProblem.Constraints[k].Variables.Count - 1, 0);
                        }
                    }
                }
            }
            logger.AddStep(MProblem, "M problem:");
            return MProblem;
        }
        public void RunSimplexMethod(Problem problem)
        {
            double[,] matrix = FillTheMatrix(problem);
            CalculateSimplexDelta(matrix);
            int inIndex = GetIndexOfMinDelta(matrix);
            int outIndex = GetIndexOfMinPositiveQ(matrix, inIndex);
            logger.DrawMatrixHeader(matrix, "Matrix:");
            logger.AddStep(matrix);

            bool flag = true;
            while ((matrix[matrix.GetLength(0) - 1, inIndex] < 0) && flag)
            {
                M = GetMaxAbsValue(matrix) + 10;
                SetMValue(matrix, M);
                ChangeBasicVector(matrix, outIndex, inIndex);
                CalculateSimplexDelta(matrix);
                inIndex = GetIndexOfMinDelta(matrix);
                if (matrix[matrix.GetLength(0) - 1, inIndex] >= 0)
                {
                    for (int i = 1; i < matrix.GetLength(0) - 1; i++)
                    {
                        matrix[i, matrix.GetLength(1) - 1] = -1;
                    }
                    logger.AddStep(matrix);
                    break;
                }
                outIndex = GetIndexOfMinPositiveQ(matrix, inIndex);
                logger.AddStep(matrix);
                if (outIndex < 0)
                {
                    logger.AddResult("Solution is not exist (f -> infinity).");
                    flag = false;
                }
            }
            if (flag)
            {
                for (int i = 1; i < matrix.GetLength(0) - 1; i++)
                {
                    for (int j = problem.functionVariables.Count - problem.MVariablesCount; j < problem.functionVariables.Count; j++)
                    {
                        if (matrix[i, 1] == j)
                        {
                            logger.AddResult("Problem doesn't have any solution.");
                            flag = false;
                        }
                    }
                }

            }
            if (flag)
            {
                List<double> vector = new();

                for (int i = 0; i < problem.functionVariables.Count - problem.MVariablesCount - problem.AdditionalVariablesCount; i++)
                {
                    var isInBasis = false;
                    for (int j = 1; j < matrix.GetLength(0) - 1; j++)
                    {
                        if (matrix[j, 1] == i)
                        {
                            vector.Add(matrix[j, matrix.GetLength(1) - 2]);
                            isInBasis = true;
                        }
                    }
                    if (!isInBasis)
                    {
                        vector.Add(0);
                    }
                }
                double value = matrix[matrix.GetLength(0) - 1, matrix.GetLength(1) - 2];
                if (problem.IsConvertedToMin)
                {
                    value = -value;
                }
                logger.AddResult(vector, value, "Result: ");
            }

        }
        private double[,] FillTheMatrix(Problem problem)
        {
            double[,] matrix = new double[problem.Constraints.Count + 2, problem.functionVariables.Count + 4];
            for (int i = 0; i < problem.Constraints.Count; i++)
            {
                for (int j = 0; j < problem.Constraints[i].Variables.Count; j++)
                {
                    if (problem.Constraints[i].Variables[j] == 1)
                    {
                        bool isZero = true;
                        for (int k = 0; k < problem.Constraints.Count; k++)
                        {
                            if (problem.Constraints[k].Variables[j] != 0 && k != i)
                            {
                                isZero = false;
                            }
                        }
                        if (isZero)
                        {
                            matrix[i + 1, 0] = problem.functionVariables[j];
                            matrix[i + 1, 1] = j;
                            break;
                        }
                    }
                }
            }
            for (int j = 0; j < problem.functionVariables.Count; j++)
            {
                matrix[0, j + 2] = problem.functionVariables[j];
            }
            for (int i = 0; i < problem.Constraints.Count; i++)
            {
                for (int j = 0; j < problem.functionVariables.Count + 1; j++)
                {
                    matrix[i + 1, j + 2] = problem.Constraints[i].Variables[j];
                }
            }
            matrix[problem.Constraints.Count + 1, 0] = problem.MVariablesCount;
            return matrix;
        }
        private void CalculateSimplexDelta(double[,] matrix)
        {
            for (int i = 2; i < matrix.GetLength(1) - 1; i++)
            {
                double delta = matrix[0, i];
                for (int j = 0; j < matrix.GetLength(0) - 1; j++)
                {
                    if (i != matrix.GetLength(1) - 2)
                    {
                        delta -= matrix[j, 0] * matrix[j, i];
                    }
                    else
                    {
                        delta += matrix[j, 0] * matrix[j, i];
                    }
                }
                matrix[matrix.GetLength(0) - 1, i] = delta;
            }
        }
        private int GetIndexOfMinDelta(double[,] matrix)
        {
            int minIndex = 2;
            for (int i = 3; i < matrix.GetLength(1) - 2; i++)
            {
                if (matrix[matrix.GetLength(0) - 1, minIndex] > matrix[matrix.GetLength(0) - 1, i])
                {
                    minIndex = i;
                }
                else if (matrix[matrix.GetLength(0) - 1, minIndex] == matrix[matrix.GetLength(0) - 1, i])
                {
                    int minQIndex = GetIndexOfMinPositiveQ(matrix, i);
                    int minCurrentQIndex = GetIndexOfMinPositiveQ(matrix, minIndex);
                    if (minQIndex >= 0 && minCurrentQIndex >= 0)
                    {
                        double minQValue = matrix[minQIndex, matrix.GetLength(1) - 2] / matrix[minQIndex, i];
                        double minCurrentQValue = matrix[minCurrentQIndex, matrix.GetLength(1) - 2] / matrix[minCurrentQIndex, minIndex];
                        if (minQValue > minCurrentQValue)
                        {
                            minIndex = i;
                        }
                    }
                }
            }
            return minIndex;
        }
        private int GetIndexOfMinPositiveQ(double[,] matrix, int minDeltaIndex)
        {
            int minQIndex = -1;
            for (int i = 1; i < matrix.GetLength(0) - 1; i++)
            {
                if (matrix[i, minDeltaIndex] > 0)
                {
                    matrix[i, matrix.GetLength(1) - 1] = matrix[i, matrix.GetLength(1) - 2] / matrix[i, minDeltaIndex];
                    if ((minQIndex == -1 || matrix[i, matrix.GetLength(1) - 1] < matrix[minQIndex, matrix.GetLength(1) - 1]) && matrix[i, matrix.GetLength(1) - 1] > 0)
                    {
                        minQIndex = i;
                    }
                }
                else
                {
                    matrix[i, matrix.GetLength(1) - 1] = -1;

                }
            }
            return minQIndex;
        }

        private void ChangeBasicVector(double[,] matrix, int outVector, int inVector)
        {
            matrix[outVector, 0] = matrix[0, inVector];
            matrix[outVector, 1] = inVector - 2;
            double divider = matrix[outVector, inVector];
            for (int i = 2; i < matrix.GetLength(1) - 1; i++)
            {
                matrix[outVector, i] /= divider;
            }
            for (int i = 1; i < matrix.GetLength(0) - 1; i++)
            {
                if (i != outVector && matrix[i, inVector] != 0)
                {
                    double coef = matrix[i, inVector] / matrix[outVector, inVector];
                    for (int j = 2; j < matrix.GetLength(1) - 1; j++)
                    {
                        matrix[i, j] -= matrix[outVector, j] * coef;
                    }
                }
            }
        }

        private double GetMaxAbsValue(double[,] matrix)
        {
            double maxAbsValue = 0;
            for (int i = 0; i < matrix.GetLength(0) - 1; i++)
            {
                for (int j = 2; j < matrix.GetLength(1) - 1; j++)
                {
                    if (Math.Abs(matrix[i, j]) > maxAbsValue)
                    {
                        maxAbsValue = Math.Abs(matrix[i, j]);
                    }
                }
            }
            return maxAbsValue;
        }
        private void SetMValue(double[,] matrix, double m)
        {
            for (int i = (int)(matrix.GetLength(1) - 2 - matrix[matrix.GetLength(0) - 1, 0]); i < matrix.GetLength(1) - 2; i++)
            {
                matrix[0, i] = m;
            }
        }
    }
}
