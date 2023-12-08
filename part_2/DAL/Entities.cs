using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace DAL
{
    public interface IPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public interface IDance
    {
        public string Dance();
    }

    public interface IStudy
    {
        public string Study();
    }

    public interface IDrive
    {
        public string Drive();
    }

    public interface IDoTrics
    {
        public string DoTrics();
    }

    [JsonDerivedType(typeof(Person), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(Student), typeDiscriminator: "student")]
    [JsonDerivedType(typeof(TaxiDriver), typeDiscriminator: "taxiDriver")]
    [JsonDerivedType(typeof(Acrobat), typeDiscriminator: "acrobat")]
    [Serializable]
    [XmlInclude(typeof(Student))]
    [XmlInclude(typeof(TaxiDriver))]
    [XmlInclude(typeof(Acrobat))]
    public class Person : IPerson
    {
        private string lastName;
        private string firstName;

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        [JsonConstructor]
        public Person(string firstName, string lastName)
        {
            this.firstName = firstName;
            this.lastName = lastName;
        }

        public Person() { }

        public override string ToString()
        {
            return "Person - " + FirstName + " " + LastName + ";";
        }
    }

    [Serializable]
    public class Student : Person, IStudy, IDance
    {
        private int course;
        private string studentId;
        private string hometown;
        private string passportNumber;

        public string Dance()
        {
            return FirstName + LastName + " is dancing!";
        }

        public string Study()
        {
            return FirstName + LastName + " is studying!";
        }

        public int Course
        {
            get { return course; }
            set { course = value; }
        }
        public string StudentId
        {
            get { return studentId; }
            set { studentId = value; }
        }
        public string Hometown
        {
            get { return hometown; }
            set { hometown = value; }
        }
        public string PassportNumber
        {
            get { return passportNumber; }
            set { passportNumber = value; }
        }

        [JsonConstructor]
        public Student(string firstName, string lastName, int course, string studentId, string hometown, string passportNumber) : base(firstName, lastName)
        {
            this.course = course;
            this.studentId = studentId;
            this.hometown = hometown;
            this.passportNumber = passportNumber;
        }

        public Student() : base() { }

        public override string ToString()
        {
            return "Student - " + FirstName + " " + LastName + "; Course: " + Course + "; Student Id: " + StudentId + "; Hometown: " + Hometown + "; Passport Number: " + PassportNumber + ";";
        }
    }

    [Serializable]
    public class TaxiDriver : Person, IDrive, IDance
    {
        public TaxiDriver(string firstName, string lastName) : base(firstName, lastName) { }

        public TaxiDriver() : base() { }


        public string Dance()
        {
            return FirstName + LastName + " is dancing!";
        }

        public string Drive()
        {
            return FirstName + LastName + " is driving!";
        }

        public override string ToString()
        {
            return "TaxiDriver - " + FirstName + " " + LastName + ";";
        }
    }

    [Serializable]
    public class Acrobat : Person, IDance, IDoTrics
    {
        public Acrobat(string firstName, string lastName) : base(firstName, lastName) { }

        public Acrobat() : base() { }


        public string Dance()
        {
            return FirstName + LastName + " is dancing!";
        }

        public string DoTrics()
        {
            return FirstName + LastName + " is dancing!";
        }

        public override string ToString()
        {
            return "Acrobat - " + FirstName + " " + LastName + ";";
        }
    }
}