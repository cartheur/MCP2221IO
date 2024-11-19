using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MCP2221IO.Commands
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "<Pending>")]
    internal class CancelI2cBusTransferCommand : BaseCommand
    {
        public CancelI2cBusTransferCommand() : base(CommandCodes.StatusSetParameters)
        {
        }

        public override void Serialize(Stream stream)
        {
            base.Serialize(stream);

            stream.WriteByte(0);    // Don't care
            stream.WriteByte(0x10); // Cancel current I2C/SMBus transfer(sub - command)
            stream.WriteByte(0xFF); // Set I2C/SMBus communication speed (sub-command) no change
        }
    }
}
