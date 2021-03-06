﻿using System.Drawing;
using HelpersLib.GraphicsHelper;
using ImageEffects.IPlugin;

namespace ImageManipulation
{
    public class Resize : IPluginItem
    {
        public override string Name { get { return "Resize"; } }

        public override string Description { get { return "Resize"; } }

        private Size size;

        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                OnPreviewTextChanged(size.ToString());
            }
        }

        public Resize()
        {
            size = new Size(200, 200);
        }

        public override Image ApplyEffect(Image img)
        {
            return Core.ChangeImageSize(img, size.Width, size.Height);
        }
    }
}