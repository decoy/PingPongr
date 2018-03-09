using Microsoft.Extensions.Logging;
using System;

namespace Examples.Complex.Services
{
    /// <summary>
    /// A silly service for generating greetings
    /// </summary>
    public class GreetingService
    {
        private static string[] Greetings = new string[] {
            "'Allo",
            "'Allo 'Allo",
            "Aye oop",
            "Ay up",
            "Ahoy",
            "G'day",
            "Greetings",
            "Hello",
            "Hey there",
            "Hey",
            "Hi there",
            "Hi",
            "Hiya",
            "How are things",
            "How are ya",
            "How ya doin'",
            "How's it goin'",
            "How's it going",
            "How's life",
            "Howdy",
            "Sup",
            "What's new",
            "What's up",
            "Yo"
        };

        private Random random = new Random();

        private readonly ILogger<GreetingService> log;

        public GreetingService(ILogger<GreetingService> log)
        {
            this.log = log;
            log.LogInformation("Greeting service initialized!");
        }

        public string GetRandomGreeting()
        {
            var i = random.Next(0, Greetings.Length);
            log.LogInformation("Getting greeting {index}", i);
            return Greetings[i];
        }
    }
}
