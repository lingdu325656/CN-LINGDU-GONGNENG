using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;
using SharpDX;
using Color = System.Drawing.Color;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Summoners
    {
        private static bool isjungling;
        private static string smiteslot;
        private static Menu mainmenu, menuconfig;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            mainmenu = new Menu("召唤师技能", "summoners");
            menuconfig = new Menu("设置", "sconfig");
            isjungling = OracleLib.SmiteAll.Any(Items.HasItem);

            foreach (Obj_AI_Hero x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                menuconfig.AddItem(new MenuItem("suseOn" + x.SkinName, "用于 " + x.SkinName)).SetValue(true);
            mainmenu.AddSubMenu(menuconfig);

            var smite = me.GetSpellSlot("summonersmite");
            if (smite != SpellSlot.Unknown || isjungling)
            {
                var Smite = new Menu("惩戒", "msmite");
                Smite.AddItem(new MenuItem("useSmite", "使用惩戒"))
                    .SetValue(new KeyBind(77, KeyBindType.Toggle, true));
                Smite.AddItem(new MenuItem("SmiteSpell", "使用惩戒+技能")).SetValue(true);
                Smite.AddItem(new MenuItem("drawSmite", "显示惩戒范围")).SetValue(true);
                Smite.AddItem(new MenuItem("smiteSmall", "惩戒小怪")).SetValue(true);
                Smite.AddItem(new MenuItem("smiteLarge", "惩戒大怪")).SetValue(true);
                Smite.AddItem(new MenuItem("smiteEpic", "惩戒红蓝大小龙")).SetValue(true);
                mainmenu.AddSubMenu(Smite);
            }
            var ignite = me.GetSpellSlot("summonerdot");
            if (ignite != SpellSlot.Unknown)
            {
                var Ignite = new Menu("点燃", "mignite");
                Ignite.AddItem(new MenuItem("useIgnite", "使用点燃")).SetValue(true);
                Ignite.AddItem(new MenuItem("dotMode", "模式 ")).SetValue(new StringList(new[] { "抢人头", "连招时" }, 1));
                mainmenu.AddSubMenu(Ignite);
            }
            var heal = me.GetSpellSlot("summonerheal");
            if (heal != SpellSlot.Unknown)
            {
                var Heal = new Menu("治疗", "mheal");
                Heal.AddItem(new MenuItem("useHeal", "使用治疗")).SetValue(true);
                Heal.AddItem(new MenuItem("useHealPct", "血量")).SetValue(new Slider(35, 1));
                Heal.AddItem(new MenuItem("useHealDmg", "伤害")).SetValue(new Slider(40, 1));
                mainmenu.AddSubMenu(Heal);
            }
            var clarity = me.GetSpellSlot("summonermana");
            if (clarity != SpellSlot.Unknown)
            {
                var Clarity = new Menu("清晰术", "mclarity");
                Clarity.AddItem(new MenuItem("useClarity", "使用清晰术")).SetValue(true);
                Clarity.AddItem(new MenuItem("useClarityPct", "蓝量")).SetValue(new Slider(40, 1));
                mainmenu.AddSubMenu(Clarity);
            }
            var barrier = me.GetSpellSlot("summonerbarrier");
            if (barrier != SpellSlot.Unknown)
            {
                var Barrier = new Menu("屏障", "mbarrier");
                Barrier.AddItem(new MenuItem("useBarrier", "使用屏障")).SetValue(true);
                Barrier.AddItem(new MenuItem("useBarrierPct", "血量")).SetValue(new Slider(35, 1));
                Barrier.AddItem(new MenuItem("useBarrierDmg", "伤害")).SetValue(new Slider(40, 1));
                mainmenu.AddSubMenu(Barrier);
            }
            var exhaust = me.GetSpellSlot("summonerexhaust");
            if (exhaust != SpellSlot.Unknown)
            {
                var Exhaust = new Menu("虚弱", "mexhaust");
                Exhaust.AddItem(new MenuItem("useExhaust", "使用虚弱")).SetValue(true);
                Exhaust.AddItem(new MenuItem("exDanger", "危险时使用")).SetValue(true);
                Exhaust.AddItem(new MenuItem("aExhaustPct", "队友血量")).SetValue(new Slider(35));
                Exhaust.AddItem(new MenuItem("eExhaustPct", "敌人血量")).SetValue(new Slider(35));
                Exhaust.AddItem(new MenuItem("exhaustMode", "模式")).SetValue(new StringList(new[] { "总是", "连招时" }));

                mainmenu.AddSubMenu(Exhaust);
            }

            root.AddSubMenu(mainmenu);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            isjungling = OracleLib.SmiteAll.Any(Items.HasItem);

            FindSmite();
            CheckExhaust();
            CheckIgnite();
            CheckSmite();
            CheckClarity();
            CheckHeal(OC.IncomeDamage);
            CheckBarrier(OC.IncomeDamage);
        }

        #region Drawings
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (me.GetSpellSlot("summonersmite") != SpellSlot.Unknown || isjungling)
            {
                if (!mainmenu.Item("drawSmite").GetValue<bool>() || me.IsDead)
                    return;

                if (mainmenu.Item("useSmite").GetValue<KeyBind>().Active)
                    Utility.DrawCircle(me.Position, 760, Color.White, 1, 1);

                var minionList =
                    MinionManager.GetMinions(me.Position, 760f, MinionTypes.All, MinionTeam.Neutral);

                if (!minionList.Any())
                    return;

                foreach (var m in minionList)
                {
                    bool valid;
                    if (Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline))
                    {
                        valid = m.IsHPBarRendered && !m.IsDead &&
                                (OracleLib.LargeMinions.Any(n => m.Name.Substring(0, m.Name.Length - 5).Equals(n) ||
                                                                   OracleLib.EpicMinions.Any(
                                                                       nx => m.Name.Substring(0, m.Name.Length - 5).Equals(nx))));
                    }
                    else
                    {
                        valid = m.IsHPBarRendered && !m.IsDead &&
                                (!m.Name.Contains("Mini") &&
                                 (OracleLib.SmallMinions.Any(z => m.Name.StartsWith(z)) ||
                                  OracleLib.LargeMinions.Any(n => m.Name.StartsWith(n)) ||
                                  OracleLib.EpicMinions.Any(nx => m.Name.StartsWith(nx))));
                    }

                    if (valid)
                    {
                        Vector2 hpBarPos = m.HPBarPosition;
                        hpBarPos.X += 35;
                        hpBarPos.Y += 18;
                        var smiteDmg = (int) me.GetSummonerSpellDamage(m, Damage.SummonerSpell.Smite);
                        float damagePercent = smiteDmg/m.MaxHealth;
                        float hpXPos = hpBarPos.X + (63*damagePercent);

                        Drawing.DrawLine(hpXPos, hpBarPos.Y, hpXPos, hpBarPos.Y + 5, 2,
                            smiteDmg > m.Health ? Color.Lime : Color.White);
                    }
                }
            }
        }

        #endregion

        #region Ignite

        private static void CheckIgnite()
        {
            var ignite = me.GetSpellSlot("summonerdot");
            if (ignite == SpellSlot.Unknown)
                return;

            if (ignite != SpellSlot.Unknown && !mainmenu.Item("useIgnite").GetValue<bool>())
                return;

            if (me.SummonerSpellbook.CanUseSpell(ignite) != SpellState.Ready)
                return;

            if (mainmenu.Item("dotMode").GetValue<StringList>().SelectedIndex == 0)
            {
                foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(600)))
                {
                    if (target.Health < me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                    {
                        if (!target.HasBuff("summonerdot", true))
                            me.SummonerSpellbook.CastSpell(ignite, target);
                    }
                }
            }

            else if (mainmenu.Item("dotMode").GetValue<StringList>().SelectedIndex == 1 &&
                     OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
            {
                var aaDmg = 0f;
                var aSpeed = me.AttackSpeedMod;
                if (aSpeed < 0.8f)
                    aaDmg = me.FlatPhysicalDamageMod*3;
                else if (aSpeed > 1f && aSpeed < 1.3f)
                    aaDmg = me.FlatPhysicalDamageMod*5;
                else if (aSpeed > 1.3f && aSpeed < 1.5f)
                    aaDmg = me.FlatPhysicalDamageMod*7;
                else if (aSpeed > 1.5f && aSpeed < 1.7f)
                    aaDmg = me.FlatPhysicalDamageMod*9;
                else if (aSpeed > 2.0f)
                    aaDmg = me.FlatPhysicalDamageMod*11;

                var target = SimpleTs.GetTarget(900f, SimpleTs.DamageType.Physical);
                if (target == null)
                    return;

                if (target.Distance(me.Position) <= 600f)
                {
                    float dmg = (me.Level*20) + 50;
                    float regenpersec = (target.FlatHPRegenMod + (target.HPRegenRate*target.Level));
                    float dmgafter = (dmg - ((regenpersec*5)/2));

                    if (target.Health < (dmgafter + aaDmg))
                    {
                        if (!target.HasBuff("summonerdot", true))
                            me.SummonerSpellbook.CastSpell(ignite, target);
                    }
                }
            }
        }

        #endregion

        #region Barrier

        private static void CheckBarrier(float incdmg = 0)
        {
            var barrier = me.GetSpellSlot("summonerbarrier");
            if (barrier == SpellSlot.Unknown)
                return;

            if (barrier != SpellSlot.Unknown && !mainmenu.Item("useBarrier").GetValue<bool>())
                return;

            if (me.SummonerSpellbook.CanUseSpell(barrier) != SpellState.Ready)
                return;

            var iDamagePercent = (int) ((incdmg/me.MaxHealth)*100);
            var mHealthPercent = (int) ((me.Health/me.MaxHealth)*100);

            if (!me.NotRecalling())
                return;

            if (mHealthPercent <= mainmenu.Item("useBarrierPct").GetValue<Slider>().Value &&
                menuconfig.Item("suseOn" + me.SkinName).GetValue<bool>())
            {
                if ((iDamagePercent >= 1 || incdmg >= me.Health) && OC.AggroTarget.NetworkId == me.NetworkId)
                    me.SummonerSpellbook.CastSpell(barrier, me);
            }

            else if (iDamagePercent >= mainmenu.Item("useBarrierDmg").GetValue<Slider>().Value &&
                     OC.AggroTarget.NetworkId == me.NetworkId)
            {
                me.SummonerSpellbook.CastSpell(barrier, me);
            }
        }

        #endregion

        #region Heal

        private static void CheckHeal(float incdmg = 0)
        {
            var heal = me.GetSpellSlot("summonerheal");
            if (heal == SpellSlot.Unknown)
                return;

            if (heal != SpellSlot.Unknown && !mainmenu.Item("useHeal").GetValue<bool>())
                return;

            if (me.SummonerSpellbook.CanUseSpell(heal) != SpellState.Ready)
                return;

            var target = OC.FriendlyTarget();
            var iDamagePercent = (int) ((incdmg/me.MaxHealth)*100);

            if (target.Distance(me.Position) <= 700f)
            {
                if (!me.NotRecalling())
                    return;

                var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                if (aHealthPercent <= mainmenu.Item("useHealPct").GetValue<Slider>().Value &&
                    menuconfig.Item("suseOn" + target.SkinName).GetValue<bool>())
                {
                    if ((iDamagePercent >= 1 || incdmg >= target.Health || target.HasBuff("summonerdot"))
                        && OC.AggroTarget.NetworkId == target.NetworkId)
                    {
                        me.SummonerSpellbook.CastSpell(heal, target);
                    }

                }
                else if (iDamagePercent >= mainmenu.Item("useHealDmg").GetValue<Slider>().Value &&
                         menuconfig.Item("suseOn" + target.SkinName).GetValue<bool>() && OC.AggroTarget.NetworkId == target.NetworkId)
                {
                    me.SummonerSpellbook.CastSpell(heal, target);
                }
            }
        }

        #endregion

        #region Clarity

        private static void CheckClarity()
        {
            var clarity = me.GetSpellSlot("summonermana");
            if (clarity == SpellSlot.Unknown)
                return;

            if (clarity != SpellSlot.Unknown && !mainmenu.Item("useClarity").GetValue<bool>())
                return;

            if (me.SummonerSpellbook.CanUseSpell(clarity) == SpellState.Ready)
            {
                var target = OC.FriendlyTarget();
                if (target.Distance(me.Position) <= 600f)
                {
                    var aManaPercent = (int) ((target.Mana/target.MaxMana)*100);
                    if (aManaPercent <= mainmenu.Item("useClarityPct").GetValue<Slider>().Value
                        && menuconfig.Item("suseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (!Utility.InFountain() && !me.HasBuff("Recall"))
                            me.SummonerSpellbook.CastSpell(clarity, target);
                    }
                }
            }
        }

        #endregion

        #region Smite
        private static void FindSmite()
        {
            if (OracleLib.SmiteBlue.Any(Items.HasItem))
                smiteslot = "s5_summonersmiteplayerganker";
            else if (OracleLib.SmiteRed.Any(Items.HasItem))
                smiteslot = "s5_summonersmiteduel";
            else if (OracleLib.SmiteGrey.Any(Items.HasItem))
                smiteslot = "s5_summonersmitequick";
            else if (OracleLib.SmitePurple.Any(Items.HasItem))
                smiteslot = "itemsmiteaoe";
            else
                smiteslot = "summonersmite";
        }

        private static void CheckSmite()
        {
            var smite = me.GetSpellSlot(smiteslot);
            if (smite == SpellSlot.Unknown)
                return;
            
            if (smite != SpellSlot.Unknown && !mainmenu.Item("useSmite").GetValue<KeyBind>().Active)
                return;

            CheckChampSmite("Vi", 125f, SpellSlot.E);
            CheckChampSmite("JarvanIV", 770f, SpellSlot.Q);
            CheckChampSmite("Poppy", 125f, SpellSlot.Q);
            CheckChampSmite("Nasus", 125f, SpellSlot.Q);
            CheckChampSmite("Riven", 125f, SpellSlot.W);
            CheckChampSmite("Malphite", 200f, SpellSlot.E);
            CheckChampSmite("LeeSin", 1100f, SpellSlot.Q, 1);
            CheckChampSmite("Nunu", 125f, SpellSlot.Q);
            CheckChampSmite("Olaf", 325f, SpellSlot.E);
            CheckChampSmite("Elise", 425f, SpellSlot.Q);
            CheckChampSmite("Warwick", 400f, SpellSlot.Q);
            CheckChampSmite("MasterYi", 600f, SpellSlot.Q);
            CheckChampSmite("Kayle", 650, SpellSlot.Q);
            CheckChampSmite("Khazix", 325f, SpellSlot.Q);
            CheckChampSmite("MonkeyKing", 300f, SpellSlot.Q);

            if (me.SummonerSpellbook.CanUseSpell(smite) != SpellState.Ready)
                return;

            List<Obj_AI_Base>
                minionList = MinionManager.GetMinions(me.Position, 760f, MinionTypes.All, MinionTeam.Neutral);

            if (minionList.Any())
            {
                foreach (Obj_AI_Base minion in minionList.Where(m => m.IsValidTarget(760f)))
                {
                    var damage = 
                        (float) me.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite);

                    if (OracleLib.LargeMinions.Any(name => minion.Name.StartsWith(name) && !minion.Name.Contains("Mini")))
                    {
                        if (minion.Health <= damage)
                        {
                            if (mainmenu.Item("smiteLarge").GetValue<bool>())
                                me.SummonerSpellbook.CastSpell(smite, minion);
                        }
                    }

                    else if (OracleLib.SmallMinions.Any(name => minion.Name.StartsWith(name) && !minion.Name.Contains("Mini")))
                    {
                        if (minion.Health <= damage)
                            if (mainmenu.Item("smiteSmall").GetValue<bool>())
                                me.SummonerSpellbook.CastSpell(smite, minion);
                    }

                    else if (OracleLib.EpicMinions.Any(name => minion.Name.StartsWith(name)))
                    {
                        if (minion.Health <= damage)
                            if (mainmenu.Item("smiteEpic").GetValue<bool>())
                                me.SummonerSpellbook.CastSpell(smite, minion);
                    }
                }
            }          
        }

        private static void CheckChampSmite(string name, float range, SpellSlot slot, int stage = 0)
        {
            if (me.SkinName != name)
                return;

            var champdamage = 0f;
            if (!mainmenu.Item("SmiteSpell").GetValue<bool>())
                return;

            if (me.SkinName == name &&
                me.Spellbook.CanUseSpell(slot) != SpellState.Ready)
                return;

            var data = me.Spellbook.GetSpell(slot);

            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(range)))
            {
                var smitedamage = 
                    (float) me.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite);

                if (me.SkinName == name)
                    champdamage = (float)me.GetSpellDamage(minion, slot, stage);

                if (me.Distance(minion.Position) > range)
                    return;

                if (OracleLib.EpicMinions.Any(xe => minion.Name.StartsWith(xe) && !minion.Name.Contains("Mini")))
                {
                    if (mainmenu.Item("smiteEpic").GetValue<bool>() && minion.Health <= smitedamage + champdamage)
                    {
                        if (me.SkinName == "JarvanIV")
                            me.Spellbook.CastSpell(slot, minion.Position);
                        else if (name == "Riven")
                            me.Spellbook.CastSpell(slot);
                        else if (me.SkinName == "LeeSin" && data.Name == "blindmonkqtwo" && minion.HasBuff("BlindMonkSonicWave"))
                            me.Spellbook.CastSpell(slot);
                        else
                        {
                            me.Spellbook.CastSpell(slot, minion);
                        }
                    }
                }

                else if (OracleLib.LargeMinions.Any(xe => minion.Name.StartsWith(xe) && !minion.Name.Contains("Mini")))
                {
                    //Game.PrintChat("ding");
                    if (mainmenu.Item("smiteLarge").GetValue<bool>() && minion.Health <= smitedamage + champdamage)
                    {
                        if (name == "JarvanIV")
                            me.Spellbook.CastSpell(slot, minion.Position);
                        else if (name == "Riven")
                            me.Spellbook.CastSpell(slot);
                        else if (name == "LeeSin" && data.Name == "blindmonkqtwo" && minion.HasBuff("BlindMonkSonicWave"))
                            me.Spellbook.CastSpell(slot);
                        else
                        {
                            me.Spellbook.CastSpell(slot, minion);
                        }
                    }
                }
            }
        }

        #endregion

        #region Exhaust

        private static void CheckExhaust()
        {
            var exhaust = me.GetSpellSlot("summonerexhaust");
            if (exhaust == SpellSlot.Unknown)
                return;
            if (exhaust != SpellSlot.Unknown && !mainmenu.Item("useExhaust").GetValue<bool>())
                return;

            if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                mainmenu.Item("exhaustMode").GetValue<StringList>().SelectedIndex == 1)
                return;

            var target = OC.FriendlyTarget();
            if (me.SummonerSpellbook.CanUseSpell(exhaust) == SpellState.Ready)
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsValidTarget(900))
                            .OrderByDescending(xe => xe.BaseAttackDamage + xe.FlatPhysicalDamageMod))
                {

                    if (enemy.Distance(me.Position) > 650)
                        return;

                    var aHealthPercent = target.Health/target.MaxHealth*100;
                    var eHealthPercent = enemy.Health/enemy.MaxHealth*100;

                    if (eHealthPercent <= mainmenu.Item("eExhaustPct").GetValue<Slider>().Value)
                        if (!enemy.IsFacing(target))
                            me.SummonerSpellbook.CastSpell(exhaust, enemy);

                        else if (aHealthPercent <= mainmenu.Item("aExhaustPct").GetValue<Slider>().Value)
                            if (enemy.IsFacing(target))
                                me.SummonerSpellbook.CastSpell(exhaust, enemy);
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var exhaust = me.GetSpellSlot("summonerexhaust");
            if (exhaust == SpellSlot.Unknown)
                return;

            if (exhaust != SpellSlot.Unknown && (!mainmenu.Item("useExhaust").GetValue<bool>() ||
                                               !mainmenu.Item("exDanger").GetValue<bool>()))
                return;

            if (me.SummonerSpellbook.CanUseSpell(exhaust) == SpellState.Ready)
            {
                if (sender.IsEnemy && sender.Type == me.Type)
                {
                    var attacker = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                    var attackerslot = attacker.GetSpellSlot(args.SData.Name);

                    if (attacker.Distance(me.Position) > 650f)
                        return;

                    foreach (var spell in OracleLib.Database.Where(x => sender.SkinName == x.Name))
                    {
                        if (spell.DangerLevel == RiskLevel.Extreme && attackerslot == SpellSlot.R)
                        {
                            me.SummonerSpellbook.CastSpell(exhaust, attacker);
                        }
                    }
                }
            }
        }

        #endregion

    }
}
