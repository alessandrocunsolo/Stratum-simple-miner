using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
namespace StratumMiner.MiningService.Domain
{
    public class UInt256
    {
        private byte[] data = new byte[32];
        public const int Size = 32;

        public UInt256(byte[] data)
        {
            var list = new List<byte>(data);
            if (data.Length <32)
            {
                var zeros = Size - data.Length;
                for (int i = 0; i < zeros; i++)
                {
                    list.Add(0);
                }

            }
            this.data = list.ToArray();

        }

        public UInt256()
        {
            for (int i = 0; i < Size; i++)
            {
                this.data[i] = 0;
            }
        }

        public byte[] Data { get { return this.data; } }
        public static UInt256 Parse(string hexString)
        {
            if (hexString.Length > 64) throw new OverflowException("Max length of HEX string must be 64");
            var chunks = new List<string>();
            if (hexString.Length < 2)
            {
                return new UInt256(new byte[1] { byte.Parse(hexString, System.Globalization.NumberStyles.HexNumber) });
            }
            for (var i = 0; i < hexString.Length; i += 2)
            {
                chunks.Add(hexString.Substring(i, 2));
            }
            var data = chunks.Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).Reverse().ToArray();

            return new UInt256(data);
        }

        public int CompareTo(UInt256 other)
        {
            var i = 32;
            while(i-->0)
            {
                if (this.data[i] > other.data[i])
                    return 1;
                if (this.data[i] < other.data[i])
                    return -1;
            }

            return 0;
        }

        public static UInt256 operator /(UInt256 dividend, UInt256 divisor)
        {
            var bigDividend = new BigInteger(dividend.data);
            var bitDivisor = new BigInteger(divisor.data);

            var result = bigDividend / bitDivisor;
            return new UInt256(result.ToByteArray());
        }
    }
}
