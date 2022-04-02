﻿/*
* MIT License
*
* Copyright (c) 2022 Derek Goslin https://github.com/DerekGn
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using MCP2221IO.Commands;
using MCP2221IO.Gpio;
using MCP2221IO.Responses;
using MCP2221IO.Usb;
using System;
using System.IO;

namespace MCP2221IO
{
    public class Device : IDevice
    {
        private long _factorySerialNumber;
        private IUsbDevice _usbDevice;

        public Device(IUsbDevice usbDevice)
        {
            _usbDevice = usbDevice ?? throw new ArgumentNullException(nameof(usbDevice));
        }

        // <inheritdoc/>
        public DeviceStatus Status => ExecuteCommand<StatusSetParametersResponse>(new StatusSetParametersCommand()).DeviceStatus;
        // <inheritdoc/>
        public ChipSettings ChipSettings => ExecuteCommand<ChipSettingsResponse>(new ReadChipSettingsCommand()).ChipSettings;
        // <inheritdoc/>
        public GpioPorts GpioPorts => ExecuteCommand<GpioPortsResponse>(new ReadGpioPortsCommand()).GpioPorts;
        // <inheritdoc/>
        public string UsbManufacturerDescriptor => ExecuteCommand<UsbManufacturerDescriptorResponse>(new ReadUsbManufacturerDescriptorCommand()).Value;
        // <inheritdoc/>
        public string UsbProductDescriptor => ExecuteCommand<UsbProductDescriptorResponse>(new ReadUsbProductDescriptorCommand()).Value;
        // <inheritdoc/>
        public string UsbSerialNumberDescriptor => ExecuteCommand<UsbSerialNumberDescriptorResponse>(new ReadUsbSerialNumberDescriptorCommand()).Value;

        //public int Speed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public CommParameters CommParameters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public ChipSettings ChipSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public GpSettings GpSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public string Manufacturer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public string Product { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public string SerialNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public long FactorySerialNumber
        //{
        //    get
        //    {
        //        if (_factorySerialNumber == 0)
        //        {
        //            var response = ExecuteCommand<FactorySerialNumberResponse>(new ReadFlashDataCommand(ReadFlashSubCode.ReadChipFactorySerialNumber));

        //            _factorySerialNumber = response.FactorySerialNumber;
        //        }

        //        return _factorySerialNumber;
        //    }
        //}

        //public UsbPowerAttributes UsbPowerAttributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public void Unlock(long code)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public void Reset()
        //{
        //    ExecuteCommand(new ResetCommand());
        //}

        //public void I2CCancelCurrentTransfer()
        //{
        //    throw new NotImplementedException();
        //}

        private void ExecuteCommand(ICommand command)
        {
            var memoryStream = new MemoryStream(new byte[64], true);

            command.Serialise(memoryStream);

            _usbDevice.Write(memoryStream);
        }

        private T ExecuteCommand<T>(ICommand command) where T : IResponse, new()
        {
            var outStream = new MemoryStream(new byte[64], true);
            var inStream = new MemoryStream();

            command.Serialise(outStream);

            _usbDevice.WriteRead(outStream, inStream);

            var result = new T();

            result.Deserialise(inStream);

            return result;
        }

        #region Dispose
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _usbDevice = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Device()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }

        #endregion
    }
}
