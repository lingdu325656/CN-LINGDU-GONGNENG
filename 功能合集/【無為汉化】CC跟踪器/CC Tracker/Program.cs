using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;
using System.IO;
using System.Drawing;

namespace CC_Tracker
{
    class Program
    {
        static List<CCBuff> CCBuffs = new List<CCBuff>();
        static List<CCSpell> CCSpells = new List<CCSpell>();
        static List<CCHero> CCHeros = new List<CCHero>();

        static int startX = Drawing.Width / 2 + 210;
        static int startY = Drawing.Height / 2 + 110;

        static Sprite Sprite;
        static Font Text;
        static Font SmallText;
        static Texture HUD;

        static Menu Menu;
        static void Main(string[] args)
        {
            Sprite = new Sprite(Drawing.Direct3DDevice);
            Text = new Font(Drawing.Direct3DDevice, new FontDescription()
            {
                FaceName = "Verdana",
                Height = 32,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });
            SmallText = new Font(Drawing.Direct3DDevice, new FontDescription()
            {
                FaceName = "Verdana",
                Height = 12,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });
            try
            {
                LoadCC();
                LoadMenu();
                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (CCBuffs.HasChampion(hero.ChampionName) || CCSpells.HasChampion(hero.ChampionName))))
                {
                    CCBuff ccBuff = null;
                    CCSpell ccSpells = null;
                    ccBuff = CCBuffs.GetChampion(hero.ChampionName);
                    ccSpells = CCSpells.GetChampion(hero.ChampionName);
                    Texture champIcon = ccSpells == null ? ccBuff.ChampIcon : ccSpells.Icon;
                    CCHeros.Add(new CCHero(hero.ChampionName, ccBuff, ccSpells, champIcon));
                }
                HUD = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(Properties.Resources.CC_Tracker_HUD, typeof(byte[])), 232, 234, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
                Drawing.OnPreReset += DrawingOnPreReset;
                Drawing.OnPostReset += DrawingOnPostReset;
                Drawing.OnDraw += Drawing_OnDraw;
                AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
                AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
                Game.OnWndProc += Game_OnWndProc;
            }
            catch (Exception ex)
            {
                Game.PrintChat("Exception: {0}", ex.Message);
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_KEYDOWN)
                return;

