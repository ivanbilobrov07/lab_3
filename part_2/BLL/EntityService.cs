using System;
using System.Diagnostics;
using DAL;
using static System.Reflection.Metadata.BlobBuilder;

namespace BLL
{
    public class EntityService
    {

        private EntityContext<Person> db;
        protected SerializationType type;
        protected string fileName;
        protected List<Person> entityList;

        public List<Person> EntityList
        {
            get { return entityList; }
        }

        public EntityService(string type, string fileName)
        {
            if (type.ToUpper() == "JSON")
            {
                this.type = SerializationType.JSON;
            }
            else if (type.ToUpper() == "BINARY")
            {
                this.type = SerializationType.Binary;
            }
            else if (type.ToUpper() == "XML")
            {
                this.type = SerializationType.XML;
            }
            else if (type.ToUpper() == "CUSTOM")
            {
                this.type = SerializationType.Custom;
            }
            else
            {
                throw new InvalidSerializationTypeException(type);
            }

            this.fileName = fileName;
            db = new EntityContext<Person>(this.type, fileName);
            entityList = db.Read();
        }

        public string AddStudent(string firstName, string lastName, int course, string studentId, string hometown, string passportNumber)
        {
            Student newStudent = new Student(firstName, lastName, course, studentId, hometown, passportNumber);
            entityList.Add(newStudent);

            db.Write(entityList);

            return newStudent.ToString();
        }

        public string AddAcrobat(string firstName, string lastName)
        {
            Acrobat newAcrobat = new Acrobat(firstName, lastName);
            entityList.Add(newAcrobat);

            db.Write(entityList);

            return newAcrobat.ToString();
        }

        public string AddTaxiDriver(string firstName, string lastName)
        {
            TaxiDriver newTaxiDriver = new TaxiDriver(firstName, lastName);
            entityList.Add(newTaxiDriver);

            db.Write(entityList);

            return newTaxiDriver.ToString();
        }

        public string Remove(int index)
        {
            Person person = entityList[index];
            entityList.RemoveAt(index);

            db.Rewrite(entityList);

            return person.ToString();
        }

        public float CalculatePercentage()
        {
            int students = 0;
            float searchedStudents = 0;

            foreach (Person pers in entityList)
            {
                if (pers.GetType() == typeof(Student))
                {
                    Student student = (Student)pers;

                    students++;
                    if (student.Course == 1 && student.Hometown.ToLower() != "kyiv")
                    {
                        searchedStudents++;
                    }
                }
            }

            return searchedStudents / students * 100;
        }

        public void Print()
        {
            if (entityList.Count == 0)
            {
                Console.WriteLine("The list is empty");
                return;
            }

            int index = 0;

            foreach (Person entity in entityList)
            {
                Console.WriteLine(index + 1 + ") " + entity.ToString());
                index++;
            }
        }
    }
}