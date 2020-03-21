using DSharpPlus.CommandsNext;
using System;
using System.Threading.Tasks;

namespace KrisMecn.Commands
{
    public class BaseKrisCommandModule : BaseCommandModule
    {
        public override Task BeforeExecutionAsync(CommandContext ctx)
        {
            try
            {
                ctx.Message.DeleteAsync();
            } catch(Exception e)
            {
                Logger.Error(e);
            }

            return base.BeforeExecutionAsync(ctx);
        }
    }
}
