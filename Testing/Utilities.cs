using System;
using System.IO;
using System.Text;

namespace CauldronTests.Utilities
{
    /// <summary>
    /// Class <c>PrefixStringWriter</c> implements a <see cref="TextWriter"/> that writes to 
    /// a string buffer and allows the resulting sequence of characters to be presented as a string.
    /// It inserts a designated prefix string before the write. Largely based on the .NET 
    /// <see cref="StringWriter"/> implementation.
    /// </summary>
    public class PrefixStringWriter : TextWriter
    {
        private Encoding _encoding;
        private readonly StringBuilder _sb;
        private bool _isOpen;

        /// <summary>
        /// The Encoding in which the output is written.
        /// </summary>
        public override Encoding Encoding => _encoding;

        /// <summary>
        /// The prefix string to append to all writes
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// This constructor initializes the new <c>PrefixStringWriter</c>
        /// A new StringBuilder is automatically created and associated 
        /// with the new <c>PrefixStringWriter</c>.
        /// </summary>
        /// <param name="encoding">The character encoding in which the output is written.</param>
        /// <param name="prefix">The prefix string to append to all writes</param>
        public PrefixStringWriter(Encoding encoding, string prefix="") 
        {
            _encoding = encoding;
            Prefix = prefix;
            _sb = new StringBuilder();
            _isOpen = true;
        }

        /// <summary>
        /// Closes the current <c>PrefixStringWriter</c> and the underlying stream.
        /// </summary>
        public override void Close()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <c>PrefixStringWriter</c> and 
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        /// <remarks>
        /// When the disposing parameter is true, this method releases all resources held
        /// by any managed objects that this <c>PrefixStringWriter</c> references. This 
        /// method invokes the Dispose method of each referenced object.
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            // Do not destroy _sb, so that we can extract this after we are
            // done writing (similar to MemoryStream's GetBuffer & ToArray methods)
            _isOpen = false;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns the underlying <c>StringBuilder</c>.
        /// </summary>
        /// <returns>The underlying <c>StringBuilder</c>.</returns>
        public StringBuilder GetStringBuilder() => _sb;

        /// <summary>
        /// Returns a string containing the characters written to the current 
        /// <c>PrefixStringWriter</c> so far.
        /// </summary>
        /// <returns>The string containing the characters written to the current <c>PrefixStringWriter</c>.</returns>
        public override string ToString() => _sb.ToString();

        /**
         * For now, only implement Write and WriteLine for strings
         * If needed to have other implementations (such as char,
         * StringBuilder, char[], etc), that can be a future
         * implementation detail. Also, no async methods have been
         * implemented at this time.
         */

        /// <summary>
        /// Writes a string to the current string, prefixed by <paramref name="Prefix"/>
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <exception cref="ObjectDisposedException">The writer is closed.</exception>
        public override void Write(string value)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(nameof(PrefixStringWriter), "Writer is closed");
            }

            if (value != null)
            {
                _sb.Append($"{Prefix}{value}");
            }
        }

        /// <summary>
        /// Writes a string to the current string, prefixed by <paramref name="Prefix"/>
        /// and followed by a line terminator.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <exception cref="ObjectDisposedException">The writer is closed.</exception>
        public override void WriteLine(string? value)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(nameof(PrefixStringWriter), "Writer is closed");
            }

            if (value != null)
            {
                _sb.AppendLine($"{Prefix}{value}");
            }
        }
    }
}
