using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HitagiBot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HitagiBot.Commands
{
    public static class BaseCommands
    {
        public static async Task Start(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            await botHandle.SendSmartTextMessageAsync(source, "Hi, I'm Hitagi!");
        }

        public static async Task Help(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            await botHandle.SendSmartTextMessageAsync(source, "Coming soon!");
        }

        public static async Task Settings(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            await botHandle.SendSmartTextMessageAsync(source, "Coming soon!");
        }

        public static async Task Info(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            await botHandle.SendSmartTextMessageAsync(source, "Coming soon!");
        }
    }
}