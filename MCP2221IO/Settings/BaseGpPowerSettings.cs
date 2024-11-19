using System.IO;
using System.Text;

namespace MCP2221IO.Settings
{
    /// <summary>
    /// A Gpio setting type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GpSetting<T> where T : System.Enum
    {
        /// <summary>
        /// The current output value on the Gpio port
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// The Gpio port direction
        /// </summary>
        public bool IsInput { get; set; }

        /// <summary>
        /// The current Gp settings port designation
        /// </summary>
        public T Designation { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(
                $"{nameof(Value)}: {Value,5} " +
                $"{nameof(IsInput)}: {IsInput,5} " +
                $"{nameof(Designation)}: {Designation,17} ");

            return stringBuilder.ToString();
        }

        // <inheritdoc/>
        internal void Deserialize(Stream stream)
        {
            int temp = stream.ReadByte();

            Value = (temp & 0x10) == 0x10;
            IsInput = (temp & 0x08) == 0x08;
            Designation = (T)(object)(temp & 0x07);
        }

        internal void Serialize(Stream stream)
        {
            int update = Value ? 0x10 : 0x00;
            update |= IsInput ? 0x08 : 0x00;
            update |= (int)(object)Designation & 0b111;

            stream.WriteByte((byte)update);
        }
    }
}

