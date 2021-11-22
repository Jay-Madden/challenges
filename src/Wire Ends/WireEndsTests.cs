using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualBasic;

namespace Wire_Ends
{
    public static class Extensions
    {
        public static IEnumerable<T> GetUniqueFlags<T>(this T flags) where T : Enum =>
            Enum.GetValues(flags.GetType())
                .Cast<Enum>()
                .Where(flags.HasFlag)
                .Select(value => (T) value);
    }
    
    [TestClass]
    public class WireEndsTests
    {
        [Flags]
        enum Direction : short
        {
            Up = 1,
            Down= 2,
            Left = 4,
            Right = 8,
            Empty = 16
        }
        
        public int CountWireEnds(string s)
        {
            var chars = "║╠╩╦═╬╣╚╝╔╗ \n";

            var directions = new Dictionary<string, Direction>
            {
                {"║", Direction.Up | Direction.Down},
                {"╠", Direction.Up | Direction.Down | Direction.Right},
                {"╩", Direction.Up | Direction.Left | Direction.Right},
                {"╦", Direction.Down | Direction.Left | Direction.Right},
                {"═", Direction.Left | Direction.Right},
                {"╬", Direction.Left | Direction.Up | Direction.Down | Direction.Right},
                {"╣", Direction.Left | Direction.Up | Direction.Down},
                {"╚", Direction.Up | Direction.Right},
                {"╝", Direction.Up | Direction.Left},
                {"╔", Direction.Down | Direction.Right},
                {"╗", Direction.Down | Direction.Left},
                {" ", Direction.Empty }
            };

            var actions = new Dictionary<Direction, Func<List<List<string>>, int, int, int>>
            {
                {
                    Direction.Up, 
                    (array, i, j) => i == 0 || !directions[array[i - 1][j]].HasFlag(Direction.Down) ? 1 : 0
                },
                {
                    Direction.Down, 
                    (array, i, j) => i >= array.Count - 1 || !directions[array[i + 1][j]].HasFlag(Direction.Up) ? 1 : 0
                },
                {
                    Direction.Left,
                    (array, i, j) => j == 0 || !directions[array[i][j - 1]].HasFlag(Direction.Right) ? 1 : 0
                },
                {
                    Direction.Right,
                    (array, i, j) => j >= array[i].Count - 1 || !directions[array[i][j + 1]].HasFlag(Direction.Left) ? 1 : 0
                },
                { Direction.Empty, (_, _, _) => 0 }
            };
            var looseEndCount = 0;

            var array = s.Split("\n")
                .Select(x =>
                    x.Select(y =>
                        y.ToString())
                        .ToList())
                .ToList();

            for (var i = 0; i < array.Count; i++)
            {
                for (var j = 0; j < array[i].Count; j++)
                {
                    looseEndCount += directions[array[i][j]]
                        .GetUniqueFlags()
                        .Sum(flag => actions[flag](array, i, j));
                }
            }
            return looseEndCount;
        }

