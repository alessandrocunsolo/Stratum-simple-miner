using StratumMiner.StratumProcol.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.MiningService.ServiceImpl
{
    public class MiningService
    {
        public string BuildCoinBase(SubscribeResponse response)
        {
            var extraOnce2 = "000000";
            var result = response.Notify.Coinbase1 + response.Extranonce1 + extraOnce2 + response.Notify.Coinbase2;

            return result;
        }

        public byte[] BuildDoubleHash(string data)
        {
            var bytes = this.ConvertHexStringToByte(data);

            return this.BuildDoubleHash(bytes);

        }

        public byte[] ConvertHexStringToByte(string data)
        {
            var chunks = new List<string>();
            var dataCopy = (string)data.Clone();
            for (var i = 0; i < data.Length; i += 2)
            {
                chunks.Add(dataCopy.Substring(i, 2));
            }
            var bytes = chunks.Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();

            return bytes;
        }

        public byte[] BuildDoubleHash(byte[] data)
        {
            var hasher = SHA256Managed.Create();
            return hasher.ComputeHash(hasher.ComputeHash(data));
        }

        public byte[] BuildMerkleRoot(string[] data, byte[] coinBaseHash)
        {
            byte[] listByte = coinBaseHash;
            
            foreach (var item in data)
            {
                var tmp = new List<byte>(listByte);
                tmp.AddRange(ConvertHexStringToByte(item));
                listByte = BuildDoubleHash(tmp.ToArray());
            }

            return listByte;
        }

        public byte[] BuildBlockHeader(SubscribeResponse response)
        {
            var coinBase = this.BuildCoinBase(response);
            var coinBaseHash = BuildDoubleHash(coinBase);
            var merkleRoot = this.BuildMerkleRoot(response.Notify.MerkleBranch, coinBaseHash);
            var blockHeader = new List<byte>();
            blockHeader.AddRange(ConvertHexStringToByte(response.Notify.Version));
            blockHeader.AddRange(ConvertHexStringToByte(response.Notify.PreviousHash));
            blockHeader.AddRange(merkleRoot.Reverse());
            blockHeader.AddRange(ConvertHexStringToByte(response.Notify.NTime));
            blockHeader.AddRange(ConvertHexStringToByte(response.Notify.NBits));
            blockHeader.AddRange(ConvertHexStringToByte("00000000"));
            blockHeader.AddRange(ConvertHexStringToByte("000000800000000000000000000000000000000000000000000000000000000000000000000000000000000080020000"));
            return blockHeader.ToArray();
        }

        public byte[] BuildNonce(byte[] blockHeader)
        {
            var bytes = BuildDoubleHash(blockHeader);
            return bytes.Take(4).ToArray();
        }

        public string ConvertByteToString(byte[] bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in bytes)
            {
                stringBuilder.AppendFormat("{0:x2}", item);
            }
            return stringBuilder.ToString();
        }
    }
}
