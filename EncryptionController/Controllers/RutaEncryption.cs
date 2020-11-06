using EncryptionController.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EncryptionController.Controllers
{
    public class RutaEncryption {

        public static void Encryption(Key values, IFormFile file, string routeDirectory) {
            if (!Directory.Exists(Path.Combine(routeDirectory, "encryption"))) {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "encryption"));
            }

            using (var reader = new BinaryReader(file.OpenReadStream())){
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "encryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.rt"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {
                        var bufferLength = values.Rows * values.Columns;
                        var byteBuffer = new byte[bufferLength];

                        while (reader.BaseStream.Position != reader.BaseStream.Length) {
                            var matriz = new byte[values.Rows, values.Columns];
                            byteBuffer = reader.ReadBytes(bufferLength);
                            var cont = 0;

                            for (int i = 0; i < values.Columns; i++) {
                                for (int j = 0; j < values.Rows; j++) {
                                    if (cont < byteBuffer.Count()) {
                                        matriz[j, i] = byteBuffer[cont];
                                        cont++;
                                    }
                                }
                            }

                            for (int i = 0; i < values.Rows; i++)
                            {
                                for (int j = 0; j < values.Columns; j++)
                                {
                                    writer.Write(matriz[i, j]);
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

            using (var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "decryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {
                        var bufferLength = values.Rows * values.Columns;
                        var byteBuffer = new byte[bufferLength];

                        while (reader.BaseStream.Position != reader.BaseStream.Length) {
                            var matriz = new byte[values.Rows, values.Columns];
                            byteBuffer = reader.ReadBytes(bufferLength);
                            var cont = 0;

                            for (int i = 0; i < values.Rows; i++)
                            {
                                for (int j = 0; j < values.Columns; j++)
                                {
                                    if (cont < byteBuffer.Count())
                                    {
                                        matriz[i, j] = byteBuffer[cont];
                                        cont++;
                                    }
                                    else
                                    {
                                        matriz[i, j] = (byte)0;
                                    }
                                }
                            }

                            for (int i = 0; i < values.Columns; i++)
                            {
                                for (int j = 0; j < values.Rows; j++)
                                {
                                    if (matriz[j, i] != (byte)0)
                                    {
                                        writer.Write(matriz[j, i]);
                                    }
                                }
                            }
                        }
                    }

                }
            }

        }

    }
}
