using System;
using System.Collections.Generic;
using DoubleDict;

namespace DoubleDictExample
{
    class Program
    {
        static void Main(string[] args)
        {
            DoubleDictOneToOne<string, string> FirstNameLastNameDoubleDict = new DoubleDictOneToOne<string, string>();

            FirstNameLastNameDoubleDict.AddPair("Jim", "Doe");
            FirstNameLastNameDoubleDict.AddPair("Sally", "Wang");
            FirstNameLastNameDoubleDict.AddPair("John", "Smith");

            List<string> FirstNames = FirstNameLastNameDoubleDict.GetPrimaryKeys();
            List<string> LastNames = FirstNameLastNameDoubleDict.GetSecondaryKeys();

            foreach(string firstName in FirstNames)
            {
                Console.WriteLine(firstName + " " + FirstNameLastNameDoubleDict.GetSecondary(firstName));
            }

            foreach (string lastName in LastNames)
            {
                Console.WriteLine(FirstNameLastNameDoubleDict.GetPrimary(lastName) + " " + lastName);
            }

            /*
             * Both outputs should read:
             * Jim Doe
             * Sally Wang
             * John Smith
             */

        }
    }
}
