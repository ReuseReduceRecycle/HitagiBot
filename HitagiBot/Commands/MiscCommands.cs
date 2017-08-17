using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HitagiBot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HitagiBot.Commands
{
    public static class MiscCommands
    {
        public static async Task Ping(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            await botHandle.SendSmartTextMessageAsync(source, "Pong");
        }

        public static async Task UserId(TelegramBotClient botHandle, Message source, GroupCollection matches)
        {
            await botHandle.SendSmartTextMessageAsync(source, "Coming soon!");
        }
    }
}