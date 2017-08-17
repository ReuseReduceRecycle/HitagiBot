using System;
using System.Net.Http;
using System.Threading;
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
            Message resultMessage = null;
            var replyToMessageId = source.Chat.Type == ChatType.Private ? 0 : source.MessageId;

            for (var i = 0; i < 3; ++i)
                try
                {
                    resultMessage = await botClient.SendTextMessageAsync(source.Chat.Id, text, parseMode,
                        replyToMessageId: replyToMessageId);
                    break;
                }
                catch (HttpRequestException)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1 * i));
                }

            return resultMessage;
        }
    }
}