using System;
using System.IO;

namespace Minimization
{
    class Program
    {
        private const string InputFile = "../../../input.txt";
        private const string OutputFile = "../../../output.txt";
        
        static void Main(string[] args)
        {
            StreamReader inputStream = new StreamReader(InputFile);
            StreamWriter outputStream = new StreamWriter(OutputFile);
            string machineType = inputStream.ReadLine();
            switch (machineType)
            {
                case MachineType.Mealy:
                    MealyMachineMinimizer mealyMachineMinimizer = new MealyMachineMinimizer(inputStream, outputStream);
                    mealyMachineMinimizer.ReadMealyMachine();
                    while(mealyMachineMinimizer.Minimize()) {}
                    mealyMachineMinimizer.PrintMealy();
                    break;
                case  MachineType.Moore:
                    MooreMachineMinimizer mooreMachineMinimizer = new MooreMachineMinimizer(inputStream, outputStream);
                    mooreMachineMinimizer.ReadMooreMachine();
                    while(mooreMachineMinimizer.Minimize()) {}
                    mooreMachineMinimizer.PrintMoore();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Автомат не распознан");
            }
        }
    }
}