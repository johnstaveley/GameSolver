﻿namespace GameSolver.Exceptions
{
    public class OptimisationException : Exception
    {
        private OptimisationException(string message) : base(message)
        {
        }

        public static OptimisationException NoSolution() => new("No solution was found.");
    }
}
