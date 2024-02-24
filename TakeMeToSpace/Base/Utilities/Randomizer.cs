using System;

namespace TakeMeToSpace.Base.Utilities;

public static class Randomizer
{
    private static readonly Random Random = new();

    public static int Next(int min, int max)
    {
        return Random.Next(min, max);
    }

    public static float NextFloat(float min, float max)
    {
        return (float)(Random.NextDouble() * (max - min)) + min;
    }

    public static bool TenPercentChance() => RandomIntBetweenZeroAndOneHundred() < 10;
    public static bool TwentyFivePercentChance() => RandomIntBetweenZeroAndOneHundred() < 25;
    public static bool FiftyPercentChance() => RandomIntBetweenZeroAndOneHundred() < 50;
    public static int RandomIntBetweenZeroAndOneHundred() => Random.Next(0, 101);
}