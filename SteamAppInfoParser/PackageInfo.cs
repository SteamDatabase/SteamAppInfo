using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ValveKeyValue;

namespace SteamAppInfoParser
{
    class PackageInfo
    {
        public EUniverse Universe { get; set; }

        public List<Package> Packages { get; set; } = [];

        /// <summary>
        /// Opens and reads the given filename.
        /// </summary>
        /// <param name="filename">The file to open and read.</param>
        public void Read(string filename)
        {
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Read(fs);
        }

        /// <summary>
        /// Reads the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="input">The input <see cref="Stream"/> to read from.</param>
        public void Read(Stream input)
        {
            using var reader = new BinaryReader(input);
            var magic = reader.ReadUInt32();


            var version = magic & 0xFF;
            magic >>= 8;

            if (magic != 0x06_56_55)
            {
                throw new InvalidDataException($"Unknown magic header: {magic:X}");
            }

            if (version < 39 || version > 40)
            {
                throw new InvalidDataException($"Unknown magic version: {version}");
            }

            Universe = (EUniverse)reader.ReadUInt32();

            var deserializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Binary);

            do
            {
                var subid = reader.ReadUInt32();

                if (subid == 0xFFFFFFFF)
                {
                    break;
                }

                var package = new Package
                {
                    SubID = subid,
                    Hash = new ReadOnlyCollection<byte>(reader.ReadBytes(20)),
                    ChangeNumber = reader.ReadUInt32(),
                };

                if (version >= 40)
                {
                    package.Token = reader.ReadUInt64();
                }

                package.Data = deserializer.Deserialize(input);

                Packages.Add(package);
            }
            while (true);
        }
    }
}
