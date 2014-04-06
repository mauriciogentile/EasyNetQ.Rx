using EasyNetQ.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQ.Rx.Tests
{
    public class MyTestMessage : MyMessage
    {
        public int Value { get; set; }
    }
}
