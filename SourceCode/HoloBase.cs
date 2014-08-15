using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using VerseBase;

namespace Clutter
{
    class HoloBase : Building
    {

        private CompPowerTrader powerComp;
        private bool Toggle = false;
        private static Texture2D UI_Pon;
        private static Texture2D UI_Poff;
        public Hologram Holoimage
        {
            get
            {
                foreach (Thing current in Find.ThingGrid.ThingsAt(base.Position))
                {
                    Hologram Holoimg = current as Hologram;
                    if (Holoimg != null)
                    {
                        return Holoimg;
                    }
                }
                return null;
            }
        }
        public int savestat = 0;
        public bool Rsave = false;
        private int BaseSaveStat = 0;
        private bool BaseRotSave = false;
        public bool DataRecived = false;


        public override void SpawnSetup()
        {
            HoloBase.UI_Pon = ContentFinder<Texture2D>.Get("Clutter/Ui/Ui_Switch_on", true);
            HoloBase.UI_Poff = ContentFinder<Texture2D>.Get("Clutter/Ui/Ui_Switch_off", true);
            base.SpawnSetup();
            powerComp = base.GetComp<CompPowerTrader>();
        }

        public override IEnumerable<Command> GetCommands()
        {
            IList<Command> list = new List<Command>();
            Command_Action command_Action = new Command_Action();

            if (powerComp.PowerOn && Toggle)
            {
                command_Action.icon = HoloBase.UI_Pon;
                command_Action.defaultDesc = "On";

            }

            if (!Toggle || !powerComp.PowerOn)
            {
                command_Action.icon = HoloBase.UI_Poff;
                command_Action.defaultDesc = "Off";

            }

            command_Action.activateSound = SoundDef.Named("Click");
            command_Action.action = new Action(ToggleStatus);
            command_Action.groupKey = 887767441;
            list.Add(command_Action);
            IEnumerable<Command> commands = base.GetCommands();
            return (commands == null) ? list.AsEnumerable<Command>() : list.AsEnumerable<Command>().Concat(commands);
        }

        public override void ExposeData()
        {
            base.ExposeData();


            Scribe_Values.LookValue<bool>(ref Toggle, "Toggle");
            Scribe_Values.LookValue<int>(ref savestat, "HoloStats");
            Scribe_Values.LookValue<bool>(ref Rsave, "HoloRotation");
            Scribe_Values.LookValue<int>(ref BaseSaveStat, "BaseHoloStats");
            Scribe_Values.LookValue<bool>(ref BaseRotSave, "BaseHoloRotation");

        }

        public override void Tick()
        {
            base.Tick();

            if (DataRecived)
            {
                BaseRotSave = Rsave;
                BaseSaveStat = savestat;

                DataRecived = false;
            }

            Thing Holo;
            ThingDef HoloImageDef = ThingDef.Named("HoloOne");
            Holo = Find.ThingGrid.ThingAt(base.Position, ThingDef.Named("HoloOne"));

            if ((!powerComp.PowerOn && Holo != null) || (powerComp.PowerOn && !Toggle && Holo != null))
            {
                Holo.Destroy();
            }

            if (powerComp.PowerOn && Toggle && Holo == null)
            {

                GenSpawn.Spawn(HoloImageDef, base.Position);

                savestat = BaseSaveStat;
                Rsave = BaseRotSave;

                if (Holoimage != null)
                {
                    if (savestat == 0)
                    {
                        Holoimage.holostatus = Hologram.HoloStatus.piramid;
                    }
                    else if (savestat == 1)
                    {
                        Holoimage.holostatus = Hologram.HoloStatus.plant;
                    }
                    else if (savestat == 2)
                    {
                        Holoimage.holostatus = Hologram.HoloStatus.balls;
                    }
                    if (Rsave)
                    {
                        Holoimage.Rswitch = true;
                    }
                    else if (!Rsave)
                    {
                        Holoimage.Rswitch = false;
                    }
                }
            }


        }



        private void ToggleStatus()
        {
            Toggle = !Toggle;


        }
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append(base.GetInspectString());
            stringBuilder.AppendLine();
            stringBuilder.Append("Holo State: ");
            if (savestat == 0)
            {
                stringBuilder.Append("Piramid");
            }
            else if (savestat == 1)
            {
                stringBuilder.Append("Plant");
            }
            else if (savestat == 2)
            {
                stringBuilder.Append("Balls");
            }
           
            stringBuilder.AppendLine();
            stringBuilder.Append("Rotation Toggle: ");
            if (Rsave)
            {
                stringBuilder.Append("On");

            }
            else if (!Rsave)
            {
                stringBuilder.Append("Off");
            }

            return stringBuilder.ToString();
        }

    }

}