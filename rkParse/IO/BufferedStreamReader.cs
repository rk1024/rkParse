using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.IO {
  class Dummy : TextReader {

  }

  public class BufferedStreamReader : IDisposable {
    private StreamReader reader;
    private StringBuilder buffer;

    public Stream BaseStream => reader.BaseStream;

    public BufferedStreamReader(Stream stream) {
      reader = new StreamReader(stream);
      buffer = new StringBuilder();
    }

    public void Dispose() {
      reader.Dispose();
    }

    int Buffer(int count) {
      if (count == 1) return Buffer();

      if (buffer.Length < count) {
        char[] buf = new char[count - buffer.Length];

        int length = reader.ReadBlock(buf, 0, buf.Length);

        buffer.Append(buf, 0, length);

        count = buffer.Length;
      }

      return count;
    }

    int Buffer() {
      if (buffer.Length == 0) {
        char ch = (char)reader.Read();

        if (ch == 0) return 0;

        buffer.Append(ch);
      }

      return 1;
    }

    public int Flush(int count = 1) {
      int length = Buffer(count);

      buffer.Remove(0, length);

      return length;
    }

    public int FlushAll() { return Flush(buffer.Length); }

    public int Peek(out string dest, int count) {
      if (count == 1) return Peek(out dest);

      int length = Buffer(count);

      char[] buf = new char[length];
      buffer.CopyTo(0, buf, 0, length);
      dest = new string(buf);

      return length;
    }

    public int Peek(out string dest) {
      int length = Buffer();

      if (length == 0) dest = "";
      else dest = char.ToString(buffer[0]);

      return length;
    }

    public int PeekAhead(out string dest, int start, int count) {
      int length = Buffer(start + count);
      length = Math.Max(0, length - start);

      char[] buf = new char[length];
      buffer.CopyTo(start, buf, 0, length);
      dest = new string(buf);

      return length;
    }

    public int Read(out string dest, int count) {
      int length = Peek(out dest, count);

      buffer.Remove(0, length);

      return length;
    }

    public int Read(out string dest) {
      int length = Peek(out dest);

      buffer.Remove(0, length);

      return length;
    }
  }
}
