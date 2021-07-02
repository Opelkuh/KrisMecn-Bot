using DSharpPlus.CommandsNext;
using KrisMecn.Helpers.Extensions;
using System;
using System.Text;
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
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            var sb = new StringBuilder();

            // add basic info
            sb.AppendFormat(
                "`{0}` @ `{1}` requested `{2}`",
                ctx.User.GetFullName(),
                ctx.Guild?.Name ?? "<DMs>",
                ctx.Command.Name
            );

            // add arguments if present
            if (!string.IsNullOrEmpty(ctx.RawArgumentString))
            {
                sb.AppendFormat(
                    " with `{0}`",
                    ctx.RawArgumentString
                );
            }

            Logger.Info("Command", sb.ToString());

            return base.BeforeExecutionAsync(ctx);
        }
    }
}
