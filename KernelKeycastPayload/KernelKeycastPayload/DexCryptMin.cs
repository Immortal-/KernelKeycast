/*
*   Copyright (C) 2011, Dextrey (0xDEADDEAD)
* LINK: http://www.hackforums.net/showthread.php?tid=1481595
*/

using System;
using System.Text;

namespace KernelKeycast
{
    internal class DexCryptMin
    {
        public static string Decrypt(string plain, string key)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(plain), Encoding.UTF8.GetBytes(key)));
        }

        public static byte[] Decrypt(byte[] plain, byte[] key)
        {
            var expandedKey = ExpandKey(key, plain.Length);
            var wholeState = plain;
            var magic = plain[plain.Length - 1];
            Array.Resize(ref wholeState, wholeState.Length - 1);

            for (var i = 0; i < wholeState.Length; i++)
            {
                wholeState[i] = (byte)(wholeState[i] ^ magic ^ expandedKey[i]);
            }

            return wholeState;
        }

        public static string Encrypt(string plain, string key)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(plain), Encoding.UTF8.GetBytes(key)));
        }

        public static byte[] Encrypt(byte[] plain, byte[] key)
        {
            var expandedKey = ExpandKey(key, plain.Length);
            var wholeState = plain;
            var magic = (byte)new Random().Next(byte.MaxValue);
            Array.Resize(ref wholeState, plain.Length + 1);
            wholeState[wholeState.Length - 1] = magic;

            for (var i = 0; i < wholeState.Length - 1; i++)
            {
                wholeState[i] = (byte)(wholeState[i] ^ expandedKey[i] ^ magic);
            }

            return wholeState;
        }

        /// <summary>
        ///     Performs DexCryptMin key expansion algorithm on variable length input key
        /// </summary>
        /// <param name="key">Input key</param>
        /// <param name="length">Count of output keystream bytes</param>
        /// <returns>Expanded keystream</returns>
        private static byte[] ExpandKey(byte[] key, int length)
        {
            if (key.Length >= length) return key;
            var rconst = BitConverter.GetBytes(Math.Round(Math.PI, 3));
            var result = new byte[length];
            Buffer.BlockCopy(key, 0, result, 0, key.Length);
            // init round (fill all remaining bytes)
            for (var i = key.Length; i < length; i++)
            {
                // x[i] = ((k[i - len(k)) % len(k)] + x[i - 1]) % 256
                result[i] = (byte)((key[(i - key.Length) % key.Length] ^ (result[i - 1])) % 256);
            }
            // main rounds (process all bytes)
            for (var round = 0; round < 2; round++)
            {
                result[0] = (byte)(result[0] ^ rconst[round]);
                for (var i = 1; i < result.Length; i++)
                {
                    // x[i] = ((x[i] ^ (rcon[r] << (i % 3))) ^ x [i - 1]) % 256
                    result[i] = (byte)(((result[i] ^ (byte)(rconst[round] << (i % 4))) ^ result[i - 1]) % 256);
                }
            }
            return result;
        }
    }
}