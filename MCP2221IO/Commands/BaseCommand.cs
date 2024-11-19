using MCP2221IO.Exceptions;
using System;
using System.IO;

namespace MCP2221IO.Commands
{
    /// <summary>
    /// A base command
    /// </summary>
    internal abstract class BaseCommand : ICommand
    {
        protected BaseCommand(CommandCodes commandCode)
        {
            CommandCode = commandCode;
        }

        // <inheritdoc/>
        public CommandCodes CommandCode { get; }

        // <inheritdoc/>
        public virtual void Serialize(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (stream.Length != 64)
            {
                throw new InvalidStreamLengthException($"Unexpected stream length Expected: [0x40] Actual [{stream.Length}]");
            }

            stream.Position = 0;
            stream.WriteByte(0);
            stream.WriteByte((byte)CommandCode);
        }
    }
}
