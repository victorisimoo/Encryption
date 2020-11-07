using EncryptionController.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EncryptionController.Controllers {
    public class ZigZagEncryption {

        public static void Encryption (Key values, IFormFile file, string routeDirectory) {

            if (!Directory.Exists(Path.Combine(routeDirectory, "encryption"))) {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "encryption"));
            }

            using (var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "encryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.zz"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {

                        var waves = (2 * values.Levels) - 2;
                        var valueWaves = (float)reader.BaseStream.Length / (float)waves;
                        var cantWaves = valueWaves % 1 <= 0.5 ? Math.Round(valueWaves) + 1 : Math.Round(valueWaves);
                        cantWaves = Convert.ToInt32(cantWaves);
                        var bufferLength = 100000;
                        var byteBuffer = new byte[bufferLength];
                        var result = new List<byte>[values.Levels];
                        var position = 0;
                        var cantLevels = 0;

                        for (int i = 0; i < values.Levels; i++) {
                            result[i] = new List<byte>();
                        }

                        while (reader.BaseStream.Position != reader.BaseStream.Length) {
                            byteBuffer = reader.ReadBytes(bufferLength);
                            foreach (var caracter in byteBuffer) {
                                if (position == 0 || position % waves == 0) {
                                    result[0].Add(caracter);
                                    cantLevels = 0;
                                } else if (position % waves == values.Levels - 1) {
                                    result[values.Levels - 1].Add(caracter);
                                    cantLevels = values.Levels - 1;
                                } else if (position % waves < values.Levels - 1) {
                                    cantLevels++;
                                    result[cantLevels].Add(caracter);
                                } else if (position % waves > values.Levels - 1) {
                                    cantLevels--;
                                    result[cantLevels].Add(caracter);
                                }
                                position++;
                            }
                        }

                        for (int i = 0; i < values.Levels; i++) {
                            var cantIteracion = i == 0 || i == values.Levels - 1 ? cantWaves : cantWaves * 2;
                            var inicio = result[i].Count();
                            for (int j = inicio; j < cantIteracion; j++) {
                                result[i].Add((byte)0);
                            }
                            writer.Write(result[i].ToArray());
                        }
                    }
                }
            }
        }

        public static void Decryption(Key values, IFormFile file, string routeDirectory) {
            if (!Directory.Exists(Path.Combine(routeDirectory, "decryption"))){
                Directory.CreateDirectory(Path.Combine(routeDirectory, "decryption"));
            }

            using (var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "decryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {
                        var waves = (2 * values.Levels) - 2;
                        var bufferLength = 100000;
                        var byteBuffer = new byte[bufferLength];
                        var cantWaves = Convert.ToInt32(reader.BaseStream.Length) / waves;
                        var intermediateValues = (Convert.ToInt32(reader.BaseStream.Length) - (2 * cantWaves)) / (values.Levels - 2);
                        var resoult = new Queue<byte>[values.Levels];
                        var position = 0;
                        var countLevels = 0;
                        var intermediateValue = 0;
                        var direction = true;

                        for (int i = 0; i < values.Levels; i++) {
                            resoult[i] = new Queue<byte>();
                        }

                        while (reader.BaseStream.Position != reader.BaseStream.Length) {
                            byteBuffer = reader.ReadBytes(bufferLength);
                            foreach (var caracter in byteBuffer) {
                                if (countLevels == values.Levels - 1) {
                                    resoult[countLevels].Enqueue(caracter);
                                }else {
                                    if (position < cantWaves) {
                                        resoult[0].Enqueue(caracter);

                                    } else if (position == cantWaves) {
                                        countLevels++;
                                        resoult[countLevels].Enqueue(caracter);
                                        intermediateValue = 1;
                                    } else if (intermediateValue < intermediateValues) {
                                        resoult[countLevels].Enqueue(caracter);
                                        intermediateValue++;
                                    } else {
                                        countLevels++;
                                        resoult[countLevels].Enqueue(caracter);
                                        intermediateValue = 1;
                                    }
                                    position++;
                                }
                            }
                        }

                        countLevels = 0;

                        while (resoult[1].Count() != 0 || (values.Levels == 2 && resoult[1].Count() != 0)) {
                            if (countLevels == 0) {
                                writer.Write(resoult[countLevels].Dequeue());
                                countLevels = 1;
                                direction = true;
                            } else if (countLevels < values.Levels - 1 && direction) {
                                writer.Write(resoult[countLevels].Dequeue());
                                countLevels++;
                            } else if (countLevels > 0 && !direction) {
                                writer.Write(resoult[countLevels].Dequeue());
                                countLevels--;
                            } else if (countLevels == values.Levels - 1) {
                                writer.Write(resoult[countLevels].Dequeue());
                                countLevels = values.Levels - 2;
                                direction = false;
                            }
                        }
                    }
                }
            }
        }

    }
}
