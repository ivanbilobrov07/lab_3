using System.Xml.Serialization;

namespace ClassLibrary1
{
    [Serializable]
    public class Book
    {
        private int serialNumber;
        private string name;
        private int yearOfPublishing;
        private int cost;
        private int numOfCopies;

        public int SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int YearOfPublishing
        {
            get { return yearOfPublishing; }
            set { yearOfPublishing = value; }
        }

        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }
        public int NumOfCopies
        {
            get { return numOfCopies; }
            set { numOfCopies = value; }
        }

        public int GetTotalCostOfCirculation()
        {
           return cost * numOfCopies; 
        }

        public Book(int serialNumber, string name, int yearOfPublishing, int cost, int numOfCopies)
        {
            this.serialNumber = serialNumber;
            this.name = name;
            this.yearOfPublishing = yearOfPublishing;
            this.cost = cost;
            this.numOfCopies = numOfCopies;
        }

        public Book()
        {
           
        }

        public void increaseCostByPercentage(int percentage)
        {
            cost += cost * percentage;
        }

        public override string ToString()
        {
            return $"Book \"{name}\", serial number - \"{serialNumber}\", year of publishing - {yearOfPublishing}, price - {cost}, number of copies - {numOfCopies}";
        }

    }
}