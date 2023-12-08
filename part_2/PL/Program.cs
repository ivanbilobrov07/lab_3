using PL;
using BLL;

namespace ConsoleApp
{
    public enum Actions
    {
        Add,
        Remove,
        Print,
        Restart,
        Stop,
        CalculatePercentage,
    }
    public enum Entities
    {
        Student,
        Acrobat,
        TaxiDriver,
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                string type = ConsoleMenu.defineSerealizationType();
                Console.Clear();

                string fileName = ConsoleMenu.getFileName();
                Console.Clear();

                Actions action = ConsoleMenu.defineAction();

                EntityService service = new EntityService(type, fileName);


                Console.Clear();
                while (action != Actions.Restart)
                {
                    Console.Clear();

                    switch (action)
                    {
                        case Actions.Add:
                            {
                                Entities entity = ConsoleMenu.defineEntity();
                                Console.Clear();

                                string person = "";

                                if (entity == Entities.Student)
                                {
                                    var studentInfo = ConsoleMenu.GetStudentInfo();

                                    string firstName = studentInfo.Item1;
                                    string lastName = studentInfo.Item2;
                                    string studentId = studentInfo.Item3;
                                    int course = studentInfo.Item4;
                                    string hometown = studentInfo.Item5;
                                    string passportNumber = studentInfo.Item6;

                                    person = service.AddStudent(firstName, lastName, course, studentId, hometown, passportNumber);
                                }
                                else if (entity == Entities.Acrobat || entity == Entities.TaxiDriver)
                                {
                                    string firstName = ConsoleMenu.GetFirstName();
                                    string lastName = ConsoleMenu.GetLastName();

                                    person = entity == Entities.Acrobat ? service.AddAcrobat(firstName, lastName) : service.AddTaxiDriver(firstName, lastName);
                                }

                                Console.WriteLine("{" + person! + "} was added successfully");

                                action = ConsoleMenu.defineAction();
                                break;
                            }
                        case Actions.Remove:
                            {
                                service.Print();

                                int index = ConsoleMenu.getUserIndexFromList("Enter an index of entity to delete", service.EntityList.Count);

                                try
                                {
                                    string person = service.Remove(index);
                                    Console.WriteLine("{" + person + "} was deleted successfully");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }

                                action = ConsoleMenu.defineAction();
                                break;
                            }
                        case Actions.Print:
                            {
                                service.Print();

                                action = ConsoleMenu.defineAction();
                                break;
                            }
                        case Actions.CalculatePercentage:
                            {
                                float percentage = service.CalculatePercentage();
                                Console.WriteLine("The percentage of 1 year students who came from other cities - " + Math.Round(percentage));

                                action = ConsoleMenu.defineAction();
                                break;
                            }
                        case Actions.Restart:
                            {
                                Console.Clear();
                                break;
                            }
                        case Actions.Stop:
                            {
                                return;
                            }
                    }
                }
            }
        }
    }
}