        [TestMethod]
        public void Test()
        {
            (string Input, int Output)[] testCases =
            {
                (
                    "", 0
                ),
                (
                    " ", 0
                ),
                (
                    "║", 2
                ),
                (
                    "╠", 3
                ),
                (
                    "╩", 3
                ),
                (
                    "╦", 3
                ),
                (
                    "═", 2
                ),
                (
                    "╬", 4
                ),
                (
                    "╣", 3
                ),
                (
                    "╚", 2
                ),
                (
                    "╝", 2
                ),
                (
                    "╔", 2
                ),
                (
                    "╗", 2
                ),
                ("║╠╩╦═╬╣╚╝╔╗ ", 14),

                (
                    "╔╦╦══╦╗\n" +
                    "║╠╩╦═╬╣\n" +
                    "╚╩═╩═╩╝\n",
                    0
                ),
                (
                    "║╔╗║\n" +
                    "║║║║\n" +
                    "║║║║\n" +
                    "╚╝╚╝",
                    2
                ),
                (
                    "╔══╗\n" +
                    "║╔╗║\n" +
                    "║╚╝║\n" +
                    "╚══╝",
                    0
                ),
                (
                    "╔╦╦╗\n" +
                    "╠╬╬╣\n" +
                    "╠╬╬╣\n" +
                    "╚╩╩╝",
                    0
                ),
                (
                    "╔══╗\n" +
                    "║╬╬║\n" +
                    "║╬╬║\n" +
                    "╚══╝",
                    8
                ),
                (
                    "╔══╗\n" +
                    "║╚╝║\n" +
                    "║╔╗║\n" +
                    "╚══╝",
                    4
                ),
                (
                    " ╔╗\n" +
                    "╔╝╚═╗\n" +
                    "╚╗  ╚═╗\n" +
                    " ╚════╝",
                    0
                ),
                (
                    " ╔╗\n" +
                    "╔╝╚╦╗\n" +
                    "╚╦═╣╚═╗\n" +
                    " ╚═╩══╝",
                    0
                ),
                (
                    " ╔╗\n" +
                    "╔╝╚╦╗\n" +
                    "╚╦  ╚═╗\n" +
                    " ╚═╩══╝",
                    3
                ),
                (
                    "╔═════════╗\n" +
                    "╚═╗       ║\n" +
                    "  ╚═╗     ║\n" +
                    "    ╚═╗   ║\n" +
                    "      ╚═╗ ║\n" +
                    "        ╚═╝\n",
                    0
                ),
                (
                    "╔════╦══╦═╗\n" +
                    "╚═╗  ╚╗ ╚═╣\n" +
                    "  ╚═╗ ╚═╗ ║\n" +
                    "    ╚═╗ ╚═╣\n" +
                    "      ╚═╗ ║\n" +
                    "        ╚═╝\n",
                    0
                ),
                (
                    "╔ ═ ═╦══ ═ \n" +
                    " ═╗   ╗ ╚ ╣\n" +
                    "   ═╗  ═  ║\n" +
                    "     ═╗ ╚  \n" +
                    "       ═╗  \n" +
                    "         ═╝\n",
                    30
                ),
                (
                    "╔═══════════════════╦═══════════════════╗\n" +
                    "║                   ║                   ║\n" +
                    "║   ╔═╗   ╔═════╗   ║   ╔═════╗   ╔═╗   ║\n" +
                    "║   ╚═╝   ╚═════╝   ║   ╚═════╝   ╚═╝   ║\n" +
                    "║                                       ║\n" +
                    "║   ═══   ║   ══════╦══════   ║   ═══   ║\n" +
                    "║         ║         ║         ║         ║\n" +
                    "╚═════╗   ╠══════   ║   ══════╣   ╔═════╝\n" +
                    "      ║   ║                   ║   ║      \n" +
                    "══════╝   ║   ╔════   ════╗   ║   ╚══════\n" +
                    "              ║           ║              \n" +
                    "══════╗   ║   ║           ║   ║   ╔══════\n" +
                    "      ║   ║   ╚═══════════╝   ║   ║      \n" +
                    "      ║   ║                   ║   ║      \n" +
                    "╔═════╝   ║   ══════╦══════   ║   ╚═════╗\n" +
                    "║                   ║                   ║\n" +
                    "║   ══╗   ═══════   ║   ═══════   ╔══   ║\n" +
                    "║     ║                           ║     ║\n" +
                    "╠══   ║   ║   ══════╦══════   ║   ║   ══╣\n" +
                    "║         ║         ║         ║         ║\n" +
                    "║   ══════╩══════   ║   ══════╩══════   ║\n" +
                    "║                                       ║\n" +
                    "╚═══════════════════════════════════════╝",
                    46
                ),
            };

            foreach (var (input, output) in testCases)
            {
                try
                {
                    int actualOutput = CountWireEnds(input);
                    Assert.IsTrue(actualOutput == output);
                }
                catch (NotImplementedException)
                {
                    Assert.Inconclusive();
                }
            }
        }
    }
}