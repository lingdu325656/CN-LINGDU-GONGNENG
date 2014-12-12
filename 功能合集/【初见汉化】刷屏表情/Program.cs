using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace ASUtility_刷屏表情 {
	class Program {

		private static Menu menu;
		static void Main(string[] args) {
			CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
			Game.OnGameUpdate+=Game_OnGameUpdate;
		
		}


		static void Game_OnGameLoad(EventArgs args) {
			LoadMenu();


		}


		private static void LoadMenu() {
			menu = new Menu("ASUtility", "ASUtility", true);

			Menu EmoteSpamm = new Menu("EmoteSpamm", "EmoteSpamm");
			EmoteSpamm.AddItem(new MenuItem("Active","Active").SetValue<KeyBind>(new KeyBind('P',KeyBindType.Toggle,true)));
			EmoteSpamm.AddItem(new MenuItem("GG", "GG").SetValue<KeyBind>(new KeyBind(37, KeyBindType.Press)));
			EmoteSpamm.AddItem(new MenuItem("YOLO", "YOLO").SetValue<KeyBind>(new KeyBind(39, KeyBindType.Press)));
			EmoteSpamm.AddItem(new MenuItem("PIG", "PIG").SetValue<KeyBind>(new KeyBind(40, KeyBindType.Press)));
			EmoteSpamm.AddItem(new MenuItem("Author", "Author:Asuvril"));
			menu.AddSubMenu(EmoteSpamm);

			menu.AddToMainMenu();
			Game.PrintChat("Asuvril's utility v1.0 Laod. Enjoy it!");
			
		}
		private static void Game_OnGameUpdate(EventArgs args) {
			
			if (menu.SubMenu("EmoteSpamm").Item("Active").GetValue<KeyBind>().Active)
			{
				if (menu.SubMenu("EmoteSpamm").Item("GG").GetValue<KeyBind>().Active)
				{
					Game.Say("/all 聽##聽聽聽聽聽聽聽 ##聽聽聽聽聽##聽聽聽聽聽聽聽##");
					Game.Say("/all 聽聽######聽聽聽聽聽聽聽聽######聽聽");
					Game.Say("/all 聽聽");
					Game.Say("/all 聽聽######聽聽聽聽聽聽聽聽######聽聽聽");
					Game.Say("/all 聽##聽聽聽聽聽聽聽聽聽聽聽聽聽聽聽聽聽 ##聽聽聽聽聽聽聽");
					Game.Say("/all 聽##聽聽聽聽聽聽聽聽聽聽聽聽聽聽聽聽聽聽##聽聽聽聽聽聽聽");
        			Game.Say("/all 聽##聽聽聽####聽聽聽聽聽##聽聽聽####");
        			Game.Say("/all 聽##聽聽聽聽聽聽聽 ##聽聽聽聽聽##聽聽聽聽聽聽聽##");
					
				}
				if (menu.SubMenu("EmoteSpamm").Item("YOLO").GetValue<KeyBind>().Active)
				{

					Game.Say("/all 聽聽聽#聽聽聽聽聽聽聽聽###聽聽聽聽####聽聽###聽聽聽聽聽聽##");
					Game.Say("/all 聽聽");
					Game.Say("/all 聽聽");
					Game.Say("/all #聽聽聽聽##聽聽聽###聽聽聽聽#聽聽聽聽聽聽聽聽聽###聽聽聽聽聽聽##");
					Game.Say("/all 聽#聽聽##聽聽#聽聽聽聽##聽聽#聽聽聽聽聽聽聽#聽聽聽聽##聽聽聽聽##");
					Game.Say("/all 聽聽###聽聽聽#聽聽聽聽##聽聽#聽聽聽聽聽聽聽#聽聽聽聽##聽聽聽聽##");
					Game.Say("/all 聽聽聽#聽聽聽聽聽聽#聽聽聽聽##聽聽#聽聽聽聽聽聽聽#聽聽聽聽##聽聽聽聽");
					Game.Say("/all 聽聽聽#聽聽聽聽聽聽#聽聽聽聽##聽聽#聽聽聽聽聽聽聽#聽聽聽聽##聽聽聽聽##");
					
				}
				if (menu.SubMenu("EmoteSpamm").Item("PIG").GetValue<KeyBind>().Active)
				{
			
					Game.Say("/all 聽聽");
					Game.Say("/all    _     _      _     _     _ ");
					Game.Say("/all   ().-.()   GL - HF   ().-.()  ");
					Game.Say("/all    _     _      _     _     _ ");
					Game.Say("/all 聽聽");
				}
			}
		}

		
	}
}
