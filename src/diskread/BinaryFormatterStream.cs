namespace Vurdalakov
{
    using System;
    using System.IO;
    using System.Text;

    public class BinaryFormatterStream : Stream
    {
        private Stream _stream;

        public BinaryFormatterStream(Stream stream)
        {
            _stream = stream;
        }

        public override bool CanRead { get { return false; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return true; } }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var stringBuilder = new StringBuilder(128);
            var chars = new Char[16];

            for (var pos = offset; pos < offset + count; )
            {
                stringBuilder.AppendFormat("{0:X08}  ", pos);

                for (var i = 0; i < 16; i++)
                {
                    stringBuilder.AppendFormat("{0:X02} ", buffer[pos]);
                    chars[i] = buffer[pos] > 31 ? (char)buffer[pos] : ' ';
                    pos++;
                }

                stringBuilder.Append(chars);
                stringBuilder.Append(Environment.NewLine);

                if (0 == (pos % 512))
                {
                    stringBuilder.Append(Environment.NewLine);
                }

                var bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
                _stream.Write(bytes, 0, bytes.Length);
            }
        }

        public override long Length { get { throw new NotSupportedException(); } }
        public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }
        public override int Read(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
        public override void SetLength(long value) { throw new NotSupportedException(); }
    }
}
