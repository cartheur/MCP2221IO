using HidSharp;
using Microsoft.Extensions.Logging;
using System;

namespace MCP2221IO.Usb
{
    public class HidSharpHidDevice : IHidDevice
    {
        private readonly ILogger<IHidDevice> _logger;
        private HidDevice _hidDevice;
        private HidStream _hidStream;
        private bool disposedValue;

        public HidSharpHidDevice(ILogger<IHidDevice> logger, HidDevice hidDevice)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hidDevice = hidDevice ?? throw new ArgumentNullException(nameof(hidDevice));
        }

        public void Open()
        {
            _hidStream = _hidDevice.Open();
        }

        public void Write(byte[] outBytes)
        {
            _hidStream.Write(outBytes);
        }

        public byte[] WriteRead(byte[] outBytes)
        {
            _logger.LogDebug($"Output HID Packet: [{BitConverter.ToString(outBytes).Replace("-", ",0x")}]");

            _hidStream.Write(outBytes);

            var inBytes = _hidStream.Read();

            _logger.LogDebug($"Input HID Packet: [{BitConverter.ToString(inBytes).Replace("-", ",0x")}]");

            return inBytes;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_hidStream != null)
                    {
                        _hidStream.Dispose();
                    }

                    _hidDevice = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
