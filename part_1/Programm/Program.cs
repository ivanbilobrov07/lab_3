using ClassLibrary1;
using Serialization;

namespace Programm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string FILE_PATH = AppDomain.CurrentDomain.BaseDirectory + "books";
            Book[] books = {
                new Book(1, "Book 1", 1999, 500, 3000),
                new Book(2, "Book 2", 2002, 550, 2500),
                new Book(3, "Book 3", 2012, 635, 5000),
                new Book(4, "Book 4", 2023, 1020, 6000),
            };

            //BinarySerialization<Book>.Write(new Book(0, "Book 0", 2023, 1020, 6000), FILE_PATH);

            BinarySerialization<Book>.Write(books, FILE_PATH);
            JSONSerialization<Book>.Write(books, FILE_PATH);
            XMLSerialization<Book>.Write(books, FILE_PATH);
            CustomSerialization<Book>.Write(books, FILE_PATH);

            Book[] binarySerBooks = BinarySerialization<Book>.Read(FILE_PATH);
            Book[] jsonSerBooks = JSONSerialization<Book>.Read(FILE_PATH);
            Book[] xmlSerBooks = XMLSerialization<Book>.Read(FILE_PATH);
            Book[] customSerBooks = (CustomSerialization<Book>.Read(FILE_PATH)).ToArray();
        }
    }
}