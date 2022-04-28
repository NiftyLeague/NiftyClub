using DarkRift;
using System;

namespace NiftyClubPlugins.Plugins.PlayerSync.Domain
{
	class PlayerEntry : IDarkRiftSerializable, IDisposable
	{
        private bool disposedValue;

        internal float PositionX { get; set; }
        internal float PositionY { get; set; }
        internal float PositionZ { get; set; }

        internal ushort Id { get; set; }

        public PlayerEntry ()
        {
            // do nothing
        }

        public PlayerEntry (Player player)
        {
            ReadFromPlayer (player);
        }

        public void ReadFromPlayer (Player player)
        {
            PositionX = player.Position.X;
            PositionY = player.Position.Y;
            PositionZ = player.Position.Z;

            Id = player.ID;
        }

        public void Deserialize (DeserializeEvent e)
        {
            PositionX = e.Reader.ReadSingle ();
            PositionY = e.Reader.ReadSingle ();
            PositionZ = e.Reader.ReadSingle ();
        }

        public void Serialize (SerializeEvent e)
        {
            e.Writer.Write (PositionX);
            e.Writer.Write (PositionY);
            e.Writer.Write (PositionZ);

            e.Writer.Write (Id);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose ()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }
    }
}
