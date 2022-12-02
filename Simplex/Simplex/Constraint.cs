using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex
{
    enum ConstraintType
    {
        Equal,
        LessOrEqual,
        MoreOrEqual
    }
    internal class Constraint
    {
        public Constraint()
        {
            Variables = new List<double>();
        }
        public ConstraintType ConstraintType { get; set; }
        public List<double> Variables { get; set; }
    }
}