            var key = args.WParam;
            switch (key)
            {
                case 37:
                    startX--;
                    break;
                case 38:
                    startY--;
                    break;
                case 39:
                    startX++;
                    break;
                case 40:
                    startY++;
                    break;
            }
        }

        static void LoadMenu()
        {
            Menu = new Menu("【無為汉化】CC跟踪器", "ccTracker", true);
            Menu.AddItem(new MenuItem("trackCC", "CC跟踪").SetValue(true));
            Menu.AddItem(new MenuItem("drawHUD", "显示界面").SetValue(true));
            Menu.AddItem(new MenuItem("rePos", "用键盘方向键移动界面!"));
            Menu.AddToMainMenu();

            Game.PrintChat("CC Tracker - Loaded!");
        }

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs e)
        {
            Text.Dispose();
            Sprite.Dispose();
        }

        private static void DrawingOnPostReset(EventArgs args)
        {
            //Game.PrintChat("DrawingOnPostReset");
            Text.OnResetDevice();
            Sprite.OnResetDevice();
        }

        private static void DrawingOnPreReset(EventArgs args)
        {
            //Game.PrintChat("DrawingOnPreReset");
            Text.OnLostDevice();
            Sprite.OnLostDevice();
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed || !Menu.Item("trackCC").GetValue<bool>())
                return;
            try
            {
                if (Sprite.IsDisposed)
                    return;
                if (Menu.Item("drawHUD").GetValue<bool>())
                {
                    Sprite.Begin();
                    Sprite.Draw(HUD, new ColorBGRA(255, 255, 255, 255), null, new Vector3(-startX, -startY, 0));
                    Sprite.End();
                }
                int newX = startX + 13;
                int newY = startY + 25;
                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => CCHeros.HasChampion(hero.ChampionName) && hero.IsValid && hero.IsEnemy))
                {
                    CCHero ccHero = CCHeros.GetChampion(hero.ChampionName);
                    Sprite.Begin();
                    Sprite.Draw(ccHero.Icon, new ColorBGRA(255, 255, 255, 255), null, new Vector3(-newX, -newY, 0));
                    Sprite.End();
                    if (ccHero.CCBuff != null)
                    {
                        newX += 32 + 5;
                        Sprite.Begin();
                        BuffInstance buff = null;
                        buff = hero.Buffs.First(x => x.Name == ccHero.CCBuff.Name);
                        if (buff != null)
                            Sprite.Draw(ccHero.CCBuff.BuffIcon, new ColorBGRA(255, 255, 255, 255), null, new Vector3(-newX, -newY, 0));
                        else
                            Sprite.Draw(ccHero.CCBuff.BuffIcon, new ColorBGRA(255, 55, 55, 255), null, new Vector3(-newX, -newY, 0));
                        Sprite.End();
                    }
                    if (ccHero.CCSpells != null)
                    {
                        foreach (SpellSlot spellSlot in ccHero.CCSpells.SpellSlots)
                        {
                            newX += 32 + 5;
                            string slot = spellSlot.ToString();
                            bool notAvailable = (hero.Spellbook.GetSpell(spellSlot).State == SpellState.NotLearned
                                || hero.Spellbook.GetSpell(spellSlot).State == SpellState.NoMana);
                            float expires = hero.Spellbook.GetSpell(spellSlot).CooldownExpires - Game.Time;
                            float smallExpires = (float) Math.Round(expires, 1);
                            Text.DrawText(null, slot, newX, newY, notAvailable ? new ColorBGRA(33, 33, 33, 255) : expires <= 0 ? new ColorBGRA(0, 255, 0, 255) : new ColorBGRA(255, 0, 0, 255));
                            if(smallExpires > 0)
                                SmallText.DrawText(null, smallExpires.ToString(), newX, newY - 3, new ColorBGRA(0, 0, 0, 255));
                        }
                    }
                    newX = startX + 13;
                    newY += 32 + 7;
                }
            }
            catch(Exception ex)
            {
                Game.PrintChat("Sprite Exception: " + ex.Message);
            }
        }

        static void LoadCC()
        {
            //Load CC Buffs - Disabled for now
            //CCBuffs.Add(new CCBuff("Annie", "pyromania", Properties.Resources.Annie_Square_0, Properties.Resources.Annie_Passive));
            //CCBuffs.Add(new CCBuff("Udyr", "UdyrBearStance", Properties.Resources.Udyr_Square_0, Properties.Resources.Udyr_BearStance));
            //Load CC Spells
            CCSpells.Add(new CCSpell("Aatrox", Properties.Resources.Aatrox_Square_0, SpellSlot.Q));
            CCSpells.Add(new CCSpell("Ahri", Properties.Resources.Ahri_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Alistar", Properties.Resources.Alistar_Square_0, SpellSlot.Q, SpellSlot.W));
            CCSpells.Add(new CCSpell("Amumu", Properties.Resources.Amumu_Square_0, SpellSlot.Q, SpellSlot.R));
            CCSpells.Add(new CCSpell("Anivia", Properties.Resources.Anivia_Square_0, SpellSlot.Q));
            CCSpells.Add(new CCSpell("Ashe", Properties.Resources.Ashe_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Azir", Properties.Resources.Azir_Square_0, SpellSlot.E, SpellSlot.R));
            CCSpells.Add(new CCSpell("Blitzcrank", Properties.Resources.Blitzcrank_Square_0, SpellSlot.Q, SpellSlot.E, SpellSlot.R));
            CCSpells.Add(new CCSpell("Brand", Properties.Resources.Brand_Square_0, SpellSlot.Q));
            CCSpells.Add(new CCSpell("Braum", Properties.Resources.Braum_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Cho'Gath", Properties.Resources.Chogath_Square_0, SpellSlot.Q, SpellSlot.W));
            CCSpells.Add(new CCSpell("Darius", Properties.Resources.Darius_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Diana", Properties.Resources.Diana_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Draven", Properties.Resources.Draven_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Braum", Properties.Resources.Braum_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Elise", Properties.Resources.Elise_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("FiddleSticks", Properties.Resources.Fiddlesticks_Square_0, SpellSlot.Q, SpellSlot.E));
            CCSpells.Add(new CCSpell("Galio", Properties.Resources.Galio_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Garen", Properties.Resources.Garen_Square_0, SpellSlot.Q));
            CCSpells.Add(new CCSpell("Gnar", Properties.Resources.Gnar_Square_0, SpellSlot.W, SpellSlot.R));
            CCSpells.Add(new CCSpell("Gragas", Properties.Resources.Gragas_Square_0, SpellSlot.E, SpellSlot.R));
            CCSpells.Add(new CCSpell("Hecarim", Properties.Resources.Hecarim_Square_0, SpellSlot.E, SpellSlot.R));
            CCSpells.Add(new CCSpell("Heimerdinger", Properties.Resources.Heimerdinger_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Irelia", Properties.Resources.Irelia_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Janna", Properties.Resources.Janna_Square_0, SpellSlot.Q, SpellSlot.R));
            CCSpells.Add(new CCSpell("Jax", Properties.Resources.Jax_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Jayce", Properties.Resources.Jayce_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Jinx", Properties.Resources.Jinx_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Kalista", Properties.Resources.Kalista_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Karma", Properties.Resources.Karma_Square_0, SpellSlot.W));
            CCSpells.Add(new CCSpell("LeBlanc", Properties.Resources.Leblanc_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("LeeSin", Properties.Resources.LeeSin_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Leona", Properties.Resources.Leona_Square_0, SpellSlot.Q, SpellSlot.E, SpellSlot.R));
            CCSpells.Add(new CCSpell("Lissandra", Properties.Resources.Lissandra_Square_0, SpellSlot.W, SpellSlot.R));
            CCSpells.Add(new CCSpell("Lulu", Properties.Resources.Lulu_Square_0, SpellSlot.W, SpellSlot.R));
            CCSpells.Add(new CCSpell("Lux", Properties.Resources.Lux_Square_0, SpellSlot.Q));
            CCSpells.Add(new CCSpell("Malphite", Properties.Resources.Malphite_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Malzahar", Properties.Resources.Malzahar_Square_0, SpellSlot.Q, SpellSlot.R));
            CCSpells.Add(new CCSpell("Maokai", Properties.Resources.Maokai_Square_0, SpellSlot.Q, SpellSlot.W));
            CCSpells.Add(new CCSpell("Morgana", Properties.Resources.Morgana_Square_0, SpellSlot.Q, SpellSlot.R));
            CCSpells.Add(new CCSpell("Nami", Properties.Resources.Nami_Square_0, SpellSlot.Q, SpellSlot.R));
            CCSpells.Add(new CCSpell("Nautilus", Properties.Resources.Nautilus_Square_0, SpellSlot.Q, SpellSlot.R));
            CCSpells.Add(new CCSpell("Nocturne", Properties.Resources.Nocturne_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Orianna", Properties.Resources.Orianna_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Pantheon", Properties.Resources.Pantheon_Square_0, SpellSlot.W));
            CCSpells.Add(new CCSpell("Poppy", Properties.Resources.Poppy_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Rammus", Properties.Resources.Rammus_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Renekton", Properties.Resources.Renekton_Square_0, SpellSlot.W));
            CCSpells.Add(new CCSpell("Rengar", Properties.Resources.Rengar_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Riven", Properties.Resources.Riven_Square_0, SpellSlot.Q, SpellSlot.W));
            CCSpells.Add(new CCSpell("Ryze", Properties.Resources.Ryze_Square_0, SpellSlot.W));
            CCSpells.Add(new CCSpell("Sejuani", Properties.Resources.Sejuani_Square_0, SpellSlot.Q, SpellSlot.R));
            CCSpells.Add(new CCSpell("Shen", Properties.Resources.Shen_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Shyvana", Properties.Resources.Shyvana_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Singed", Properties.Resources.Singed_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Sion", Properties.Resources.Sion_Square_0, SpellSlot.Q, SpellSlot.R));
            CCSpells.Add(new CCSpell("Skarner", Properties.Resources.Skarner_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Sona", Properties.Resources.Sona_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Soraka", Properties.Resources.Soraka_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Swain", Properties.Resources.Swain_Square_0, SpellSlot.W));
            CCSpells.Add(new CCSpell("Syndra", Properties.Resources.Syndra_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Taric", Properties.Resources.Taric_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Thresh", Properties.Resources.Thresh_Square_0, SpellSlot.Q, SpellSlot.E));
            CCSpells.Add(new CCSpell("Tristana", Properties.Resources.Tristana_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("TwistedFate", Properties.Resources.TwistedFate_Square_0, SpellSlot.W));
            CCSpells.Add(new CCSpell("Urgot", Properties.Resources.Urgot_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Varus", Properties.Resources.Varus_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Vayne", Properties.Resources.Vayne_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Veigar", Properties.Resources.Veigar_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("Vel'Koz", Properties.Resources.VelKoz_Square_0,  SpellSlot.E));
            CCSpells.Add(new CCSpell("Vi", Properties.Resources.Vi_Square_0, SpellSlot.Q, SpellSlot.R));
            CCSpells.Add(new CCSpell("Viktor", Properties.Resources.Viktor_Square_0, SpellSlot.W));
            CCSpells.Add(new CCSpell("Volibear", Properties.Resources.Volibear_Square_0, SpellSlot.Q));
            CCSpells.Add(new CCSpell("Warwick", Properties.Resources.Warwick_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("MonkeyKing", Properties.Resources.MonkeyKing_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Xerath", Properties.Resources.Xerath_Square_0, SpellSlot.E));
            CCSpells.Add(new CCSpell("XinZhao", Properties.Resources.XinZhao_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Yasuo", Properties.Resources.Yasuo_Square_0, SpellSlot.R));
            CCSpells.Add(new CCSpell("Zac", Properties.Resources.Zac_Square_0, SpellSlot.E, SpellSlot.R));
            CCSpells.Add(new CCSpell("Ziggs", Properties.Resources.Ziggs_Square_0, SpellSlot.W));
            CCSpells.Add(new CCSpell("Zyra", Properties.Resources.Zyra_Square_0, SpellSlot.E, SpellSlot.R));
        }
    }
    public static class Extensions
    {
        public static CCHero GetChampion(this List<CCHero> ccheros, string champion)
        {
            try
            {
                return ccheros.First(x => x.Name == champion);
            }
            catch
            {
                return null;
            }
        }
        public static CCBuff GetChampion(this List<CCBuff> ccbuffs, string champion)
        {
            try
            {
                return ccbuffs.First(x => x.Champion == champion);
            }
            catch
            {
                return null;
            }
        }
        public static CCSpell GetChampion(this List<CCSpell> ccspells, string champion)
        {
            try
            {
                return ccspells.First(x => x.Champion == champion);
            }
            catch
            {
                return null;
            }
        }
        public static bool HasChampion(this List<CCHero> ccheros, string champion)
        {
            foreach (CCHero cchero in ccheros)
            {
                if (cchero.Name == champion)
                    return true;
            }
            return false;
        }
        public static bool HasChampion(this List<CCSpell> ccspells, string champion)
        {
            foreach (CCSpell ccspell in ccspells)
            {
                if (ccspell.Champion == champion)
                    return true;
            }
            return false;
        }

        public static bool HasChampion(this List<CCBuff> ccbuffs, string champion)
        {
            foreach (CCBuff ccbuff in ccbuffs)
            {
                if (ccbuff.Champion == champion)
                    return true;
            }
            return false;
        }
    }
}
