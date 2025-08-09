using NAudio.Wave;

namespace SpaceInvaders.Services
{
    public class LoopStream : WaveStream
    {
        private readonly WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat => sourceStream.WaveFormat;
        public override long Length => sourceStream.Length;

        public override long Position
        {
            get => sourceStream.Position;
            set => sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = sourceStream.Read(buffer, offset, count);
            if (read < count && this.EnableLooping)
            {
                sourceStream.Position = 0;
                int read2 = sourceStream.Read(buffer, offset + read, count - read);
                read += read2;
            }
            return read;
        }
    }
}