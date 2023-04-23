using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestParser
{
    public class Percent
    {
        public double Persent(double fullPrice, double balance)
        {
            double percent = (balance / 1.0000000000000) * 100;
            double result = percent*fullPrice;
            return result;
        }


    }
}
