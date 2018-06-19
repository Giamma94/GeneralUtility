using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralUtility.NumberUtility
{
    class Verify
    {

        /// <summary>
        /// Ceck if the scring is a numeric value with a specific limit
        /// </summary>
        /// <param name="str">Input String</param>
        /// <param name="minLimit">Limit minimum</param>
        /// <param name="maxLimit">Limit maximum</param>
        /// <returns></returns>
        public static bool IsValid(string str, int minLimit, int maxLimit)
        {
            return int.TryParse(str, out var i) && i >= minLimit && i <= maxLimit;
        }
    }
}
