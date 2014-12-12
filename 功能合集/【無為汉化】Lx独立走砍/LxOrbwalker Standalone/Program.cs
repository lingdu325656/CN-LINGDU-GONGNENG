using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LX_Orbwalker;

namespace LxOrbwalker_Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }


        public static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color='#FF0000'>銆愮劇鐐烘眽鍖栥€慙x鐙珛璧扮爫</font> loaded. - <font color='#5882FA'>E2Slayer</font>");
            var menu = new Menu("【無為汉化】Lx独立走砍", "my_mainmenu", true);
            var orbwalkerMenu = new Menu("Lx走砍", "my_Orbwalker");
            LXOrbwalker.AddToMenu(orbwalkerMenu);
            menu.AddSubMenu(orbwalkerMenu);
            menu.AddToMainMenu();
        }

    }
}
