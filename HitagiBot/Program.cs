using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HitagiBot.Commands;
using HitagiBot.Utilities;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace HitagiBot
{
    public static class Program
    {
        public static readonly IConfigurationRoot Config = InitializeConfiguration();
        private static ArgumentHandler<Message> _messageHandler;

        private static void Main()
        {
            Console.Title = nameof(HitagiBot);

            var botHandle = new TelegramBotClient(Config["Tokens:Telegram"]);
            string botUsername = null;

            try
            {
                botUsername = botHandle.GetMeAsync().Result.Username;
            }
            catch (AggregateException e)
            {
                Console.WriteLine($"An error occurred while retrieving the bot's info: {e.Message}");
                Console.WriteLine("Press any key to terminate...");
                Console.ReadKey();
                Environment.Exit(1);
            }

            _messageHandler = new ArgumentHandler<Message>(botUsername);
            _messageHandler.Add("/start", BaseCommands.Start);
            _messageHandler.Add("/help", BaseCommands.Help);
            _messageHandler.Add("/settings", BaseCommands.Settings);
            _messageHandler.Add("/info", BaseCommands.Info);
            _messageHandler.Add("/lastplayed", LastFmCommands.LastPlayed);
            _messageHandler.Add("/setlastfm", LastFmCommands.SetLastFm);
            _messageHandler.Add("/weather", WeatherCommands.WeatherCommand);
            _messageHandler.Add("/forecast", WeatherCommands.Forecast);
            _messageHandler.Add("/geocode", GeocodeCommands.Geocode);
            _messageHandler.Add("/setlocation", GeocodeCommands.SetLocation);
            _messageHandler.Add("/movie", MovieCommands.Movie);
            _messageHandler.Add("/tvshow", TvCommands.TvShow);
            _messageHandler.Add("/ping", MiscCommands.Ping);
            _messageHandler.Add("/userid", MiscCommands.UserId);

            botHandle.OnMessage += Bot_OnMessage;

            botHandle.StartReceiving();

            while (botHandle.IsReceiving)
                Thread.Sleep(TimeSpan.FromSeconds(10));

            Environment.Exit(0);
        }

        private static IConfigurationRoot InitializeConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", false, true).Build();
        }

        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var botHandle = (TelegramBotClient) sender;

            if ((DateTime.Now - message.Date).TotalMinutes > 1)
                return;

            if (!string.IsNullOrWhiteSpace(message.Text))
                _messageHandler.Run(botHandle, message, message.Text).FireAndForget(botHandle, message);
        }

        private static async void FireAndForget(this Task task, TelegramBotClient botHandle, Message message)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                var errorText =
                    $"An exception was encountered: {e.GetType().Name} - {e.Message}{Environment.NewLine}{e.StackTrace}";
                Console.WriteLine(errorText);

                using (var errorStream = new MemoryStream(Encoding.UTF8.GetBytes(errorText)))
                {
                    var oops = "Oops, something went wrong ﾍ(;´o｀)ﾍ";
                    var errorFile = new FileToSend("Error.txt", errorStream);

                    await botHandle.SendSmartTextMessageAsync(message, oops);
                    await botHandle.SendDocumentAsync(Config["Maintainer"], errorFile);
                }
            }
        }
    }
}