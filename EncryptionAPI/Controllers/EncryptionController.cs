using System;
using System.IO;
using System.Net.Mime;
using EncryptionController.Controllers;
using EncryptionController.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace EncryptionAPI.Controllers {

    [Route("api/")]
    [ApiController]
    public class EncryptionController : ControllerBase {

        #region Objects
        private static string routeDirectory = Environment.CurrentDirectory;
        #endregion

        [HttpGet("home")]
        public ActionResult Get() {
            return Ok();
        
        }

        [HttpPost, Route("cipher/{method}")]
        public ActionResult Encrypt(string method, [FromForm] IFormFile file, [FromForm] Key key) {
            switch (method) {
                case "cesar":
                    CesarEncryption.Encryption(new Key { Word = key.Word }, file, routeDirectory);
                    ReturnCesarFile(file);
                    break;
                case "zigzag":
                    ZigZagEncryption.Encryption(new Key { Levels = key.Levels}, file, routeDirectory);
                    //ReturnZigZagFile(file);
                    break;
                case "ruta":
                    RutaEncryption.Encryption(new Key { Rows = key.Rows, Columns = key.Columns}, file, routeDirectory);
                    //ReturnRutaFile(file);
                    break;

            }

          
            
            return Ok();
        }

        [HttpPost("decipher")]
        public ActionResult Decrypt([FromForm] IFormFile file, [FromForm] Key key) {
            switch (Path.GetExtension(file.FileName)) {
                case ".crs":
                    CesarEncryption.Decryption(new Key { Word = key.Word }, file, routeDirectory);
                    //RetunDecryptionCesarFile(file);
                    break;
                case ".zz":
                    ZigZagEncryption.Decryption(new Key { Levels = key.Levels }, file, routeDirectory);
                    //RetunDecryptionZigZagFile(file);
                    break;
                case ".rt":
                    RutaEncryption.Decryption(new Key { Rows = key.Rows, Columns = key.Columns }, file, routeDirectory);
                    //RetunDecryptionRutaFile(file);
                    break;
            }
            return Ok();
        }

        public ActionResult ReturnCesarFile(IFormFile file) {
            return PhysicalFile(Path.Combine(
                routeDirectory, "encrypt", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.csr");
        }

    }
}
