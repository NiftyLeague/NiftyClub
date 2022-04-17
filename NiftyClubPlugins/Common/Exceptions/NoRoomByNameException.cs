using System;
using System.Collections.Generic;
using System.Text;

namespace NiftyClubPlugins.Common.Exceptions
{
    class NoRoomByNameException : Exception
    {
        public NoRoomByNameException ()
        {
        }

        public NoRoomByNameException (string name) : base (String.Format ("No room with name: {0}", name))
        {
        }
    }
}
