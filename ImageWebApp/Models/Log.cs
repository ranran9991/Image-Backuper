using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infrastructure;

namespace ImageWebApp.Models
{
    public class Log
    {
        public List<MessageRecievedEventArgs> log;

        public Log()
        {
            log = new List<MessageRecievedEventArgs>();
        }
    }
}