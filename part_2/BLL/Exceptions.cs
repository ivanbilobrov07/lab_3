using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }

    public class InvalidSerializationTypeException : Exception
    {
        public InvalidSerializationTypeException(string type)
            : base($"Invalid serialization type: {type}. Supported types are JSON, BINARY, and XML.")
        {
        }
    }

    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() : base("Entity not found.")
        {
        }

        public EntityNotFoundException(string message) : base(message)
        {
        }
    }

    public class DuplicateStudentException : Exception
    {
        public DuplicateStudentException(string studentId) : base($"The student with student id '{studentId}' is already stored in the list.")
        {
        }
    }
}
