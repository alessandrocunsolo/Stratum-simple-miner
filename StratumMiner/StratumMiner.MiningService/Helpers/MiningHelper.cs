using StratumMiner.MiningService.Domain;
using StratumMiner.StratumProcol.Responses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.MiningService.Helpers
{
    public static class MiningHelper
    {
        public static string BuildCoinBase(string extraNonce1, Job notifyRequest,string extraNonce2)
        {
            var result = notifyRequest.Coinbase1 + extraNonce1 + extraNonce2 + notifyRequest.Coinbase2;

            return result;
        }

        public static byte[] BuildDoubleHash(string data)
        {
            var bytes = ConvertHexStringToByte(data);

            return BuildDoubleHash(bytes);

        }

        public static byte[] ConvertHexStringToByte(string data)
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

        public static byte[] BuildDoubleHash(byte[] data)
        {
            var hasher = SHA256Managed.Create();
            return hasher.ComputeHash(hasher.ComputeHash(data));
        }

        public static byte[] BuildMerkleRoot(string[] data, byte[] coinBaseHash)
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

        public static byte[] BuildBlockHeader(string extraNonce1,Job job,string extraNonce2,string nonce)
        {
            var blockHeader = new List<byte>(BuildBlockWithoutOnce(extraNonce1, job, extraNonce2));
            blockHeader.AddRange(ConvertHexStringToByte(nonce));
            blockHeader.AddRange(ConvertHexStringToByte("000000800000000000000000000000000000000000000000000000000000000000000000000000000000000080020000"));
            return blockHeader.ToArray();
        }
        public static byte[] BuildBlockWithoutOnce(string extraNonce1, Job job, string extraNonce2)
        {
            var coinBase = BuildCoinBase(extraNonce1, job, extraNonce2);
            var coinBaseHash = BuildDoubleHash(coinBase);
            var merkleRoot = BuildMerkleRoot(job.MerkleBranch, coinBaseHash);
            var blockHeader = new List<byte>();
            blockHeader.AddRange(ConvertHexStringToByte(job.Version));
            blockHeader.AddRange(ConvertHexStringToByte(job.PreviousHash));
            blockHeader.AddRange(merkleRoot.Reverse());
            blockHeader.AddRange(ConvertHexStringToByte(job.NTime));
            blockHeader.AddRange(ConvertHexStringToByte(job.NBits));
            return blockHeader.ToArray();
        }

        public static string FindNonce(byte[] blockHeaderWithoutOnce,int difficulty, UInt32 maxIteration, UInt256 targetDiff)
        {
            byte[] nonce;
            var hashTable = new Hashtable();
            var extraPadding = ConvertHexStringToByte("000000800000000000000000000000000000000000000000000000000000000000000000000000000000000080020000");

            //var targetDiff = DifficultyToTarget(difficulty);

            var foundNone = "";
            Parallel.For(0, maxIteration, (i, loopState) =>
            {
                nonce = BitConverter.GetBytes(i).Take(4).ToArray();
                
                var newBlockHeader = new List<byte>(blockHeaderWithoutOnce);
                newBlockHeader.AddRange(nonce);
                newBlockHeader.AddRange(extraPadding);
                var hash = BuildDoubleHash(newBlockHeader.ToArray());
                var str = ConvertByteToString(hash);
                
                //verify difficulty
                var hashUint256 = new UInt256(hash);
                
                if (hashUint256.CompareTo(targetDiff) <=0 || str.StartsWith("0000"))
                {
                    Console.WriteLine(str);
                    foundNone = ConvertByteToString(nonce);
                    loopState.Break();
                }
                

            });

            return foundNone;

        }

        public static UInt256 DifficultyToTarget(int difficulty)
        {
            var diff1 = UInt256.Parse("00000000ffff0000000000000000000000000000000000000000000000000000");
            var uint256Diff = new UInt256(BitConverter.GetBytes(difficulty));
            return diff1 / uint256Diff;
        }

        public static byte[] GetNonce()
        {
            var random = new Random();
            var bytes = new byte[4];
            random.NextBytes(bytes);

            return bytes;
            //var bytes = BuildDoubleHash(blockHeader);
            //return bytes.Take(4).ToArray();
        }
        public static string GetNonceString()
        {
            var hexString = "1234567890abcdef";
            var random = new Random();
            return new string(hexString.OrderBy(x => random.Next(int.MaxValue)).Take(8).ToArray());
        }

        public static string ConvertByteToString(byte[] bytes)
        {
            var stringBuilder = new StringBuilder();
            var reversedBytes = bytes.Reverse();
            foreach (var item in reversedBytes)
            {
                stringBuilder.AppendFormat("{0:x2}", item);
            }
            return stringBuilder.ToString();
        }
    }
}
