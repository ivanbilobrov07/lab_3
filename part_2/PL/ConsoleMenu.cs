using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BLL;
using ConsoleApp;

namespace PL
{
    internal class ConsoleMenu
    {
        static private string getPropertyValue(Func<string, bool> isValid, string message, string errorMessage)
        {
            string value;

            Console.WriteLine(message);
            value = Console.ReadLine()!;

            while (true)
            {
                try
                {
                    if (isValid(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(errorMessage);
                    value = Console.ReadLine()!;
                }
            }
        }

        static private int getPropertyValue(Func<int, bool> isValid, string message, string errorMessage)
        {
            string value;

            Console.WriteLine(message);
            value = Console.ReadLine()!;

            while (true)
            {
                try
                {
                    int parsedValue = int.Parse(value);
                    if (isValid(parsedValue))
                    {
                        return parsedValue;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please, enter only a number");
                    value = Console.ReadLine()!;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(errorMessage);
                    value = Console.ReadLine()!;
                }
            }
        }

        static private int getUserIndexFromList(string askMessage, string[] values)
        {
            Console.WriteLine(askMessage);

            List<string> valuesList = new List<string>(values);
            int index = -1;

            foreach (string value in valuesList)
            {
                Console.WriteLine($"{valuesList.IndexOf(value) + 1}. {value}");
            }

            bool firstIteration = true;

            while (!Helpers.isValidIndex(index, valuesList.Count))
            {
                if (!firstIteration) Console.WriteLine("Please choose the index from the list above");

                try
                {
                    index = int.Parse(Console.ReadLine()!) - 1;
                }
                catch
                {
                    index = -1;
                }

                firstIteration = false;
            }

            return index;
        }

        static public int getUserIndexFromList(string askMessage, int length)
        {
            Console.WriteLine(askMessage);

            int index = -1;

            bool firstIteration = true;

            while (!Helpers.isValidIndex(index, length))
            {
                if (!firstIteration) Console.WriteLine("Please choose the index from the list above");

                try
                {
                    index = int.Parse(Console.ReadLine()!) - 1;
                }
                catch
                {
                    index = -1;
                }

                firstIteration = false;
            }

            return index;
        }

        static public string defineSerealizationType()
        {
            string[] types = { "JSON", "Binary", "XML", "Custom" };
            int typeIndex = getUserIndexFromList("Choose type of serialization to work with: ", types);

            return types[typeIndex];
        }

        static public string getFileName()
        {
            Console.WriteLine("Enter the name of the file without extension");

            string fileName = Console.ReadLine()!;

            while (fileName.Contains("."))
            {
                Console.WriteLine("Please enter without extension");
                fileName = Console.ReadLine()!;
            }

            return fileName;
        }

        static public Entities defineEntity()
        {
            string[] entities = { "Student", "Acrobat", "Taxi driver" };
            int entityIndex = getUserIndexFromList("Choose an entity to work with", entities);

            if (entityIndex == 0)
            {
                return Entities.Student;
            }
            else if (entityIndex == 1)
            {
                return Entities.Acrobat;
            }
            else
            {
                return Entities.TaxiDriver;
            }
        }

        static public Actions defineAction()
        {
            string[] actions = { "Add a new entity to file", "Delete an entity from file", "Print all entities", "Search students", "Chosee another type of serialization", "Exit" };
            int actionIndex = getUserIndexFromList("Choose an action, you want to do: ", actions);

            switch (actionIndex)
            {
                case 0: return Actions.Add;
                case 1: return Actions.Remove;
                case 2: return Actions.Print;
                case 3: return Actions.CalculatePercentage;
                case 4: return Actions.Restart;
                case 5: return Actions.Stop;
                default: return Actions.Print;
            }
        }

        static public string GetStudentId()
        {
            return getPropertyValue(StudentValidation.isValidStudentId, "Enter the the student id:", "Please, enter the valid student id:");
        }

        static public string GetFirstName()
        {
            return getPropertyValue(StudentValidation.isValidName, "Enter the first name:", "Please, enter the valid first name:");
        }

        static public string GetLastName()
        {
            return getPropertyValue(StudentValidation.isValidName, "Enter the last name:", "Please, enter the valid last name:");
        }

        static public (string, string, string, int, string, string) GetStudentInfo()
        {
            string firstName = getPropertyValue(StudentValidation.isValidName, "Enter the first name of the student:", "Please, enter the valid first name:");
            string lastName = getPropertyValue(StudentValidation.isValidName, "Enter the last name of the student:", "Please, enter the valid last name:");
            string studentId = GetStudentId();
            int course = getPropertyValue(StudentValidation.isValidCourse, "Enter the course of the student:", "Please, enter the valid course:");
            string hometown = getPropertyValue(StudentValidation.isValidHometown, "Enter the hometown:", "Please, enter the valid hometown:");
            string passportNumber = getPropertyValue(StudentValidation.isValidPassportNumber, "Enter the passport number:", "Please, enter the valid passport number:");

            return (firstName, lastName, studentId, course, hometown, passportNumber);
        }
    }
}
