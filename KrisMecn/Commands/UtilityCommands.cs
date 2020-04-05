using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KrisMecn.Commands
{
    public class UtilityCommands : BaseKrisCommandModule
    {
        private const int MESSAGE_SIZE_LIMIT = 2000;
        private const int HELP_CACHE_TTL = 1;

        private List<string> _helpCache;
        private long _helpCacheExpire = Environment.TickCount64;

        [
            Command("help"),
            Aliases("h"),
            Description("Sends you a list of all available commands")
        ]
        public async Task Help(CommandContext ctx)
        {
            int cmdCount = ctx.CommandsNext.RegisteredCommands.Count;
            if (_helpCache == null || _helpCacheExpire <= Environment.TickCount64)
            {
                _helpCache = GenerateHelp(ctx.Prefix, ctx.CommandsNext.RegisteredCommands.Values);
                _helpCacheExpire = Environment.TickCount64 + HELP_CACHE_TTL;
            }

            // send help in parts
            // this is to overcome the 2000 character limit
            foreach(var helpPart in _helpCache)
            {
                if (ctx.Member == null) await ctx.RespondAsync(helpPart); // send reply to DMs
                else await ctx.Member.SendMessageAsync(helpPart);
            }
        }

        private List<string> GenerateHelp(string prefix, IEnumerable<Command> commands)
        {
            var moduleCmdHelp = new Dictionary<string, List<string>>();

            foreach(var cmd in commands.Distinct())
            {
                List<string> helpStrings = new List<string>();
                foreach(var ol in cmd.Overloads)
                {
                    var sb = new StringBuilder(prefix);
                    
                    sb.Append(cmd.Name);
                    
                    foreach(var arg in ol.Arguments)
                    {
                        sb.Append(" <")
                          .Append(arg.Name);

                        if (arg.IsOptional) sb.Append("?");

                        sb.Append(">");
                    }

                    if(!string.IsNullOrEmpty(cmd.Description))
                    {
                        sb.Append(" - ")
                          .Append(cmd.Description);
                    }

                    helpStrings.Add(sb.ToString());
                }

                string moduleName = cmd.Module.ModuleType.Name;
                if (moduleCmdHelp.ContainsKey(moduleName)) moduleCmdHelp[moduleName].AddRange(helpStrings);
                else moduleCmdHelp.Add(moduleName, helpStrings);
            }

            // generate final help string
            var ret = new List<string>();
            var hb = new StringBuilder(MESSAGE_SIZE_LIMIT, MESSAGE_SIZE_LIMIT);
            foreach(var moduleName in moduleCmdHelp.Keys)
            {
                var readableName = Regex.Replace(moduleName, "((?<!^)[A-Z])", " $1").ToUpper();

                hb.AppendFormat("__**{0}**__\n", readableName);
                foreach(var cmdHelp in moduleCmdHelp[moduleName])
                {
                    if(hb.Length + cmdHelp.Length + 2 /* newline size */ > hb.MaxCapacity)
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
    }
}
