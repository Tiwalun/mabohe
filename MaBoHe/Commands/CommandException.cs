using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaBoHe.Commands
{
    class CommandException : Exception
    {
        public CommandException()
        {

        }

        public CommandException(string msg) : base(msg)
        {
           
        }
    }
}
