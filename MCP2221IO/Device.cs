﻿using MCP2221IO.Commands;
using MCP2221IO.Exceptions;
using MCP2221IO.Gpio;
using MCP2221IO.Responses;
using MCP2221IO.Settings;
using MCP2221IO.Usb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MCP2221IO
{
    /// <summary>
    /// A MCP2221 device
    /// </summary>
    public class Device : IDevice
    {
        internal const int MaxPacketSize = 64;
        internal const int MaxBlockSize = 59;
        internal bool _gpioPortsRead = false;

        private const int MaxRetries = 5;

        private readonly ILogger<IDevice> _logger;
        private string _factorySerialNumber;
        private IHidDevice _hidDevice;

        public Device(ILogger<IDevice> logger, IHidDevice hidDevice)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hidDevice = hidDevice ?? throw new ArgumentNullException(nameof(hidDevice));
        }
        public Device(IHidDevice hidDevice)
        {
            _hidDevice = hidDevice ?? throw new ArgumentNullException(nameof(hidDevice));
        }

        // <inheritdoc/>
        public DeviceStatus Status { get; internal set; }

        // <inheritdoc/>
        public ChipSettings ChipSettings { get; internal set; }

        // <inheritdoc/>
        public GpSettings GpSettings { get; internal set; }

        // <inheritdoc/>
        public SramSettings SramSettings { get; internal set; }

        // <inheritdoc/>
        public GpioPort GpioPort0 { get; internal set; }

        // <inheritdoc/>
        public GpioPort GpioPort1 { get; internal set; }

        // <inheritdoc/>
        public GpioPort GpioPort2 { get; internal set; }

        // <inheritdoc/>
        public GpioPort GpioPort3 { get; internal set; }

        // <inheritdoc/>
        public string UsbManufacturerDescriptor
        {
            get => HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    return ExecuteCommand<ReadFlashStringResponse>(new ReadUsbManufacturerDescriptorCommand()).Value;
                });
            set =>
                HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    ExecuteCommand<WriteFlashDataResponse>(new WriteUsbManufacturerDescriptorCommand(value));
                });
        }

        // <inheritdoc/>
        public string UsbProductDescriptor
        {
            get => HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    return ExecuteCommand<ReadFlashStringResponse>(new ReadUsbProductDescriptorCommand()).Value;
                });
            set => HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    ExecuteCommand<WriteFlashDataResponse>(new WriteUsbProductDescriptorCommand(value));
                });
        }

        // <inheritdoc/>
        public string UsbSerialNumberDescriptor
        {
            get => HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    return ExecuteCommand<ReadFlashStringResponse>(new ReadUsbSerialNumberDescriptorCommand()).Value;
                });
            set => HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    ExecuteCommand<WriteFlashDataResponse>(new WriteUsbSerialNumberCommand(value));
                });
        }

        // <inheritdoc/>
        public string FactorySerialNumber => GetFactorySerialNumber();

        // <inheritdoc/>
        public void UnlockFlash(Password password)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    ExecuteCommand<UnlockFlashResponse>(new UnlockFlashCommand(password));
                });
        }

        // <inheritdoc/>
        public void ReadDeviceStatus()
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    Status = ExecuteCommand<StatusSetParametersResponse>(new ReadStatusSetParametersCommand()).DeviceStatus;
                });
        }

        // <inheritdoc/>
        public void ReadChipSettings()
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    ChipSettings = ExecuteCommand<ReadChipSettingsResponse>(new ReadChipSettingsCommand()).ChipSettings;
                });
        }

        // <inheritdoc/>
        public void ReadGpSettings()
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    GpSettings = ExecuteCommand<ReadGpSettingsResponse>(new ReadGpSettingsCommand()).GpSettings;
                });
        }

        // <inheritdoc/>
        public void ReadSramSettings()
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    SramSettings = ExecuteCommand<ReadSramSettingsResponse>(new ReadSramSettingsCommand()).SramSettings;
                });
        }

        // <inheritdoc/>
        public void ReadGpioPorts()
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    var response = ExecuteCommand<ReadGpioPortsResponse>(new ReadGpioPortsCommand());

                    GpioPort0 = response.GpioPort0;
                    GpioPort1 = response.GpioPort1;
                    GpioPort2 = response.GpioPort2;
                    GpioPort3 = response.GpioPort3;

                    _gpioPortsRead = true;
                });
        }

        // <inheritdoc/>
        public void WriteChipSettings(Password password)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    if (ChipSettings == null)
                    {
                        throw new ReadRequiredException($"{nameof(ChipSettings)} must be read from the device");
                    }

                    ExecuteCommand<WriteFlashDataResponse>(new WriteChipSettingsCommand(ChipSettings, password));
                });
        }

        // <inheritdoc/>
        public void WriteGpSettings()
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    if (GpSettings == null)
                    {
                        throw new ReadRequiredException($"{nameof(GpSettings)} must be read from the device");
                    }

                    ExecuteCommand<WriteFlashDataResponse>(new WriteGpSettingsCommand(GpSettings));
                });
        }

        // <inheritdoc/>
        public void WriteSramSettings(bool clearInterrupts)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    if (SramSettings == null)
                    {
                        throw new ReadRequiredException($"{nameof(SramSettings)} must be read from the device");
                    }

                    ExecuteCommand<WriteSramSettingsResponse>(new WriteSramSettingsCommand(SramSettings, clearInterrupts));
                });
        }

        // <inheritdoc/>
        public void WriteGpioPorts()
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    if (!_gpioPortsRead)
                    {
                        throw new ReadRequiredException($"Gpio ports must be read from the device");
                    }

                    ExecuteCommand<WriteGpioPortsResponse>(new WriteGpioPortsCommand(GpioPort0, GpioPort1, GpioPort2, GpioPort3));
                });
        }

        // <inheritdoc/>
        public void Reset()
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    ExecuteCommand(new ResetCommand());
                });
        }

        // <inheritdoc/>
        public void CancelI2cBusTransfer()
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    Status = ExecuteCommand<StatusSetParametersResponse>(new CancelI2cBusTransferCommand()).DeviceStatus;
                });
        }

        // <inheritdoc/>
        public void SetI2cBusSpeed(int speed)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    int retryCount = 0;

                    do
                    {
                        Status = ExecuteCommand<StatusSetParametersResponse>(new SetI2cBusSpeedCommand(speed)).DeviceStatus;

                        retryCount++;

                    } while (Status.SpeedStatus != I2cSpeedStatus.Set && retryCount < MaxRetries);

                    if (retryCount == MaxRetries)
                    {
                        throw new I2cOperationException("Unable to set I2C Speed exceeded retry count");
                    }
                });
        }

        // <inheritdoc/>
        public void Open()
        {
            HandleOperationExecution(nameof(Device), () => _hidDevice.Open());
        }

        // <inheritdoc/>
        public void Close()
        {
            HandleOperationExecution(nameof(Device), () => Dispose());
        }

        // <inheritdoc/>
        public void I2cWriteData(I2cAddress address, IList<byte> data)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    I2cWriteData<I2cWriteDataResponse>(CommandCodes.WriteI2cData, address, data);
                });
        }

        // <inheritdoc/>
        public void I2cWriteDataRepeatStart(I2cAddress address, IList<byte> data)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    I2cWriteData<I2cWriteDataRepeatStartResponse>(CommandCodes.WriteI2cDataRepeatedStart, address, data);
                });
        }

        // <inheritdoc/>
        public void I2cWriteDataNoStop(I2cAddress address, IList<byte> data)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    I2cWriteData<I2cWriteDataNoStopResponse>(CommandCodes.WriteI2cDataNoStop, address, data);
                });
        }

        // <inheritdoc/>
        public IList<byte> I2cReadData(I2cAddress address, ushort length)
        {
            return HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    return I2cReadData<I2cReadDataResponse>(CommandCodes.ReadI2cData, address, length);
                });
        }

        // <inheritdoc/>
        public IList<byte> I2cReadDataRepeatedStart(I2cAddress address, ushort length)
        {
            return HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    return I2cReadData<I2cReadDataRepeatedStarteResponse>(CommandCodes.ReadI2cDataRepeatedStart, address, length);
                });
        }

        // <inheritdoc/>
        public IList<I2cAddress> I2cScanBus(bool useTenBitAddressing)
        {
            uint upperAddress = useTenBitAddressing ? I2cAddress.TenBitRangeUpper : I2cAddress.SevenBitRangeUpper;

            return I2cScanBusInternal(useTenBitAddressing, upperAddress);
        }

        // <inheritdoc/>
        public void SmBusQuickCommand(I2cAddress address, bool write)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    AssertAddress(address);

                    if (write)
                    {
                        ExecuteCommand<I2cWriteDataResponse>(new I2cWriteDataCommand(CommandCodes.WriteI2cData, address, new List<byte>()));
                    }
                    else
                    {
                        ExecuteCommand<I2cReadDataResponse>(new I2cReadDataCommand(CommandCodes.ReadI2cData, address, 0));
                    }
                });
        }

        // <inheritdoc/>
        public byte SmBusReadByte(I2cAddress address, bool pec = false)
        {
            return HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    AssertAddress(address);

                    var result = I2cReadData<I2cReadDataResponse>(CommandCodes.ReadI2cData, address, sizeof(byte));

                    if (pec)
                    {
                        AssertPec(result);
                    }

                    return result[0];
                });
        }

        // <inheritdoc/>
        public void SmBusWriteByte(I2cAddress address, byte data, bool pec = false)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    AssertAddress(address);

                    List<byte> writeData = new List<byte>() { data };

                    if (pec)
                    {
                        writeData.Add(Crc8.ComputeChecksum(new List<byte>() { data }));
                    }

                    I2cWriteData(address, writeData);
                });
        }

        // <inheritdoc/>
        public byte SmBusReadByteCommand(I2cAddress address, byte command, bool pec = false)
        {
            return SmBusReadCommand(address, command, sizeof(byte), pec).First();
        }

        // <inheritdoc/>
        public void SmBusWriteByteCommand(I2cAddress address, byte command, byte data, bool pec = false)
        {
            SmBusWriteCommand(address, command, pec, data);
        }

        // <inheritdoc/>
        public short SmBusReadShortCommand(I2cAddress address, byte command, bool pec = false)
        {
            return BitConverter.ToInt16(SmBusReadCommand(address, command, sizeof(short), pec).Take(sizeof(short)).ToArray());
        }

        // <inheritdoc/>
        public void SmBusWriteShortCommand(I2cAddress address, byte command, short data, bool pec = false)
        {
            SmBusWriteCommand(address, command, pec, BitConverter.GetBytes(data));
        }

        // <inheritdoc/>
        public int SmBusReadIntCommand(I2cAddress address, byte command, bool pec = false)
        {
            return BitConverter.ToInt32(SmBusReadCommand(address, command, sizeof(int), pec).Take(sizeof(int)).ToArray());
        }

        // <inheritdoc/>
        public void SmBusWriteIntCommand(I2cAddress address, byte command, int data, bool pec = false)
        {
            SmBusWriteCommand(address, command, pec, BitConverter.GetBytes(data));
        }

        // <inheritdoc/>
        public long SmBusReadLongCommand(I2cAddress address, byte command, bool pec = false)
        {
            return BitConverter.ToInt64(SmBusReadCommand(address, command, sizeof(long), pec).Take(sizeof(long)).ToArray());
        }

        // <inheritdoc/>
        public void SmBusWriteLongCommand(I2cAddress address, byte command, long data, bool pec = false)
        {
            SmBusWriteCommand(address, command, pec, BitConverter.GetBytes(data));
        }

        // <inheritdoc/>
        public IList<byte> SmBusBlockRead(I2cAddress address, byte command, byte count, bool pec = false)
        {
            return HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    List<byte> writeData = new List<byte>() { command };

                    I2cWriteData<I2cWriteDataNoStopResponse>(CommandCodes.WriteI2cDataNoStop, address, writeData);

                    var result = I2cReadData<I2cReadDataRepeatedStarteResponse>(CommandCodes.ReadI2cDataRepeatedStart, address, count);

                    if (pec)
                    {
                        AssertPec(result);
                    }

                    return result;
                });
        }

        // <inheritdoc/>
        public void SmBusBlockWrite(I2cAddress address, byte command, IList<byte> block, bool pec = false)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    AssertAddress(address);

                    if(block.Count > IDevice.MaxSmBusBlockSize)
                    {
                        throw new ArgumentOutOfRangeException(nameof(block), $"Must be less than [0x{IDevice.MaxSmBusBlockSize}]");
                    }

                    List<byte> writeData = new List<byte>() { command, (byte)block.Count };
                    writeData.AddRange(block);

                    if (pec)
                    {
                        writeData.Add(Crc8.ComputeChecksum(writeData));
                    }

                    I2cWriteData(address, writeData );
                });
        }

        internal IList<I2cAddress> I2cScanBusInternal(bool useTenBitAddressing, uint upperAddress)
        {
            List<I2cAddress> result = new List<I2cAddress>();

            for (uint i = I2cAddress.SevenBitRangeLower + 1; i < upperAddress; i++)
            {
                I2cAddress address = new I2cAddress(i, useTenBitAddressing ? I2cAddressSize.TenBit : I2cAddressSize.SevenBit);

                try
                {
                    _logger.LogDebug($"Probing Device Address [0x{address.Value:X}]");

                    var response = I2cReadData(address, 1);

                    if (response.Count > 0)
                    {
                        _logger.LogDebug($"Read [{response.Count}] byte from Device Address [0x{address.Value:X}]");
                    }

                    result.Add(address);
                }
                catch (CommandExecutionFailedException ex)
                {
                    HandleScanError(address, ex);
                }
                catch (I2cOperationException ex)
                {
                    HandleScanError(address, ex);
                }
            }

            return result;
        }

        private IList<byte> SmBusReadCommand(I2cAddress address, byte command, ushort length, bool pec)
        {
            return HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    List<byte> writeData = new List<byte>() { command };

                    I2cWriteData<I2cWriteDataNoStopResponse>(CommandCodes.WriteI2cDataNoStop, address, writeData);

                    var result = I2cReadData<I2cReadDataRepeatedStarteResponse>(CommandCodes.ReadI2cDataRepeatedStart, address, length);

                    if (pec)
                    {
                        AssertPec(result);
                    }

                    return result;
                });
        }

        private void SmBusWriteCommand(I2cAddress address, byte command, bool pec, params byte[] data)
        {
            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    List<byte> writeData = new List<byte>() { command };
                    writeData.AddRange(data);

                    if (pec)
                    {
                        writeData.Add(Crc8.ComputeChecksum(writeData));
                    }

                    I2cWriteData<I2cWriteDataResponse>(CommandCodes.WriteI2cData, address, writeData);
                });
        }

        private void HandleScanError(I2cAddress address, Exception ex)
        {
            _logger.LogWarning(ex, $"Device Address [0x{address.Value:X2}] did not respond");

            CancelI2cBusTransfer();
        }

        private void AssertAddress(I2cAddress address)
        {
            if(address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (address.Size != I2cAddressSize.SevenBit)
            {
                throw new SmBusInvalidAddressSizeException($"{nameof(I2cAddress)} size must be {I2cAddressSize.SevenBit}");
            }
        }

        private void AssertPec(IList<byte> result)
        {
            if (result.Count > 1)
            {
                byte expected = Crc8.ComputeChecksum(result.Take(result.Count - 1).ToList());

                if (result[result.Count - 1] != expected)
                {
                    throw new SmBusInvalidCrcException(expected, result[result.Count - 1], "PEC does not match");
                }
            }
        }

        private IList<byte> I2cReadData<T>(CommandCodes commandCode, I2cAddress address, ushort length) where T : IResponse, new()
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    List<byte> result = new List<byte>();

                    var response = ExecuteCommand<T>(new I2cReadDataCommand(commandCode, address, length), false);

                    if (response.ExecutionResult != 0)
                    {
                        throw new I2cOperationException(response.ExecutionResult, $"{nameof(I2cReadData)} The read of i2c data failed with execution result code [0x{response.ExecutionResult:x}]");
                    }

                    while (result.Count < length || length == 0)
                    {
                        var getResponse = ExecuteCommand<GetI2cDataResponse>(new GetI2cDataCommand(), false);

                        if (getResponse.ExecutionResult != 0)
                        {
                            throw new I2cOperationException(getResponse.ExecutionResult, $"{nameof(I2cReadData)} The read of i2c data failed with execution result code [0x{getResponse.ExecutionResult:x}]");
                        }

                        if (getResponse.Data.Count > 0)
                        {
                            result.AddRange(getResponse.Data);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return result;
                });
        }

        private void I2cWriteData<T>(CommandCodes commandCode, I2cAddress address, IList<byte> data) where T : IResponse, new()
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Count > IDevice.MaxI2cBlockSize)
            {
                throw new ArgumentOutOfRangeException(nameof(data), data, $"Must be less than 0x{IDevice.MaxI2cBlockSize:X4}");
            }

            HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    int blockCount = (data.Count + MaxBlockSize - 1) / MaxBlockSize;

                    for (int i = 0; i < blockCount; i++)
                    {
                        int blockSize = Math.Min(MaxBlockSize, Math.Abs(data.Count - (i * MaxBlockSize)));

                        T response = ExecuteCommand<T>(new I2cWriteDataCommand(commandCode, address, data.Skip(MaxBlockSize * i).Take(blockSize).ToList()));

                        if (response.ExecutionResult != 0)
                        {
                            throw new I2cOperationException(response.ExecutionResult, $"{nameof(I2cWriteData)} The write of i2c data failed with execution result code [0x{response.ExecutionResult:x}]");
                        }
                    }
                });
        }

        private string GetFactorySerialNumber()
        {
            return HandleOperationExecution(
                nameof(Device),
                () =>
                {
                    if (String.IsNullOrWhiteSpace(_factorySerialNumber))
                    {
                        var response = ExecuteCommand<FactorySerialNumberResponse>(new ReadFactorySerialNumberCommand());

                        _factorySerialNumber = response.SerialNumber;
                    }

                    return _factorySerialNumber;
                });
        }

        private void ExecuteCommand(ICommand command)
        {
            var memoryStream = new MemoryStream(new byte[MaxPacketSize], true);

            command.Serialize(memoryStream);

            _hidDevice.Write(memoryStream.ToArray());
        }

        private T ExecuteCommand<T>(ICommand command, bool checkResult = true) where T : IResponse, new()
        {
            var outStream = new MemoryStream(new byte[MaxPacketSize], true);
            var inStream = new MemoryStream();

            command.Serialize(outStream);

            inStream.Write(_hidDevice.WriteRead(outStream.ToArray()));

            var result = new T();

            result.Deserialize(inStream);

            if (checkResult && result.ExecutionResult != 0)
            {
                throw new CommandExecutionFailedException($"Unexpected command execution status Expected: [0x00] Actual [0x{result.ExecutionResult:x}]");
            }

            return result;
        }

        [DebuggerStepThrough]
        private void HandleOperationExecution(string className, Action operation, [CallerMemberName] string memberName = "")
        {
            Stopwatch sw = new Stopwatch();

            try
            {
                sw.Start();
                operation();
                sw.Stop();

                _logger.LogDebug($"Executed [{className}].[{memberName}] in [{sw.Elapsed.TotalMilliseconds}] ms");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred executing [{className}].[{memberName}] Reason: [{ex.Message}]");

                throw;
            }
        }

        [DebuggerStepThrough]
        private T HandleOperationExecution<T>(string className, Func<T> operation, [CallerMemberName] string memberName = "")
        {
            Stopwatch sw = new Stopwatch();
            T result;

            try
            {
                sw.Start();
                result = operation();
                sw.Stop();

                _logger.LogDebug($"Executed [{className}].[{memberName}] in [{sw.Elapsed.TotalMilliseconds}] ms");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred executing [{className}].[{memberName}] Reason: [{ex.Message}]");

                throw;
            }

            return result;
        }

        #region Dispose
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            _logger.LogDebug($"Disposing {nameof(Device)}");

            if (!disposedValue)
            {
                if (disposing)
                {
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

        #endregion
    }
}
