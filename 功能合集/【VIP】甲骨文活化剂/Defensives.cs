using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Defensives
    {
        private static int bufftick, bufftime;
        private static bool danger, stealth;
        private static Menu mainmenu, menuconfig;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            mainmenu = new Menu("防御物品", "dmenu");
            menuconfig = new Menu("设置", "dconfig");

            foreach (Obj_AI_Hero x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                menuconfig.AddItem(new MenuItem("DefenseOn" + x.SkinName, "用于" + x.SkinName)).SetValue(true);
            mainmenu.AddSubMenu(menuconfig);

            CreateMenuItem("兰顿", "兰顿", 40, 40, true);
            CreateMenuItem("大天使", "大天使", 55, 40);
            CreateMenuItem("中亚", "中亚", 35, 40);
            CreateMenuItem("山岳之容", "山岳之容", 20, 40);
            CreateMenuItem("鸟盾", "鸟盾", 45, 40);

            var tMenu = new Menu("升华护符", "tboost");
            tMenu.AddItem(new MenuItem("useTalisman", "对落后的人使用")).SetValue(true);
            tMenu.AddItem(new MenuItem("useAllyPct", "队友血量")).SetValue(new Slider(50, 1));
            tMenu.AddItem(new MenuItem("useEnemyPct", "敌人血量")).SetValue(new Slider(50, 1));
            tMenu.AddItem(new MenuItem("talismanMode", "模式 ")).SetValue(new StringList(new[] { "总是", "连招时" }));
            mainmenu.AddSubMenu(tMenu);

            var bMenu = new Menu("号令之旗", "bannerc");
            bMenu.AddItem(new MenuItem("useBanner", "使用")).SetValue(true);
            mainmenu.AddSubMenu(bMenu);

            var oMenu = new Menu("扫描透镜", "olens");
            oMenu.AddItem(new MenuItem("useOracles", "对隐形使用")).SetValue(true);
            oMenu.AddItem(new MenuItem("oracleMode", "模式")).SetValue(new StringList(new[] { "总是", "连招时" }));
            mainmenu.AddSubMenu(oMenu);

            if (Game.MapId == GameMapId.CrystalScar)
            {
                CreateMenuItem("沃格勒特的巫师帽", "沃格勒特的巫师帽", 35, 40); // not SR
                CreateMenuItem("奥戴恩的面纱", "奥戴恩的面纱", 40, 40, true); // not SR
            }

            root.AddSubMenu(mainmenu);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {

            // Oracle's Lens
            if (Items.HasItem(3364) && Items.CanUseItem(3364) && mainmenu.Item("useOracles").GetValue<bool>())
            {
                if (!Items.CanUseItem(3364))
                    return;

                if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                    mainmenu.Item("oracleMode").GetValue<StringList>().SelectedIndex == 1)
                    return;

                if (!stealth)
                    return;

                var target = OC.FriendlyTarget();
                if (target.Distance(me.Position) <= 600f && stealth || target.HasBuff("RengarRBuff", true))
                    Items.UseItem(3364, target.Position);
            }

            // Banner of command (basic)
            if (Items.HasItem(3060) && Items.CanUseItem(3060) && mainmenu.Item("useBanner").GetValue<bool>())
            {
                List<Obj_AI_Base> minionList = MinionManager.GetMinions(me.Position, 1000);
                if (!minionList.Any())
                    return;

                foreach (Obj_AI_Base minyone in minionList.Where(
                    minion => minion.IsValidTarget(1000) && 
                    minion.BaseSkinName.Contains("MechCannon")))
                {
                    Items.UseItem(3060, minyone);
                }
            }

           // Talisman of Ascension
            if (Items.HasItem(3069) && Items.CanUseItem(3069) && mainmenu.Item("useTalisman").GetValue<bool>())
            {
                if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                    mainmenu.Item("talismanMode").GetValue<StringList>().SelectedIndex == 1)
                    return;

                var target = OC.FriendlyTarget();
                if (target.Distance(me.Position) > 600)
                    return;

                var enemies = target.CountHerosInRange(true, 1000);
                var allies = target.CountHerosInRange(false, 1000);

                var weakEnemy =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .OrderByDescending(ex => ex.Health/ex.MaxHealth*100)
                        .First(x => x.IsValidTarget(1000));

                var aHealthPercent = target.Health / target.MaxHealth * 100;
                var eHealthPercent = weakEnemy.Health / weakEnemy.MaxHealth * 100;

                if (weakEnemy.Distance(target.Position) <= 900 &&
                    (allies > enemies && eHealthPercent <= mainmenu.Item("useEnemyPct").GetValue<Slider>().Value))
                {
                    Items.UseItem(3069);
                }

                if (enemies > allies && aHealthPercent <= mainmenu.Item("useAllyPct").GetValue<Slider>().Value)
                {
                    Items.UseItem(3069);
                }
            }

            // Deffensives
            if (OC.FriendlyTarget() == null) 
                return;
            if (OC.IncomeDamage < 1) 
                return;

            UseItem("鸟盾", 3190, 600f, OC.IncomeDamage);
            UseItem("大天使", 3040, 450f, OC.IncomeDamage, true);
            UseItem("中亚", 3157, 450f, OC.IncomeDamage, true);
            UseItem("兰顿", 3143, 450f, OC.IncomeDamage);
            UseItem("山岳之容", 3401, 700f, OC.IncomeDamage, false, true);

            if (Game.MapId != GameMapId.CrystalScar)
                return;

            UseItem("奥戴恩的面纱", 3180, 450f, OC.IncomeDamage);
            UseItem("沃格勒特的巫师帽", 3090, 450f, OC.IncomeDamage, true);
        }

        private static void UseItem(string name, int itemId, float itemRange, float incdmg = 0, bool selfuse = false, bool targeted = false)
        {
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            if (!mainmenu.Item("use" + name).GetValue<bool>())
                return;
        
            var target = selfuse ? me : OC.FriendlyTarget();
            if (target.Distance(me.Position) > itemRange)
                return;

            var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
            var iDamagePercent = (int) (incdmg/target.MaxHealth*100);

            if (me.NotRecalling() && mainmenu.Item("DefenseOn" + target.SkinName).GetValue<bool>())
            {
                if (aHealthPercent <= mainmenu.Item("use" + name + "Pct").GetValue<Slider>().Value)
                {
                    if ((iDamagePercent >= 1 || incdmg >= target.Health) && 
                        OC.AggroTarget.NetworkId == target.NetworkId)
                            Items.UseItem(itemId, targeted ? target : null);

                    if (iDamagePercent >= mainmenu.Item("use" + name + "Dmg").GetValue<Slider>().Value && 
                        OC.AggroTarget.NetworkId == target.NetworkId)
                            Items.UseItem(itemId, targeted ? target : null);
                }

                if (mainmenu.Item("use" + name + "Danger").GetValue<bool>())
                    if (danger && OC.AggroTarget.NetworkId == target.NetworkId)
                        Items.UseItem(itemId, targeted ? target : null);             
            }
        }

        private static void CreateMenuItem(string displayname, string name, int hpvalue, int dmgvalue, bool itemcount = false)
        {
            var menuName = new Menu(displayname, name.ToLower());
            menuName.AddItem(new MenuItem("use" + name, "使用 " + name)).SetValue(true);
            menuName.AddItem(new MenuItem("use" + name + "Pct", "血量")).SetValue(new Slider(hpvalue));
            if (!itemcount)
                menuName.AddItem(new MenuItem("use" + name + "Dmg", "伤害")).SetValue(new Slider(dmgvalue));
            if (itemcount)
                menuName.AddItem(new MenuItem("use" + name + "Count", "使用计算")).SetValue(new Slider(3, 1, 5));
            menuName.AddItem(new MenuItem("use" + name + "Danger", "危险时使用")).SetValue(true);
            mainmenu.AddSubMenu(menuName);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || sender.Type != me.Type)
                return;

            var target = OC.FriendlyTarget();
            var attacker = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
            var attackerslot = attacker.GetSpellSlot(args.SData.Name);

            stealth = false; danger = false;
            foreach (var data in OracleLib.Database.Where(data => sender.SkinName == data.Name))
            {
                if (target.Distance(attacker.Position) > data.Range)
                    return;

                if (data.DangerLevel == RiskLevel.Stealth)
                    stealth = true;

                if (data.DangerLevel == RiskLevel.Extreme && attackerslot == SpellSlot.R)
                    danger = true;
            }
        }       
    }
}