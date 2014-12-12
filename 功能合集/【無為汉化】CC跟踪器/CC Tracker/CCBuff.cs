using LeagueSharp;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Tracker
{
    public class CCBuff
    {
        public string Champion { get; set; }
        public string Name { get; set; }
        public Texture ChampIcon { get; set; }
        public Texture BuffIcon { get; set; }

        public CCBuff(string champ, string name, Bitmap champIcon, Bitmap icon)
        {
            this.Champion = champ;
            this.Name = name;
            this.ChampIcon = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(champIcon, typeof(byte[])), 32, 32, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            this.BuffIcon = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(icon, typeof(byte[])), 32, 32, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
        }
    }
}
