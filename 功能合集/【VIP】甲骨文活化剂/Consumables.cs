using System;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Consumables
    {
        private static Menu mainmenu;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            mainmenu = new Menu("消耗品", "imenu");
			CreateMenuItem("蓝药", "Mana", 40, 0);
            CreateMenuItem("饼干", "Biscuit", 40, 25, true, true);
            CreateMenuItem("红药", "Health", 40, 25, false, true);
            CreateMenuItem("水晶瓶", "Flask", 40, 35, true, true);

            root.AddSubMenu(mainmenu);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("FlaskOfCrystalWater", 2004, "Mana", 0, 0, false);
            UseItem("ItemMiniRegenPotion", 2010, "Biscuit", OC.IncomeDamage, OC.MinionDamage);
            UseItem("ItemCrystalFlask", 2041, "Flask", OC.IncomeDamage, OC.MinionDamage);
            UseItem("RegenerationPotion", 2003, "Health", OC.IncomeDamage, OC.MinionDamage, true, false);
        }

        private static void UseItem(string name, int itemId, string menuvar, float incdmg = 0, float mindmg = 0, bool usehealth = true, bool usemana = true)
        {
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;
            
            if (!me.HasBuff(name, true) && me.NotRecalling() && !Utility.InFountain())
            {
                if (!mainmenu.Item("use" + menuvar).GetValue<bool>())
                    return;

                var aHealthPercent = (int) ((me.Health/me.MaxHealth)*100);
                var aManaPercent = (int) ((me.Mana/me.MaxMana)*100);
                var iDamagePercent = (int) ((incdmg/me.MaxHealth)*100);
                var mDamagePercent = (int) ((mindmg/me.MaxHealth)*100);

                if (usemana && aManaPercent <= mainmenu.Item("use" + menuvar + "Mana").GetValue<Slider>().Value)
                    Items.UseItem(itemId);

                if (usehealth && aHealthPercent <= mainmenu.Item("use" + menuvar + "Pct").GetValue<Slider>().Value)
                {
                    if (iDamagePercent >= 1 || incdmg >= me.Health || me.HasBuff("summonerdot") ||
                        mDamagePercent >= 1 || mindmg >= me.Health)
                    {
                        if (OC.AggroTarget.NetworkId == me.NetworkId)
                            Items.UseItem(itemId);
                    }

                    else if (iDamagePercent >= mainmenu.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
                    {
                        if (OC.AggroTarget.NetworkId == me.NetworkId)
                            Items.UseItem(itemId);
                    }
                }
            }
        }

        private static void CreateMenuItem(string name, string menuvar, int dvalue, int dmgvalue, bool usemana = true, bool usehealth = false)
        {
            var menuName = new Menu(name, "m" + menuvar);
            menuName.AddItem(new MenuItem("use" + menuvar, "使用" + name)).SetValue(true);
            if (usehealth)
            {
                menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "血量")).SetValue(new Slider(dvalue));
                menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "伤害")).SetValue(new Slider(dmgvalue));
            }
            if (usemana)
                menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "蓝量")).SetValue(new Slider(40));
            mainmenu.AddSubMenu(menuName);
        }
    }
}