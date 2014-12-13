using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class AutoSpells
    {

        private static Menu mainmenu, menuconfig;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            mainmenu = new Menu("自动技能", "asmenu");
            menuconfig = new Menu("设置", "asconfig");

            // auto shields
            CreateMenuItem(95, "BraumE", "Unbreakable", "braumshield", SpellSlot.E);
            CreateMenuItem(95, "DianaOrbs", "Pale Cascade", "dianashield", SpellSlot.W);
            CreateMenuItem(95, "GalioBulwark", "Bulwark", "galioshield", SpellSlot.W);
            CreateMenuItem(95, "GarenW", "Courage", "garenshield", SpellSlot.W, false);
            CreateMenuItem(95, "EyeOfTheStorm", "Eye of the Storm", "jannashield", SpellSlot.E);
            CreateMenuItem(95, "KarmaSolKimShield", "Inspire", "karmashield", SpellSlot.E);
            CreateMenuItem(95, "LuluE", "Help Pix!", "lulushield", SpellSlot.E);
            CreateMenuItem(95, "LuxPrismaticWave", "Prismatic Barrier", "luxshield", SpellSlot.W);
            CreateMenuItem(95, "NautilusPiercingGaze", "Titans Wraith", "nautshield", SpellSlot.W);
            CreateMenuItem(95, "OrianaRedactCommand", "Command Protect", "oriannashield", SpellSlot.E);
            CreateMenuItem(95, "ShenFeint", "Feint", "shenshield", SpellSlot.W, false);
            CreateMenuItem(95, "MoltenShield", "Molten Shield", "annieshield", SpellSlot.E);
            CreateMenuItem(95, "JarvanIVGoldenAegis", "Golden Aegis", "j4shield", SpellSlot.W);
            CreateMenuItem(95, "BlindMonkWOne", "Safegaurd", "leeshield", SpellSlot.W, false);
            CreateMenuItem(95, "RivenFeint", "Valor", "rivenshield", SpellSlot.E, false);
            CreateMenuItem(95, "FioraRiposte", "Riposte", "fiorashield", SpellSlot.W, false);
            CreateMenuItem(95, "RumbleShield", "Scrap Shield", "rumbleshield", SpellSlot.W, false);
            CreateMenuItem(95, "SionW", "Soul Furnace", "sionshield", SpellSlot.W);
            CreateMenuItem(95, "SkarnerExoskeleton", "Exoskeleton", "skarnershield", SpellSlot.W);
            CreateMenuItem(95, "UrgotTerrorCapacitorActive2", "Terror Capacitor", "urgotshield", SpellSlot.W);
            CreateMenuItem(95, "Obduracy", "Brutal Strikes", "malphshield", SpellSlot.W);
            CreateMenuItem(95, "DefensiveBallCurl", "Defensive Ball Curl", "rammusshield", SpellSlot.W);

            // auto heals
            CreateMenuItem(80, "TriumphantRoar", "Triumphant Roar", "alistarheal", SpellSlot.E);
            CreateMenuItem(80, "PrimalSurge", "Primal Surge", "nidaleeheal", SpellSlot.E);
            CreateMenuItem(80, "RemoveScurvy", "Remove Scurvy", "gangplankheal", SpellSlot.W);
            CreateMenuItem(80, "JudicatorDivineBlessing", "Divine Blessing", "kayleheal", SpellSlot.W);
            CreateMenuItem(80, "NamiE", "Ebb and Flow", "namiheal", SpellSlot.W);
            CreateMenuItem(80, "SonaW", "Aria of Perseverance", "sonaheal", SpellSlot.W);
            CreateMenuItem(80, "SorakaW", "Astral Infusion", "sorakaheal", SpellSlot.W, false);
            CreateMenuItem(80, "Imbue", "Imbue", "taricheal", SpellSlot.Q);

            // auto ultimates
            CreateMenuItem(35, "LuluR", "Wild Growth", "luluult", SpellSlot.R);
            CreateMenuItem(20, "UndyingRage", "Undying Rage", "tryndult", SpellSlot.R, false);
            CreateMenuItem(20, "ChronoShift", "Chorno Shift", "zilult", SpellSlot.R);
            CreateMenuItem(20, "YorickReviveAlly", "Omen of Death", "yorickult", SpellSlot.R);

            // slow removers
            CreateMenuItem(0, "EvelynnW", "Draw Frenzy", "eveslow", SpellSlot.W, false);
            CreateMenuItem(0, "GarenQ", "Decisive Strike", "garenslow", SpellSlot.Q, false);

            // auto zhonya skills
            //CreateMenuItem(0, "FioraDance", "Blade Waltz", "fioradodge", SpellSlot.R, false);

            root.AddSubMenu(mainmenu);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {     
            if (me.HasBuffOfType(BuffType.Slow))
            {
                // slow removals
                UseSpell("GarenQ", "garenslow", float.MaxValue, false);
                UseSpell("EvelynnW", "eveslow", float.MaxValue, false);
            }

            // auto heals
            UseSpell("TriumphantRoar", "alistarheal", 575f);
            UseSpell("PrimalSurge", "nidaleeheal", 600f);
            UseSpell("RemoveScurvy", "gangplankheal");
            UseSpell("JudicatorDivineBlessing", "kayleheal", 900f);
            UseSpell("NamiE", "namiheal", 725f);
            UseSpell("SonaW", "sonaheal", 1000f);
            UseSpell("SorakaW", "sorakaheal",450f, false);
            UseSpell("Imbue", "taricheal", 750f);

            if (OC.IncomeDamage < 1)
                return;
            // auto shields
            UseSpell("BraumE", "braumshield");
            UseSpell("DianaOrbs", "dianashield");
            UseSpell("GalioBulwark", "galioshield", 800f);
            UseSpell("GarenW", "garenshield", float.MaxValue, false);
            UseSpell("EyeOfTheStorm", "jannashield", 800f);
            UseSpell("KarmaSolKimShield", "karmashield", 800f);
            UseSpell("LuxPrismaticWave", "luxshield", 1075f);
            UseSpell("NautilusPiercingGaze", "nautshield");
            UseSpell("OrianaRedactCommand", "oriannashield", 1100f);
            UseSpell("ShenFeint", "shenshield", float.MaxValue, false);
            UseSpell("JarvanIVGoldenAegis", "j4shield");
            UseSpell("BlindMonkWOne", "leeshield", 700f, false);
            UseSpell("RivenFeint", "rivenshield", float.MaxValue, false);
            UseSpell("RumbleShield", "rumbleshield");
            UseSpell("SionW", "sionshield");
            UseSpell("SkarnerExoskeleton", "skarnershield");
            UseSpell("UrgotTerrorCapacitorActive2", "urgotshield");
            UseSpell("MoltenShield", "annieshield");
            UseSpell("FioraRiposte", "fiorashield", float.MaxValue, false);
            UseSpell("Obduracy", "malphshield");
            UseSpell("DefensiveBallCurl", "rammusshield");

            // auto ults
            UseSpell("LuluR", "luluult", 900f, false);
            UseSpell("UndyingRage", "tryndult", float.MaxValue, false);
            UseSpell("ChronoShift", "zilult", 900f, false);
            UseSpell("YorickReviveAlly", "yorickult", 900f, false);

            // auto zhonya skills
            //UseSpell("FioraDance", "fioradodge", OC.IncomeDamage, 300f, false);
        }

        private static void UseSpell(string sdataname, string menuvar, float range = float.MaxValue, bool usemana = true)
        {
            var slot = me.GetSpellSlot(sdataname);
            if (slot == SpellSlot.Unknown)
                return;

            if (slot != SpellSlot.Unknown && !mainmenu.Item("use" + menuvar).GetValue<bool>())
                return;

            var spell = new Spell(slot, range);
            if (!spell.IsReady())
                return;

            var allyuse = range.ToString() != float.MaxValue.ToString();
            var target = allyuse ? OC.FriendlyTarget() : me;

            if (!menuconfig.Item("ason" + target.SkinName).GetValue<bool>())
                return;

            if (target.Distance(me.Position) > range) 
                return;

            var incdmg = OC.IncomeDamage;

            var aManaPercent = (int) ((me.Mana/me.MaxMana)*100);
            var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
            var iDamagePercent = (int) ((incdmg/target.MaxHealth)*100);

            if (OC.AggroTarget.Distance(me.Position) > spell.Range || !me.NotRecalling())
                return;

            if (aHealthPercent <= mainmenu.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && menuvar.Contains("shield"))
            {
                if (usemana && aManaPercent <= mainmenu.Item("use" + menuvar + "Mana").GetValue<Slider>().Value)
                    return;

                if (me.SkinName == "Soraka" && (me.Health/me.MaxHealth*100 <= mainmenu.Item("useSorakaMana").GetValue<Slider>().Value || target.IsMe))
                    return;

                if ((iDamagePercent >= 1 || incdmg >= target.Health) && OC.AggroTarget.NetworkId == target.NetworkId)
                {
                    if (menuvar == "luxshield" || menuvar == "rivenshield")
                    {
                        var po = spell.GetPrediction(target);
                        if (po.Hitchance >= HitChance.Medium && !target.IsMe)
                            spell.Cast(po.CastPosition);
                        else
                        {
                            spell.Cast(Game.CursorPos);
                        }
                    }
                    else
                    {
                        spell.Cast(target);
                    }
                }
            }
            else if (aHealthPercent <= mainmenu.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && menuvar.Contains("heal"))
            {
                if (me.SkinName == "Soraka" && (int)(me.Health/me.MaxHealth*100) <= mainmenu.Item("useSorakaMana").GetValue<Slider>().Value)
                    return;

                if (aManaPercent >= mainmenu.Item("use" + menuvar + "Mana").GetValue<Slider>().Value && usemana)
                    spell.Cast(target);
            }
            else if (iDamagePercent >= mainmenu.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
            {
                spell.Cast(target);
            }
        }

        private static void CreateMenuItem(int defaultvalue, string sdataname, string displayname, string menuvar, SpellSlot slot, bool usemana = true)
        {
            var champslot = me.GetSpellSlot(sdataname);
            if (champslot != SpellSlot.Unknown && champslot == slot)
            {
                foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                    menuconfig.AddItem(new MenuItem("ason" + x.SkinName, "用于" + x.SkinName)).SetValue(true);
                mainmenu.AddSubMenu(menuconfig);

                var menuName = new Menu(displayname, menuvar.ToLower());
                menuName.AddItem(new MenuItem("use" + menuvar, "用于" + displayname)).SetValue(true);
                if (menuvar.Contains("slow"))
                    menuName.AddItem(new MenuItem("use" + menuvar + "Slow", "移除减速").SetValue(true));
                if (!menuvar.Contains("slow"))
                    menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "血量")).SetValue(new Slider(defaultvalue));
                if (!menuvar.Contains("ult") || !menuvar.Contains("slow"))
                    menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "伤害")).SetValue(new Slider(45));
                if (menuvar.Contains("soraka"))
                    menuName.AddItem(new MenuItem("useSorakaMana", "最小血量")).SetValue(new Slider(35));
                if (usemana)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "最小蓝量")).SetValue(new Slider(45));
                mainmenu.AddSubMenu(menuName);
            }
        }
    }
}