using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using System.Drawing;
using SharpDX.Direct3D9;

namespace CC_Tracker
{
    public class CCSpell
    {
        public string Champion { get; set; }
        public Texture Icon { get; set; }
        public SpellSlot[] SpellSlots { get; set; }

        public CCSpell(string champ, Bitmap icon, params SpellSlot[] slots)
        {
            this.Champion = champ;
            this.Icon = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(icon, typeof(byte[])), 32, 32, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            this.SpellSlots = slots;
        }
    }
}
