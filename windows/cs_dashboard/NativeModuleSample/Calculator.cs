using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ReactNative.Managed;


namespace cs_dashboard.NativeModuleSample
{
    [ReactModule]
    class Calculator
    {

        [ReactMethod("add")]
        public double Add(double a, double b)
        {
            return a + b;
        }
    }
}
