using EncryptionController.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace EncryptionController.Controllers {
    public class ZigZagEncryption {

        public static void Encryption (Key values, IFormFile file, string routeDirectory) {
            using (var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "encryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.zz"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {

                        var GrupoOlas = (2 * values.Levels) - 2;
                        var len = (float)reader.BaseStream.Length / (float)GrupoOlas;
                        var cantOlas = len % 1 <= 0.5 ? Math.Round(len) + 1 : Math.Round(len);
                        cantOlas = Convert.ToInt32(cantOlas);

                        var pos = 0;
                        var contNivel = 0;

                        var mensaje = new List<byte>[values.Levels];

                        for (int i = 0; i < values.Levels; i++) {
                            mensaje[i] = new List<byte>();
                        }

                        var bufferLength = 100000;
                        var byteBuffer = new byte[bufferLength];

                        while (reader.BaseStream.Position != reader.BaseStream.Length) {
                            byteBuffer = reader.ReadBytes(bufferLength);
                            foreach (var caracter in byteBuffer) {
                                if (pos == 0 || pos % GrupoOlas == 0) {
                                    mensaje[0].Add(caracter);
                                    contNivel = 0;
                                } else if (pos % GrupoOlas ==  - 1) {
                                    mensaje[values.Levels - 1].Add(caracter);
                                    contNivel = values.Levels - 1;
                                }
                                else if (pos % GrupoOlas < values.Levels - 1)
                                {
                                    contNivel++;
                                    mensaje[contNivel].Add(caracter);
                                } else if (pos % GrupoOlas > values.Levels - 1) {
                                    contNivel--;
                                    mensaje[contNivel].Add(caracter);
                                }
                                pos++;
                            }
                        }

                        for (int i = 0; i < values.Levels; i++) {
                            var cantIteracion = i == 0 || i == values.Levels - 1 ? cantOlas : cantOlas * 2;
                            var inicio = mensaje[i].Count();
                            for (int j = inicio; j < cantIteracion; j++)
                            {
                                mensaje[i].Add((byte)0);
                            }
                            writer.Write(mensaje[i].ToArray());
                        }
                    }
                }
            }
        }

        public static void Decryption(Key values, IFormFile file, string routeDirectory) {
            using (var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "decryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {
                        var GrupoOlas = (2 * values.Levels) - 2;
                        var cantOlas = Convert.ToInt32(reader.BaseStream.Length) / GrupoOlas;
                        var intermedios = (Convert.ToInt32(reader.BaseStream.Length) - (2 * cantOlas)) / (values.Levels - 2);

                        var pos = 0;
                        var contNivel = 0;
                        var contIntermedio = 0;

                        var mensaje = new Queue<byte>[values.Levels];

                        for (int i = 0; i < values.Levels; i++) {
                            mensaje[i] = new Queue<byte>();
                        }

                        var bufferLength = 100000;
                        var byteBuffer = new byte[bufferLength];

                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBuffer = reader.ReadBytes(bufferLength);
                            foreach (var caracter in byteBuffer)
                            {
                                if (contNivel == values.Levels - 1)
                                {
                                    mensaje[contNivel].Enqueue(caracter);
                                }
                                else
                                {
                                    if (pos < cantOlas)
                                    {
                                        mensaje[0].Enqueue(caracter);
                                    }
                                    else if (pos == cantOlas)
                                    {
                                        contNivel++;
                                        mensaje[contNivel].Enqueue(caracter);
                                        contIntermedio = 1;
                                    }
                                    else if (contIntermedio < intermedios)
                                    {
                                        mensaje[contNivel].Enqueue(caracter);
                                        contIntermedio++;
                                    }
                                    else
                                    {
                                        contNivel++;
                                        mensaje[contNivel].Enqueue(caracter);
                                        contIntermedio = 1;
                                    }
                                    pos++;
                                }
                            }
                        }

                        contNivel = 0;
                        var direccion = true;
                        //True es hacia abajo
                        //False es hacia arriba

                        while (mensaje[1].Count() != 0 || (values.Levels == 2 && mensaje[1].Count() != 0))
                        {
                            if (contNivel == 0)
                            {
                                writer.Write(mensaje[contNivel].Dequeue());
                                contNivel = 1;
                                direccion = true;
                            }
                            else if (contNivel < values.Levels - 1 && direccion)
                            {
                                writer.Write(mensaje[contNivel].Dequeue());
                                contNivel++;
                            }
                            else if (contNivel > 0 && !direccion)
                            {
                                writer.Write(mensaje[contNivel].Dequeue());
                                contNivel--;
                            }
                            else if (contNivel == values.Levels - 1)
                            {
                                writer.Write(mensaje[contNivel].Dequeue());
                                contNivel = values.Levels - 2;
                                direccion = false;
                            }
                        }
                    }
                }
            }
        }

    }
}
