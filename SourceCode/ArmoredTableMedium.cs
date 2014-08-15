using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using VerseBase;
using RimWorld;

namespace Clutter
{
    public class Table_Medium : Building
    {

        private int roll = 0;

        private static Texture2D Ui_Change;
        private Material Mat_Midworld_Clean;
        private Material Mat_Midworld_Clutter;
        private Material Mat_Spacer_Clean;
        private Material Mat_Spacer_Clutter;

        public override Material DrawMat(IntRot rot)

        {
            
            {
                if (this.roll <= 0)
                {
                    return Mat_Spacer_Clean;
                }
                if (this.roll <= 1 && roll > 0)
                {
                    return (Mat_Spacer_Clutter);
                }
                if (this.roll <= 2 && roll > 1)
                {
                    return (Mat_Midworld_Clean);
                }
                if (this.roll <= 3 && roll > 2)
                {
                    return (Mat_Midworld_Clutter);
                }
                else
                {
                    return (Mat_Spacer_Clean);
                }
            }
        }

        public override void SpawnSetup()
        {
            base.SpawnSetup();



            //UI
            Ui_Change = ContentFinder<Texture2D>.Get("Clutter/Ui/UI_Change Button", true);


            //Mat
            Mat_Midworld_Clean = MaterialPool.MatFrom("Clutter/Tables/TableMidworldMediumClean", true);
            Mat_Midworld_Clutter = MaterialPool.MatFrom("Clutter/Tables/TableMidworldMediumClutter", true);
            Mat_Spacer_Clean = MaterialPool.MatFrom("Clutter/Tables/TableSpacerMediumClean", true);
            Mat_Spacer_Clutter = MaterialPool.MatFrom("Clutter/Tables/TableSpacerMediumClutter", true);





        }



        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<int>(ref roll, "roll");
        }

        public override IEnumerable<Command> GetCommands()
        {
            IEnumerable<Command> commands;
            IList<Command> commands1 = new List<Command>();
            Command_Action commandAction = new Command_Action();
            commandAction.icon = Table_Medium.Ui_Change;
            if (this.roll <= 0 && Find.ResearchManager.IsFinished(ResearchProjectDef.Named("TableCustom")))
            {

                commandAction.defaultDesc = "Next: Spacer with clutter";
            }
            if (this.roll <= 1 && roll > 0)
            {

                commandAction.defaultDesc = "Next: Midworld clean";
            }
            if (this.roll <= 2 && roll > 1 && Find.ResearchManager.IsFinished(ResearchProjectDef.Named("TableCustom")))
            {

                commandAction.defaultDesc = "Next: Midworld with clutter";

            }
            if (this.roll <= 3 && roll > 2)
            {

                commandAction.defaultDesc = "Next: Spacer clean";

            }
            if (this.roll >= 4)
            {
                roll = 0;

            }
            if (this.roll <= 0 && !Find.ResearchManager.IsFinished(ResearchProjectDef.Named("TableCustom")))
            {

                commandAction.defaultDesc = "Next: Midworld clean";
            }

            if (this.roll <= 2 && roll > 1 && !Find.ResearchManager.IsFinished(ResearchProjectDef.Named("TableCustom")))
            {

                commandAction.defaultDesc = "Next: Spacer clean";
            }
            commandAction.activateSound = SoundDef.Named("Click");
            commandAction.action = new Action(SwitchTextureState);
            commandAction.groupKey = 887765321;
            commands1.Add(commandAction);

            IEnumerable<Command> commands2 = base.GetCommands();
            commands = (commands2 == null ? commands1.AsEnumerable<Command>() : commands1.AsEnumerable<Command>().Concat<Command>(commands2));
            return commands;
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.Append("Current:");
            stringBuilder.AppendLine();

            if (this.roll <= 0)
            {
                stringBuilder.Append(" Spacer clean ");
            }

            if (this.roll <= 1 && roll > 0)
            {
                stringBuilder.Append(" Spacer with clutter ");
            }
            if (this.roll <= 2 && roll > 1)
            {
                stringBuilder.Append(" Midworld clean ");
            }

            if (this.roll <= 3 && roll > 2)
            {
                stringBuilder.Append(" Midworld with clutter ");
            }

            return stringBuilder.ToString();
        }

        public void SwitchTextureState()
        {

            if (this.roll >= 4)
            {
                roll = 0;
            }
            if (this.roll < 4 && Find.ResearchManager.IsFinished(ResearchProjectDef.Named("TableCustom")))
            {
                roll++;
            }
            if (this.roll < 4 && !Find.ResearchManager.IsFinished(ResearchProjectDef.Named("TableCustom")))
            {
                roll = roll + 2;
            }




            Find.MapDrawer.MapChanged(Position, MapChangeType.Things);
        }
    }
}
