using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentSearchEngine
{
    internal static class VectorExtensions
    {
        public static double Magnitude(this IEnumerable<double> xs)
        {
            return Math.Sqrt(xs.Sum(x => x * x));
        }

        public static double CrossProduct(this IEnumerable<double> xs, IEnumerable<double> ys)
        {
            return xs.Magnitude() * ys.Magnitude();
        }

        public static double DotProduct(this IEnumerable<double> xs, IEnumerable<double> ys)
        {
            return xs.Zip(ys, (x, y) => x * y).Sum();
        }

        public static double Cosine(this IEnumerable<double> xs, IEnumerable<double> ys)
        {
            return xs.DotProduct(ys) / xs.CrossProduct(ys);
        }
    }
}