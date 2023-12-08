using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Helpers
    {
        public static bool isValidIndex(int index, int length)
        {
            return index < length && index >= 0;
        }
    }
}
