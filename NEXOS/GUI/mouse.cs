﻿using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.HAL;
using Cosmos.System.Graphics;
using System.Drawing;
using IL2CPU.API.Attribs;
using NEXOS.GUI;

namespace NEXOS.GUI
{
    class Mouse
    {
        public readonly Canvas _canvas;
        public Bitmap _cursor;
        [ManifestResourceStream(ResourceName = "NEXOS.GUI.Resources.cursor.bmp")]
        static byte[] cursorbyte;

        public Mouse(Canvas canvas)
        {
            _canvas = canvas;
            _cursor = new Bitmap(cursorbyte);

        }
        public void DrawCursor()
        {
            Sys.MouseManager.ScreenWidth = (uint)_canvas.Mode.Columns;
            Sys.MouseManager.ScreenHeight = (uint)_canvas.Mode.Rows;

            int X = (int)Sys.MouseManager.X;
            int Y = (int)Sys.MouseManager.Y;

            _canvas.DrawImageAlpha(_cursor, X, Y);
        }
    }
}
