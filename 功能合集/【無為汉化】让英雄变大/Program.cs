using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace MakeMyMonsterGrow
{{
    internal class Program
    {{
        public static Menu Config;
        public static Int32 SequenceId = 1;
        public static Int32 previousNetId = 0x40000019;
        public static Int32 newNetId = 0x40000019;
        static void Main(string[] args)
        {{
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        static void Game_OnGameLoad(EventArgs args)
        {{
            Config = new Menu("【無為汉化】让英雄变大", MakeMyMonsterGrow", true);
            Game.OnGameSendPacket += Game_OnGameSendPacket;
            Config.AddItem(new MenuItem("Size", 变大尺寸").SetValue(new Slider(150, 50, 200)));
            Config.AddToMainMenu();
			
			Config.AddSubMenu(new Menu("無為汉化", by wuwei"));
				Config.SubMenu("by wuwei").AddItem(new MenuItem("qunhao", 汉化群：386289593"));
				Config.SubMenu("by wuwei").AddItem(new MenuItem("qunhao2", 娃娃群：158994507"));
        }
        static void Game_OnGameSendPacket(GamePacketEventArgs args)
        {{
            if (args.PacketData[0] == Packet.C2S.SetTarget.Header)
            {{
                ChangeSize(previousNetId, 1.0f);
                newNetId = BitConverter.ToInt32(args.PacketData, 9);
                ChangeSize(newNetId, (Single)(Config.Item("Size").GetValue<Slider>().Value) / 100.0f);
                previousNetId = newNetId;
            }
            else if (args.PacketData[0] == Packet.C2S.UpdateConfirm.Header)
                SequenceId = BitConverter.ToInt32(args.PacketData, 5) + 1;
        }
        private static void ChangeSize(Int32 netId, Single size)
        {{
            Obj_AI_Hero hero = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(netId);
            if (hero != null)
            {{
                GamePacket p = new GamePacket(0xC4);
                p.WriteInteger(0);
                p.WriteInteger(SequenceId++);
                p.WriteByte(0x1);
                p.WriteByte(0x8);
                p.WriteInteger(netId);
                p.WriteInteger(0x800);
                p.WriteByte(0x8);
                p.WriteFloat(size);
                p.Process();
                SequenceId++;
            }
        }
    }
}
