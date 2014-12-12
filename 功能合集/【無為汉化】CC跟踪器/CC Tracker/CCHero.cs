using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Tracker
{
    public class CCHero
    {
        public string Name { get; set; }
        public  CCBuff CCBuff { get; set; }
        public CCSpell CCSpells { get; set; }
        public Texture Icon { get; set; }
        public CCHero(string name, CCBuff ccBuff, CCSpell ccSpells, Texture icon)
        {
            this.Name = name;
            this.CCBuff = ccBuff;
            this.CCSpells = ccSpells;
            this.Icon = icon;
        }
    }
}
