﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionController.Models {
    public class Key {

        public string Word { get; set; }
        public int Levels { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

    }
}
