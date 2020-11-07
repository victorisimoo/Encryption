using EncryptionController.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace EncryptionController.Controllers {
    public class CesarEncryption {

        private static Dictionary<int, int> principalDictionary = new Dictionary<int, int>();

        private static void getDictionary(string key, int opc) {

            principalDictionary = new Dictionary<int, int>();
            key = key.ToUpper();
            var originalCount = 65;
            var newCont = 65;

            if (opc == 1) {
                do {
                    if (key.Length > 0) {
                        if (!principalDictionary.ContainsValue(key[0])) {
                            principalDictionary.Add(originalCount, key[0]);
                            originalCount++;
                        }
                        key = key.Substring(1, key.Length - 1);
                    } else {
                        if (!principalDictionary.ContainsValue(newCont)) {
                            principalDictionary.Add(originalCount, newCont);
                            originalCount++;
                        }
                        newCont++;
                    }
                } while (originalCount < 91);
            }else {
                do {
                    if (key.Length > 0) {
                        if (!principalDictionary.ContainsKey(key[0])) {
                            principalDictionary.Add(key[0], originalCount);
                            originalCount++;
                        }
                        key = key.Substring(1, key.Length - 1);
                    } else {
                        if (!principalDictionary.ContainsKey(newCont)) {
                            principalDictionary.Add(newCont, originalCount);
                            originalCount++;
                        }
                        newCont++;
                    }
                } while (originalCount < 91);
            }
        }


        public static void Encryption(Key values, IFormFile file, string routeDirectory) {

            if (!Directory.Exists(Path.Combine(routeDirectory, "encryption"))) {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "encryption"));
            }

            getDictionary(values.Word, 1);
            using (var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "encryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.crs"), FileMode.OpenOrCreate)){
                    using (var writer = new BinaryWriter(streamWriter)) {
                        var bffLength = 10000;
                        var bffByte = new byte[bffLength];
                        while (reader.BaseStream.Position != reader.BaseStream.Length) {
                            bffByte = reader.ReadBytes(bffLength);

                            foreach (var character in bffByte) {
                                var actualCharacter = Convert.ToInt32(character);
                                if (actualCharacter >= 65 && actualCharacter <= 90) {
                                    writer.Write((byte)principalDictionary[actualCharacter]);
                                } else if (actualCharacter >= 97 && actualCharacter <= 122) {
                                    writer.Write((byte)(principalDictionary[actualCharacter - 32] + 32));
                                } else {
                                    writer.Write(character);
                                }
                            }
                        }

                    }
                }
            }
        }

        public static void Decryption(Key values, IFormFile file, string routeDirectory) {
            if (!Directory.Exists(Path.Combine(routeDirectory, "decryption"))) {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "decryption"));
            }
            getDictionary(values.Word, 2);
            using(var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "decryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {
                        var bffLenght = 10000;
                        var bffByte = new byte[bffLenght];
                        while (reader.BaseStream.Position != reader.BaseStream.Length) {
                            bffByte = reader.ReadBytes(bffLenght);

                            foreach (var caracter in bffByte) {
                                var actual = Convert.ToInt32(caracter);
                                if (actual >= 65 && actual <= 90) {
                                    writer.Write((byte)principalDictionary[actual]);

                                } else if (actual >= 97 && actual <= 122) {
                                    writer.Write((byte)(principalDictionary[actual - 32] + 32));
                                } else {
                                    writer.Write(caracter);
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
