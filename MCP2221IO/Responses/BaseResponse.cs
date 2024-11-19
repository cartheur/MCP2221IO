using MCP2221IO.Commands;
using MCP2221IO.Exceptions;
using System;
using System.IO;

namespace MCP2221IO.Responses
{
    /// <summary>
    /// A base <see cref="IResponse"/> type
    /// </summary>
    internal abstract class BaseResponse : IResponse
    {
        protected BaseResponse(CommandCodes commandCode)
        {
            CommandCode = commandCode;
        }

        // <inheritdoc/>
        public CommandCodes CommandCode { get; }

        public byte ExecutionResult { get; private set; }

        // <inheritdoc/>
        public virtual void Deserialize(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (stream.Length != 65)
            {
                throw new InvalidStreamLengthException($"Unexpected stream length Expected: [0x41] Actual [0x{stream.Length:x}]");
            }

            stream.Position = 1;

            byte responseCode = (byte)stream.ReadByte();
            if (responseCode != (byte)CommandCode)
            {
                throw new InvalidResponseTypeException($"Unexpected response code Expected: [0x{(byte)CommandCode:x}] Actual [0x{responseCode:x}]");
            }

            ExecutionResult = (byte)stream.ReadByte();
        }
    }
}
