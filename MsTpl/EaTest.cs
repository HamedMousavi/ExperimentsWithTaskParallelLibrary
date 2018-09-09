using System;


namespace MsTpl
{
    public class EaTest
    {

        private EventAggeragtor _ea;
        private Subscriber _sub;


        public void RunTests()
        {
            _sub = new Subscriber();
            _ea = new EventAggeragtor();

            // Todo: Upon app startup load all subscriptions and register them automatically
            // So that we no longer have _ea.Subscribe plumbing code like the one below
            // All we'll end up writing would be to call Application.Events.Publish(ClassName); HOW COOL IS THAT?!
            _ea.Subscribe(_sub);
            _ea.Publish(new MyEvent("Hi there!"));
            _ea.Publish(new MySecondEvent("How you doin'?!!"));

            Console.ReadKey();
        }


        private class MyEvent
        {
            public string Greeting { get; }

            public MyEvent(string greeting)
            {
                Greeting = greeting;
            }
        }


        private class MySecondEvent
        {
            public string Greeting { get; }

            public MySecondEvent(string greeting)
            {
                Greeting = greeting;
            }
        }


        private class Subscriber : ISubscriber<MyEvent>, ISubscriber<MySecondEvent>
        {
            public void OnEvent(MyEvent e)
            {
                Console.WriteLine("Received an event:");
                Console.WriteLine(e.Greeting);
                Console.WriteLine("End of event!");
            }

            public void OnEvent(MySecondEvent e)
            {
                Console.WriteLine("Received a 2'nd event:");
                Console.WriteLine(e.Greeting);
                Console.WriteLine("End of event!");
            }
        }
    }
}
