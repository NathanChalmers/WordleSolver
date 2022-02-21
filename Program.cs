using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WordleSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            NodeCollection words = new NodeCollection();
            words.AddDictionary("./words_alpha.txt");

            Console.WriteLine("WorldeSolver!\n\n");
            Console.WriteLine("Instructions: Answer each of the prompts using the following options to generate a new guess");
            Console.WriteLine("1. Enter letters in format of letter followed by immediately by the position. (E.g a1)");
            Console.WriteLine("2. Multiple Letters are seperated by spaces. (E.g b2 d5)");
            Console.WriteLine("3. Green Letters only need to be entered the frist time they appear");
            Console.WriteLine("4. If no letters match enter no");
            Console.WriteLine("5. If the wordle has been solved enter success");

            bool isSolved = false;
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine("\n\nGuess #" + i + ": " + words.MostLikely());
                isSolved = ParseLockedLetters(words);

                if (isSolved)
                {
                    break;
                }

                ParseRemovedLetters(words);
                ParseFloatLetters(words);
            }

            if (!isSolved)
            {
                Console.WriteLine("\n\nWas the wordle successfully solved? (yes/no)");
                string response = Console.ReadLine().ToLower();

                if (response == "yes")
                {
                    isSolved = true;
                }
            }

            if (isSolved)
            {
                Console.WriteLine("\n\n Congratulations!");
            }
            else
            {
                Console.WriteLine("\n\n Sorry to let you donw. I'm usually pretty good at this");
            }

            Console.WriteLine("\n\n Press Enter to Exit");
            Console.ReadLine();
        }

        public static bool ParseLockedLetters(NodeCollection words)
        {
            Console.Write("Green/Locked Letters?:");

            string line = Console.ReadLine().Trim().ToLower();

            if (line == "no")
            {
                return false;
            }

            if (line == "success")
            {
                return true;
            }

            Regex r = new Regex("(?<value>[a-z])(?<level>[0-9])");
            MatchCollection matches = r.Matches(line);

            if (matches.Count == 0)
            {
                Console.WriteLine("Invalid Input\n");
                ParseLockedLetters(words);
            }

            for (int i = 0; i < matches.Count; i++)
            {
                string value = matches[i].Groups["value"].Value;
                int level = Convert.ToInt32(matches[i].Groups["level"].Value);

                words.LockLetter(level, value);
            }

            return false;
        }

        public static void ParseRemovedLetters(NodeCollection words)
        {
            Console.Write("Grey/Removed Letters?:");

            string line = Console.ReadLine().Trim().ToLower();

            if (line == "no")
            {
                return;
            }

            Regex r = new Regex("(?<value>[a-z])(?<level>[0-9])");
            MatchCollection matches = r.Matches(line);

            if (matches.Count == 0)
            {
                Console.WriteLine("Invalid Input\n");
                ParseRemovedLetters(words);
            }

            foreach (Match match in matches)
            {
                string value = match.Groups["value"].Value;

                words.RemoveLetter(value);
            }
        }

        public static void ParseFloatLetters(NodeCollection words)
        {
            Console.Write("Orange/Mismatched Letters?:");

            string line = Console.ReadLine().Trim().ToLower();

            if (line == "no")
            {
                return;
            }

            Regex r = new Regex("(?<value>[a-z])(?<level>[0-9])");
            MatchCollection matches = r.Matches(line);

            if (matches.Count == 0)
            {
                Console.WriteLine("Invalid Input\n");
                ParseRemovedLetters(words);
            }

            Dictionary<string, List<int>> oranges = new Dictionary<string, List<int>>();

            foreach (Match match in matches)
            {
                string value = match.Groups["value"].Value;
                int level = Convert.ToInt32(match.Groups["level"].Value);

                if (!oranges.ContainsKey(value))
                {
                    oranges.Add(value, new List<int>());
                }

                oranges[value].Add(level);
            }

            foreach (string value in oranges.Keys)
            {
                int[] levels = oranges[value].ToArray();

                words.FloatLetter(levels, value);
            }
        }
    }
}
