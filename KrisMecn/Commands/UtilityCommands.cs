using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using KrisMecn.Attributes;
using KrisMecn.Helpers.Extensions;

namespace KrisMecn.Commands
{
    public class UtilityCommands : BaseKrisCommandModule
    {
        private const string UPTIME_TIMESTAMP_FORMAT = @"dd'd 'hh'h 'mm'm 'ss's'";

        private const int MESSAGE_SIZE_LIMIT = 2000;
        private const int HELP_CACHE_TTL = 10000;

        private List<string> _helpCache;
        private long _helpCacheExpire = Environment.TickCount64;

        [
            Hidden,
            Command("uptime"),
            Description("Sends you the uptime of the bot")
        ]
        public async Task Uptime(CommandContext ctx)
        {
            var startTimes = ctx.Client.GetBotContext().BotInstance.StartTimes;

            // create time strings
            var startTimeStr = (DateTime.Now - startTimes.BotStart).ToString(UPTIME_TIMESTAMP_FORMAT);
            var socketStartTimeStr = (DateTime.Now - startTimes.SocketStart).ToString(UPTIME_TIMESTAMP_FORMAT);

            await ctx.ReplyToDM($"**Uptime**: {startTimeStr}\n**Current connection uptime**: {socketStartTimeStr}");
        }

        [
            Command("help"),
            Aliases("h"),
            Description("Sends you a list of all available commands")
        ]
        public async Task Help(CommandContext ctx)
        {
            if (_helpCache == null || _helpCacheExpire <= Environment.TickCount64)
            {
                string helpPrefix = ctx.Client.GetBotContext().BotInstance.Config.HelpPrefix;

                _helpCache = GenerateHelp(ctx.Prefix, ctx.CommandsNext.RegisteredCommands.Values, helpPrefix);
                _helpCacheExpire = Environment.TickCount64 + HELP_CACHE_TTL;
            }

            // send help in parts
            // this is to overcome the 2000 character limit
            foreach (var helpPart in _helpCache)
            {
                await ctx.ReplyToDM(helpPart);
            }
        }

        [
            Command("invite"),
            Aliases("inv", "inviteLink"),
            Description("Sends you the bot's invite link")
        ]
        public async Task Invite(CommandContext ctx)
        {
            string inviteLink = ctx.Client.GetBotContext().BotInstance.Config.InviteLink;

            await ctx.ReplyToDM($"Invite link: {inviteLink}");
        }

        private List<string> GenerateHelp(string commandPrefix, IEnumerable<Command> commands, string helpPrefix = "")
        {
            var moduleCmdHelp = new Dictionary<string, List<string>>();

            var targetCommands = commands.Distinct().ToList();
            for(int x = 0; x < targetCommands.Count; x++) {
                var cmd = targetCommands[x];

                // ignore hidden commands
                if (cmd.IsHidden) continue;

                // convert command groups into individual commands
                if (cmd is CommandGroup)
                {
                    var cmdGroup = cmd as CommandGroup;
                    targetCommands.AddRange(cmdGroup.Children);
                    continue;
                }

                List<string> helpStrings = new List<string>();
                var sb = new StringBuilder();

                var commandGroupPrefix = GetCommandGroupPrefix(cmd);

                // add command overloads
                for (int i = 0; i < cmd.Overloads.Count; i++)
                {
                    var ol = cmd.Overloads[i];

                    sb.Append("> **")
                      .Append(commandPrefix)
                      .Append(commandGroupPrefix)
                      .Append(cmd.Name);

                    foreach (var arg in ol.Arguments)
                    {
                        sb.Append(" <")
                          .Append(arg.Name);

                        if (arg.IsOptional) sb.Append("?");

                        sb.Append(">");
                    }

                    sb.Append("**");

                    // add newline only if it's the last cycle
                    if (i < cmd.Overloads.Count - 1) sb.Append("\n");
                }

                // add description
                if (!string.IsNullOrEmpty(cmd.Description))
                {
                    sb.Append("\n*Description:* ")
                      .Append(cmd.Description);
                }

                // add alisases
                if (cmd.Aliases.Count > 0)
                {
                    sb.Append("\n*Aliases:* ");

                    int lastIndex = cmd.Aliases.Count - 1;
                    // append all aliases except the last one
                    for (int i = 0; i < lastIndex; i++)
                    {
                        sb
                          .Append(commandPrefix)
                          .Append(commandGroupPrefix)
                          .Append(cmd.Aliases[i])
                          .Append(", ");
                    }

                    // append last
                    sb.Append(commandPrefix).Append(commandGroupPrefix).Append(cmd.Aliases[lastIndex]);
                }

                helpStrings.Add(sb.ToString());

                var moduleType = cmd.Module.ModuleType;

                var displayNameAttribute = moduleType.GetTypeInfo().GetCustomAttribute<ModuleDisplayNameAttribute>();
                string moduleName = displayNameAttribute?.DisplayName ?? moduleType.Name;

                if (moduleCmdHelp.ContainsKey(moduleName)) moduleCmdHelp[moduleName].AddRange(helpStrings);
                else moduleCmdHelp.Add(moduleName, helpStrings);
            }

            // generate final help string
            var ret = new List<string>();

            var hb = new StringBuilder(MESSAGE_SIZE_LIMIT, MESSAGE_SIZE_LIMIT);

            // add prefix
            hb.Append(helpPrefix);

            foreach (var moduleName in moduleCmdHelp.Keys)
            {
                var readableName = Regex.Replace(moduleName, "((?<!^)[A-Z])", " $1").ToUpper();

                hb.AppendFormat("__**{0}**__\n\n", readableName);
                foreach (var cmdHelp in moduleCmdHelp[moduleName])
                {
                    if (hb.Length + cmdHelp.Length + 2 /* newline size */ > hb.MaxCapacity)
                    {
                        // finish string builder and start creating a new part
                        ret.Add(hb.ToString());
                        hb = new StringBuilder(MESSAGE_SIZE_LIMIT, MESSAGE_SIZE_LIMIT);
                    }

                    hb.Append(cmdHelp).Append("\n");
                }

                hb.Append("\n");
            }

            // add final part
            ret.Add(hb.ToString());

            return ret;
        }

        private string GetCommandGroupPrefix(Command command)
        {
            if (command.Parent == null) return "";

            var parent = command.Parent;
            var parts = new List<string>();
            while (parent != null)
            {
                parts.Add(parent.Name);
                parent = parent.Parent;
            }

            parts.Reverse();

            return string.Join(" ", parts) + " ";
        }
    }
}
