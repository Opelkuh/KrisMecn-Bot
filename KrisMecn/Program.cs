namespace KrisMecn
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Info("Starting...");

            var bot = new Bot();

            bot.StartAsync().GetAwaiter().GetResult();
        }
    }
}
