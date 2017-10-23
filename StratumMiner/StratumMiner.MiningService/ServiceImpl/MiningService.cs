using StratumMiner.StratumProcol.Responses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.MiningService.ServiceImpl
{
    public class MiningService
    {
        public string BuildCoinBase(string extraNonce1, NotifyRequest notifyRequest,string extraNonce2)
        {
            var result = notifyRequest.Coinbase1 + extraNonce1 + extraNonce2 + notifyRequest.Coinbase2;

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

        public byte[] BuildBlockHeader(string extraNonce1,NotifyRequest notifyRequest,string extraNonce2,string nonce)
        {
            var blockHeader = new List<byte>(this.BuildBlockWithoutOnce(extraNonce1, notifyRequest, extraNonce2));
            blockHeader.AddRange(ConvertHexStringToByte(nonce));
            blockHeader.AddRange(ConvertHexStringToByte("000000800000000000000000000000000000000000000000000000000000000000000000000000000000000080020000"));
            return blockHeader.ToArray();
        }
        public byte[] BuildBlockWithoutOnce(string extraNonce1, NotifyRequest notifyRequest, string extraNonce2)
        {
            var coinBase = this.BuildCoinBase(extraNonce1, notifyRequest, extraNonce2);
            var coinBaseHash = BuildDoubleHash(coinBase);
            var merkleRoot = this.BuildMerkleRoot(notifyRequest.MerkleBranch, coinBaseHash);
            var blockHeader = new List<byte>();
            blockHeader.AddRange(ConvertHexStringToByte(notifyRequest.Version));
            blockHeader.AddRange(ConvertHexStringToByte(notifyRequest.PreviousHash));
            blockHeader.AddRange(merkleRoot.Reverse());
            blockHeader.AddRange(ConvertHexStringToByte(notifyRequest.NTime));
            blockHeader.AddRange(ConvertHexStringToByte(notifyRequest.NBits));
            return blockHeader.ToArray();
        }

        public string FindNonce(byte[] blockHeaderWithoutOnce,int difficulty, UInt32 maxIteration)
        {
            byte[] nonce;
            var hashTable = new Hashtable();
            var extraPadding = ConvertHexStringToByte("000000800000000000000000000000000000000000000000000000000000000000000000000000000000000080020000");
            Parallel.For(0, maxIteration, (i, loopState) =>
            {
                var zeroCount = 0;
                nonce = BitConverter.GetBytes(i);
                var newBlockHeader = new List<byte>(blockHeaderWithoutOnce);
                newBlockHeader.AddRange(nonce);
                newBlockHeader.AddRange(extraPadding);
                var hash = this.BuildDoubleHash(newBlockHeader.ToArray());
                var reverseHash = hash.Reverse();
                var equalTozero = true;

                foreach (var item in reverseHash)
                {
                    if (item == 0 && equalTozero)
                    {
                        zeroCount += 1;
                        continue;
                    }
                    break;
                }
                if (zeroCount >= difficulty)
                {
                    var nonceKey = ConvertByteToString(nonce);
                    if (!hashTable.ContainsKey(nonceKey))
                    {
                        hashTable.Add(nonceKey, zeroCount);
                    }
                    loopState.Break();
                }
                

            });
            var maxCount = 0;
            var maxNonce = "";
            foreach (var key in hashTable.Keys)
            {
                var value = (int)hashTable[key];
                if (value > maxCount)
                {
                    maxCount = value;
                    maxNonce =(string)key;
                }
            }

            return maxNonce;

        }
        public byte[] GetNonce()
        {
            var random = new Random();
            var bytes = new byte[4];
            random.NextBytes(bytes);

            return bytes;
            //var bytes = BuildDoubleHash(blockHeader);
            //return bytes.Take(4).ToArray();
        }
        public string GetNonceString()
        {
            var hexString = "1234567890abcdef";
            var random = new Random();
            return new string(hexString.OrderBy(x => random.Next(int.MaxValue)).Take(8).ToArray());
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
