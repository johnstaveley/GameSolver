namespace GameSolver.Models
{
    public class Constraint
    {
        public int LeftX { get; }
        public int LeftY { get; }
        public int RightX { get; }
        public int RightY { get; }
        public ConstraintType Type { get; }
        public Constraint(int leftX, int leftY, int rightX, int rightY, ConstraintType constraintType)        
        {
            LeftX = leftX;
            LeftY = leftY;
            RightX = rightX;
            RightY = rightY;
            Type = constraintType;
        }
        public override string ToString() {
            return $"X1:{LeftX},Y1:{LeftY},X2:{RightX},Y2:{RightY},Type:{Type}";
        }
    }
    public enum ConstraintType
    {
        GreaterThan,
        LessThan,
        EqualTo10,
        EqualTo5
    }
}
