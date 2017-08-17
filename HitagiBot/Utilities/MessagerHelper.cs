using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HitagiBot.Utilities
{
    public static class MessagerHelper
    {
        public static string InsertInvisibleLink(string link, ParseMode parseMode)
        {
            return parseMode == ParseMode.Html ? $"<a href=\"{link}\">\u2063</a>" : $"[\u2063]({link})";
        }

        public static async Task<Message> SendSmartTextMessageAsync(this TelegramBotClient botClient, Message source,
            string text, ParseMode parseMode = ParseMode.Default)
        {
            var replyToMessageId = source.Chat.Type == ChatType.Private ? 0 : source.MessageId;

            return await botClient.SendTextMessageAsync(source.Chat.Id, text, parseMode,
                replyToMessageId: replyToMessageId);
        }
    }
}