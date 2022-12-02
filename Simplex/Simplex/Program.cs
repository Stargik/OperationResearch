namespace Simplex
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger consoleLogger = new ConsoleLogger();
            Problem problem = new Problem();
            Console.WriteLine("Enter the function variables: ");
            List<double> functionVariables = Console.ReadLine().Split(' ').Select(s => Convert.ToDouble(s)).ToList();
            problem.functionVariables = functionVariables;
            Console.WriteLine("Enter the type of optimum(min/max): ");
            string optimumType = Console.ReadLine();
            problem.OptimalType = optimumType.ToLower() == "max" ? OptimalType.Max : OptimalType.Min;
            Console.WriteLine("Enter the number of constraints: ");
            int constraintsCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < constraintsCount; i++)
            {
                Console.WriteLine("Enter the constraints variables: ");
                List<double> constraintsVariables = Console.ReadLine().Split(' ').Select(s => Convert.ToDouble(s)).ToList();
                Console.WriteLine("Enter the constraints type(=/<=/>=): ");
                string constraintTypeInput = Console.ReadLine();
                ConstraintType constraintType;
                switch (constraintTypeInput)
                {
                    case ("<="):
                        constraintType = ConstraintType.LessOrEqual;
                        break;
                    case (">="):
                        constraintType = ConstraintType.MoreOrEqual;
                        break;
                    default:
                        constraintType = ConstraintType.Equal;
                        break;
                }
                Constraint constraint = new Constraint { ConstraintType = constraintType, Variables = constraintsVariables };
                problem.Constraints.Add(constraint);
            }
            SimplexCalculator simplexCalculator = new SimplexCalculator(consoleLogger);
            Problem canonicalProblem = simplexCalculator.ConverteToCanonicalForm(problem);
            Problem mProblem = simplexCalculator.ConvertToMProblem(canonicalProblem);
            simplexCalculator.RunSimplexMethod(mProblem);
            consoleLogger.Show();

            //ConsoleLogger consoleLogger = new ConsoleLogger();
            //Problem problem = new Problem();
            //problem.functionVariables = new List<double> { 3, 2 };
            //problem.OptimalType = OptimalType.Max;
            //problem.Constraints = new List<Constraint>();
            //problem.Constraints.Add(new Constraint { ConstraintType = ConstraintType.LessOrEqual, Variables = new List<double> { 5, 3, 17 } });
            //problem.Constraints.Add(new Constraint { ConstraintType = ConstraintType.MoreOrEqual, Variables = new List<double> { 3, 2, 3 } });
            //problem.Constraints.Add(new Constraint { ConstraintType = ConstraintType.LessOrEqual, Variables = new List<double> { -4, 3, 7 } });
            //SimplexCalculator simplexCalculator = new SimplexCalculator(consoleLogger);
            //Problem canonicalProblem = simplexCalculator.ConverteToCanonicalForm(problem);
            //Problem mProblem = simplexCalculator.ConvertToMProblem(canonicalProblem);
            //simplexCalculator.RunSimplexMethod(mProblem);
            //consoleLogger.Show();
            Console.ReadKey();
        }
    }
}