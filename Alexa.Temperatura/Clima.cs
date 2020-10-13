using System;
using System.Collections.Generic;
using System.Text;

namespace Alexa.Temperatura
{
    public class Clima
    {
        public DataResult data { get; set; }
    }

    public class DataResult
    {
        public int temperature { get; set; }
    }
}   
