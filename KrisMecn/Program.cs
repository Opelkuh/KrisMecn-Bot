namespace KrisMecn
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();

            bot.StartAsync().GetAwaiter().GetResult();
        }
    }
}
