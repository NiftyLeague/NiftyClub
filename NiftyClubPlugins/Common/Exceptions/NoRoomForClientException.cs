using System;

namespace NiftyClubPlugins.Common.Exceptions
{
    class NoRoomForClientException : Exception
    {
        public NoRoomForClientException ()
        {
        }

        public NoRoomForClientException (string name) : base (String.Format ("No room for client: {0}", name))
        {

        }
    }
}
