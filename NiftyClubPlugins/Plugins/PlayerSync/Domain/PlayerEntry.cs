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

        internal float RotationX { get; set; }
        internal float RotationY { get; set; }
        internal float RotationZ { get; set; }
        internal float RotationW { get; set; }

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

            RotationX = player.Rotation.X;
            RotationY = player.Rotation.Y;
            RotationZ = player.Rotation.Z;
            RotationW = player.Rotation.W;

            Id = player.ID;
        }

        public void Deserialize (DeserializeEvent e)
        {
            PositionX = e.Reader.ReadSingle ();
            PositionY = e.Reader.ReadSingle ();
            PositionZ = e.Reader.ReadSingle ();

            RotationX = e.Reader.ReadSingle ();
            RotationY = e.Reader.ReadSingle ();
            RotationZ = e.Reader.ReadSingle ();
            RotationW = e.Reader.ReadSingle ();
        }

        public void Serialize (SerializeEvent e)
        {
            e.Writer.Write (PositionX);
            e.Writer.Write (PositionY);
            e.Writer.Write (PositionZ);

            e.Writer.Write (RotationX);
            e.Writer.Write (RotationY);
            e.Writer.Write (RotationZ);
            e.Writer.Write (RotationW);

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
