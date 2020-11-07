using System;
using System.IO;
using System.Net.Mime;
using EncryptionController.Controllers;
using EncryptionController.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            int code = 0;
            switch (method) {
                case "cesar":
                    CesarEncryption.Encryption(new Key { Word = key.Word }, file, routeDirectory);
                    code = 1;
                    break;
                case "zigzag":
                    ZigZagEncryption.Encryption(new Key { Levels = key.Levels }, file, routeDirectory);
                    code = 2;
                    break;
                case "ruta":
                    RutaEncryption.Encryption(new Key { Rows = key.Rows, Columns = key.Columns }, file, routeDirectory);
                    code = 3;
                    break;

            }
            return ReturnFileEncrypt(file, code);
        }

        [HttpPost("decipher")]
        public ActionResult Decrypt([FromForm] IFormFile file, [FromForm] Key key) {
            int code = 0;
            switch (Path.GetExtension(file.FileName)) {
                case ".crs":
                    CesarEncryption.Decryption(new Key { Word = key.Word }, file, routeDirectory);
                    code = 1;
                    break;
                case ".zz":
                    ZigZagEncryption.Decryption(new Key { Levels = key.Levels }, file, routeDirectory);
                    code = 2;
                    break;
                case ".rt":
                    RutaEncryption.Decryption(new Key { Rows = key.Rows, Columns = key.Columns }, file, routeDirectory);
                    code = 3;
                    break;
            }
            return ReturnFileDecrypt(file, code);
        }

        public ActionResult ReturnFileEncrypt(IFormFile file, int code) {
            if (code == 1) {
                return PhysicalFile(Path.Combine(
               routeDirectory, "encryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.crs"), MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.crs");

            }else if (code == 2){
                return PhysicalFile(Path.Combine(
               routeDirectory, "encryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.zz"), MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.zz");

            }else if(code == 3) {
                return PhysicalFile(Path.Combine(
               routeDirectory, "encryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.rt"), MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.rt");

            }else {
                return StatusCode(500, "InternalServerError");
            }
           
        }

        public ActionResult ReturnFileDecrypt(IFormFile file, int code) {
            if (code == 1) {
                return PhysicalFile(Path.Combine(
               routeDirectory, "decryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt");

            } else if (code == 2) {
                return PhysicalFile(Path.Combine(
               routeDirectory, "decryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt");

            } else if (code == 3) {
                return PhysicalFile(Path.Combine(
               routeDirectory, "decryption", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt");

            } else {
                return StatusCode(500, "InternalServerError");
            }
        }


    }
}
