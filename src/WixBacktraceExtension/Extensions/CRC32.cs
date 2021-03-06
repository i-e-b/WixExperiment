﻿namespace WixBacktraceExtension.Extensions
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// CRC32 string extension
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Return CRC32 as Int from a source string
        /// </summary>
        public static int CRC32(this string src)
        {
            return (int)Crc32.Compute(Encoding.UTF8.GetBytes(src));
        }
    }

    /// <summary>
    /// CRC32 hash
    /// </summary>
    public class Crc32 : HashAlgorithm
    {
        /// <summary> Seed data </summary>
        public const UInt32 DefaultPolynomial = 0xedb88320;
        /// <summary> Seed data </summary>
        public const UInt32 DefaultSeed = 0xffffffff;

        private UInt32 hash;
        private readonly UInt32 seed;
        private readonly UInt32[] table;
        private static UInt32[] defaultTable;

        /// <summary>
        /// Initialise CRC32
        /// </summary>
        public Crc32()
        {
            table = InitializeTable(DefaultPolynomial);
            seed = DefaultSeed;
            Initialize();
        }

        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        /// <returns>
        /// The size, in bits, of the computed hash code.
        /// </returns>
        public override int HashSize { get { return 32; } }

        /// <summary>
        /// Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm"/> class.
        /// </summary>
        public override sealed void Initialize() { hash = seed; }

        /// <summary>
        /// When overridden in a derived class, routes data written to the object into the hash algorithm for computing the hash.
        /// </summary>
        protected override void HashCore(byte[] buffer, int start, int length)
        {
            hash = CalculateHash(table, hash, buffer, start, length);
        }

        /// <summary>
        /// When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.
        /// </summary>
        /// <returns>
        /// The computed hash code.
        /// </returns>
        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt32ToBigEndianBytes(~hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        /// <summary> Compute hash </summary>
        public static UInt32 Compute(byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
        }

        /// <summary> Compute hash </summary>
        public static UInt32 Compute(UInt32 seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
        }

        /// <summary> Compute hash </summary>
        public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        private static UInt32[] InitializeTable(UInt32 polynomial)
        {
            if (polynomial == DefaultPolynomial && defaultTable != null) return defaultTable;

            var createTable = new UInt32[256];
            for (var i = 0; i < 256; i++)
            {
                var entry = (UInt32)i;
                for (var j = 0; j < 8; j++) entry = (entry & 1) == 1 ? (entry >> 1) ^ polynomial : entry >> 1;
                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial) defaultTable = createTable;
            return createTable;
        }

        // ReSharper disable SuggestBaseTypeForParameter
        private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size)
        {
            var crc = seed;
            for (var i = start; i < size; i++)
                unchecked
                {
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                }
            return crc;
        }
        // ReSharper restore SuggestBaseTypeForParameter

        private static byte[] UInt32ToBigEndianBytes(UInt32 x)
        {
            return new[] {
				(byte)((x >> 24) & 0xff),
				(byte)((x >> 16) & 0xff),
				(byte)((x >> 8) & 0xff),
				(byte)(x & 0xff)
			};
        }
    }
}