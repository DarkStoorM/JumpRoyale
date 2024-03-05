using System;

namespace JumpRoyale.Utils;

public static class PerlinNoiseGenerator
{
    private static readonly int[] Permutation = []; // Insert 256 values here

    private static readonly int[] P = new int[512];

    static PerlinNoiseGenerator()
    {
        for (int i = 0; i < 512; i++)
        {
            P[i] = Permutation[i % 256];
        }
    }

    public static double GeneratePerlinNoise(double x, double y)
    {
        int xIndex = (int)Math.Floor(x) & 255;
        int yIndex = (int)Math.Floor(y) & 255;

        x -= Math.Floor(x);
        y -= Math.Floor(y);

        double u = Fade(x);
        double v = Fade(y);

        int a = P[xIndex] + yIndex;
        int aa = P[a];
        int ab = P[a + 1];
        int b = P[xIndex + 1] + yIndex;
        int ba = P[b];
        int bb = P[b + 1];

        return Lerp(
            v,
            Lerp(u, Grad(P[aa], x, y), Grad(P[ba], x - 1, y)),
            Lerp(u, Grad(P[ab], x, y - 1), Grad(P[bb], x - 1, y - 1))
        );
    }

    private static double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static double Lerp(double t, double a, double b)
    {
        return a + t * (b - a);
    }

    private static double Grad(int hash, double x, double y)
    {
        int h = hash & 15;
        double grad = 1 + (h & 7); // Gradient value 1-8

        // Randomly invert half of them
        if ((h & 8) != 0)
        {
            grad = -grad;
        }

        return grad * x + grad * y;
    }
}
