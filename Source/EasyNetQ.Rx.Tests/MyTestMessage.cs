using EasyNetQ.Tests;

namespace EasyNetQ.Rx.Tests
{
    public class MyTestMessage : MyMessage
    {
        public int Value;

        public MyTestMessage(int value)
        {
            Value = value;
        }
    }
}
