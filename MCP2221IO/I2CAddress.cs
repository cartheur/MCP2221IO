using System;
using System.Collections.Generic;
using System.Text;

namespace MCP2221IO
{
    /// <summary>
    /// Represents an I2C device address
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "<Pending>")]
    public class I2cAddress
    {
        public const uint SevenBitRangeLower = 0x07;
        public const uint SevenBitRangeUpper = 0x78;
        public const uint TenBitRangeUpper = 0x03FF;

        public I2cAddress(uint address) : this(address, I2cAddressSize.SevenBit)
        {
        }

        public I2cAddress(uint address, I2cAddressSize size)
        {
            Size = size;
            
            if(Size == I2cAddressSize.SevenBit)
            {
                if ((address <= SevenBitRangeLower) || (address >= SevenBitRangeUpper))
                {
                    throw new ArgumentOutOfRangeException(nameof(address), address, $"{I2cAddressSize.SevenBit} Address must between the range of 0x{SevenBitRangeLower:X2} and 0x{SevenBitRangeUpper:X2}");
                }
            }
            else
            {
                if (address > TenBitRangeUpper)
                {
                    throw new ArgumentOutOfRangeException(nameof(address), address, $"{I2cAddressSize.TenBit} Address must between the range of 0x00 and 0x{TenBitRangeUpper:X4}");
                }
            }

            CalculateAddress(address, size);
        }

        public uint Value { get; private set; }

        public IReadOnlyCollection<byte> ReadAddress { get; private set; }

        public IReadOnlyCollection<byte> WriteAddress { get; private set; }

        public I2cAddressSize Size { get; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"{nameof(Value)}: 0x{Value:X4} {nameof(Size)}: {Size}");

            return stringBuilder.ToString();
        }

        private void CalculateAddress(uint address, I2cAddressSize size)
        {
            switch (size)
            {
                case I2cAddressSize.SevenBit:
                    Value = address;

                    var baseAddress = (byte)(address << 1);

                    WriteAddress = new List<byte>() { baseAddress }.AsReadOnly();
                    ReadAddress = new List<byte>() { (byte)(baseAddress + 1) }.AsReadOnly();
                    break;
                case I2cAddressSize.TenBit:
                    var msb = ((address | 0xF800) & 0xFB00) >> 7;
                    var lsb = address & 0x00FF;
                    
                    Value = address;

                    WriteAddress = new List<byte>() { (byte)msb, (byte)lsb }.AsReadOnly();
                    ReadAddress = new List<byte>() { (byte)(msb + 1), (byte)lsb }.AsReadOnly();
                    break;
                default:
                    break;
            }
        }
    }
}