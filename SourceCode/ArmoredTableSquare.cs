using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using RimWorld;
using VerseBase;

namespace Clutter
{
    public class Table_Square : Building
    {
        private bool TClean = false;
        private static Texture2D UI_change;
        
        private Material cleanMat;
        private Material clutterMat;
        public override Material DrawMat(IntRot rot)

        {
            
            {
                return (!this.TClean) ? this.clutterMat : this.cleanMat;
            }
        }
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            Table_Square.UI_change = ContentFinder<Texture2D>.Get("Clutter/Ui/UI_Change Button", true);

            this.cleanMat = MaterialPool.MatFrom("Clutter/Tables/TableMidworldSquare", true);
            this.clutterMat = MaterialPool.MatFrom("Clutter/Tables/TableSpacerSquare", true);
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<bool>(ref this.TClean, "TClean", false, false);
        }
        public override IEnumerable<Command> GetCommands()
        {
            IList<Command> list = new List<Command>();
            Command_Action command_Action = new Command_Action();
            command_Action.icon = Table_Square.UI_change;
            if (!this.TClean)
            {

                command_Action.defaultDesc = "Next: Midworld ";
            }
            else
            {

                command_Action.defaultDesc = "Next: Spacer ";
            }
            command_Action.activateSound = SoundDef.Named("Click");
            command_Action.action = new Action(this.SwitchTextureState);
            command_Action.groupKey = 887765321;
            list.Add(command_Action);
            IEnumerable<Command> commands = base.GetCommands();
            return (commands == null) ? list.AsEnumerable<Command>() : list.AsEnumerable<Command>().Concat(commands);
        }

        public override string GetInspectString()
        {
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.Append("Current:");
            stringBuilder.AppendLine();

            
            if (this.TClean)
            {
                stringBuilder.Append(" Midworld ");
            }
            else
            {
                stringBuilder.Append(" Spacer ");
            }

            return stringBuilder.ToString();
        }

        private void SwitchTextureState()
        {
            this.TClean = !this.TClean;
            Find.MapDrawer.MapChanged(base.Position, MapChangeType.Things);
        }
    }
}