namespace AIGraphics
{
    internal class SSSManager
    {
        private readonly AIGraphics _parent;

        internal SSSManager(AIGraphics parent)
        {
            _parent = parent;
        }

        internal bool Enabled { get; set; }
        internal float Downsampling { get; set; }

        internal float ScatteringRadius { get; set; }

        internal int ScatteringIterations { get; set; }

        internal int ShaderIterations { get; set; }

        internal bool Dithered { get; set; }

        internal float DitherIntensity { get; set; }

        internal float DitherScale { get; set; }
    }
}