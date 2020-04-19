using System;
using System.Collections.Generic;
using System.Text;

namespace KrisMecn.Entities
{
    class StartTimes
    {
        /// <summary>
        /// Time when the application was started
        /// </summary>
        public DateTime BotStart = DateTime.Now;

        /// <summary>
        /// Time when the application last connected
        /// </summary>
        public DateTime SocketStart = DateTime.Now;
    }
}
