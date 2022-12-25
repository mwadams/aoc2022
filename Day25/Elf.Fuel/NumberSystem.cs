namespace Elf.Fuel;

using System;
using System.Text;

internal static class NumberSystem
{
    public static long[] PowersOfFive = BuildPowersOfFive();

    private static long[] BuildPowersOfFive()
    {
        long[] memory = new long[50];
        long runningTotal = 1;
        for(long i = 0; i < 25; ++i)
        {
            memory[i] = runningTotal;
            runningTotal *= 5;
        }

        return memory;
    }

    public static long GetNumber(ReadOnlySpan<char> number)
    {
        long runningTotal = 0;
        for(int i = 0; i < number.Length; ++i)
        {
            char val = number[^(i+1)];
            runningTotal += PowersOfFive[i] * GetNumber(val);
        }

        return runningTotal;
    }

    public static string BuildSnafu(long number)
    {
        int characterIndex = 0;
        Span<char> characters = stackalloc char[256];
        while(number > 0)
        {
            (number, long remainder) = Math.DivRem(number, 5);
            characters[characterIndex++] = GetSnafuChar(remainder);
            if (remainder > 2)
            {
                // Last chance for an off-by-one error
                number += 1;
            }
        }

        Span<char> reversed = characters[..characterIndex];
        reversed.Reverse();
        return reversed.ToString();
    }

    private static char GetSnafuChar(long remainder)
    {
        // This is basically the inverse of GetNumber below
        return remainder switch
        {
            3 => '=',
            4 => '-',
            0 => '0',
            1 => '1',
            2 => '2',
            _ => throw new NotSupportedException(),
        };
    }

    private static long GetNumber(char val)
    {
        return val switch
        {
            '=' => -2,
            '-' => -1,
            '0' => 0,
            '1' => 1,
            '2' => 2,
            _ => throw new NotSupportedException(),
        };
    }
}
