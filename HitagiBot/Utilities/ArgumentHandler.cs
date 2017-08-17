using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;

namespace HitagiBot.Utilities
{
    public class ArgumentHandler<T>
    {
        public delegate Task CommandFunc(TelegramBotClient botHandle, T source, GroupCollection matches);

        private readonly Regex _argumentRegex;

        private readonly Dictionary<string, CommandFunc> _commandStore = new Dictionary<string, CommandFunc>();

        public ArgumentHandler(string botUsername)
        {
            _argumentRegex = new Regex($@"^([^@\s]+)(?:@{botUsername})?(?:(?: |$)(.*))?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public void Add(string commandMatch, CommandFunc commandFunc)
        {
            commandMatch = commandMatch.ToLower();

            if (_commandStore.ContainsKey(commandMatch))
                _commandStore[commandMatch] += commandFunc;
            else
                _commandStore[commandMatch] = commandFunc;
        }

        public async Task Run(TelegramBotClient botHandle, T source, string sourceText)
        {
            var result = _argumentRegex.Match(sourceText);

            if (result.Success)
            {
                var commandMatch = result.Groups[1].Value.ToLower();

                if (_commandStore.ContainsKey(commandMatch))
                    await _commandStore[commandMatch](botHandle, source, result.Groups);
            }
        }
    }
}