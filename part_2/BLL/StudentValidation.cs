using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BLL
{
    public class StudentValidation
    {
        static public bool isValidName(string name)
        {
            if (name.Length >= 2)
            {
                return true;
            }

            throw new ValidationException("Invalid name. It must be at least 2 characters long.");
        }

        static public bool isValidStudentId(string id)
        {
            string pattern = @"^\d{8}$";
            bool isMatch = Regex.IsMatch(id, pattern);

            if (isMatch)
            {
                return true;
            }

            throw new ValidationException("Invalid student ID. It must be a 7-digit number.");
        }

        static public bool isValidCourse(int course)
        {
            if (course >= 1 && course <= 6)
            {
                return true;
            }

            throw new ValidationException("Invalid course. It must be between 1 and 6.");
        }

        static public bool isValidHometown(string hometown)
        {
            if (hometown.Length >= 2)
            {
                return true;
            }

            throw new ValidationException("Invalid hometown. It must be at least 2 characters long.");
        }

        static public bool isValidPassportNumber(string value)
        {
            string pattern = @"^\d{9}$";
            bool isMatch = Regex.IsMatch(value, pattern);

            if (isMatch)
            {
                return true;
            }

            throw new ValidationException("Invalid passport number. It must be a 7-digit number.");
        }
    }
}
