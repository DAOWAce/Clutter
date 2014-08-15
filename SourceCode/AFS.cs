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
    class AFS : Building
    {

        private CompGlower glowerComp;
        private int charge;
        private static Texture2D UI_Son;
        private static Texture2D UI_Soff;
        private float distance = 10f;
        private int counter_runticks = 0;
        private bool flagWorkDone = true;
        private int activeWorkItem = 0;
        private IEnumerable<Thing> foundThings;
        private Material Afs_On;
        private Material Afs_Off;
       



        public override void SpawnSetup()
        {
            base.SpawnSetup();
            AFS.UI_Son = ContentFinder<Texture2D>.Get("Clutter/Ui/Ui_Switch_on", true);
            AFS.UI_Soff = ContentFinder<Texture2D>.Get("Clutter/Ui/Ui_Switch_off", true);
            
            glowerComp = base.GetComp<CompGlower>();
            Afs_On = MaterialPool.MatFrom("Clutter/Devices/AFS_ON", true);
            Afs_Off = MaterialPool.MatFrom("Clutter/Devices/AFS_OFF", true);
            charge = 20;
            if (Find.ResearchManager.IsFinished(ResearchProjectDef.Named("AFSLife")))
            {
                charge = (charge + 5);
            }
            if (Find.ResearchManager.IsFinished(ResearchProjectDef.Named("AFSCore")))
            {
                charge = (charge + 10);
            }


        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<int>(ref charge, "Charges");
        }

        public override IEnumerable<Command> GetCommands()
        {
            IList<Command> list = new List<Command>();
            Command_Action command_Action = new Command_Action();

            if (charge <= 0)
            {

                command_Action.disabled = true;

            }
            if (flagWorkDone)
            {
                command_Action.icon = AFS.UI_Son;
                command_Action.defaultDesc = "Off";

            }

            if (!flagWorkDone)
            {
                command_Action.icon = AFS.UI_Soff;
                command_Action.defaultDesc = "On";

            }
            command_Action.activateSound = SoundDef.Named("Click");
            command_Action.action = new Action(this.SwitchWorkDone);
            command_Action.groupKey = 887767441;
            list.Add(command_Action);
            IEnumerable<Command> commands = base.GetCommands();
            return (commands == null) ? list.AsEnumerable<Command>() : list.AsEnumerable<Command>().Concat(commands);
        }

        public override void DrawExtraSelectionOverlays()
        {
            if (charge > 0)
            {
                GenDraw.DrawRadiusRing(base.Position, distance);
            }
        }


        private void SwitchWorkDone()
        {
            flagWorkDone = !flagWorkDone;

        }



        public override void Tick()
        {
            base.Tick();


            if (!flagWorkDone && charge > 0)
            {
                glowerComp.Lit = true;
            }
            if (charge <= 0 || flagWorkDone)
            {
                glowerComp.Lit = false;
            }

            if (flagWorkDone)
            {

                activeWorkItem = 0;
                return;
            }


            if (!flagWorkDone && counter_runticks <= 0)
            {


                if (activeWorkItem <= 0)
                {
                    foundThings = FindFireInRoomAndDistance(base.Position, distance);
                    if (foundThings == null)
                        activeWorkItem = 0;
                    else
                        activeWorkItem = foundThings.Count();
                }




                if (activeWorkItem <= 0 || foundThings.Count() == 0)
                {
                    flagWorkDone = true;
                    return;
                }


                Thing fire = null;

                if (activeWorkItem <= foundThings.Count() && activeWorkItem > 0 && charge > 0)
                {
                    fire = foundThings.ElementAt(activeWorkItem - 1);
                    activeWorkItem -= 1;


                    if (fire != null)
                    {

                        counter_runticks = 50;

                        ThingDef spongeDef = ThingDef.Named("PlasmaSponge");
                       

                        IEnumerable<Thing> foundSponges = Find.ListerThings.ThingsOfDef(spongeDef);
                        bool spongeFoundFlag = false;
                        //if (Find.ThingGrid.ThingAt(fire.Position, marker) == null)
                        //{



                            foreach (Thing sponge in foundSponges)
                            {

                                
                                if (sponge.Position == fire.Position)
                                {
                                    spongeFoundFlag = true;
                                    break;
                                }
                                


                            }


                            if (!spongeFoundFlag)
                            {
                                charge -= 1;
                                //GenSpawn.Spawn(marker, fire.Position);
                                Bullet_Sponge PSpark = (Bullet_Sponge)GenSpawn.Spawn(ThingDef.Named("PlasmaSpark"), base.Position);
                                PSpark.Launch(this, fire.Position);

                                //GenSpawn.Spawn(spongeDef, fire.Position);
                            }
                       // }
                    }
                }

            }
            if (!flagWorkDone && counter_runticks > 0)
            {
                counter_runticks--;
            }

        }




        private IEnumerable<Thing> FindFireInRoomAndDistance(IntVec3 position, float distance)
        {

            Room room = RoomQuery.RoomAt(position);
            if (room == null)
            {
                IEnumerable<IntVec3> founds = GenAdj.AdjacentSquares8WayAndInside(position);
                foreach (IntVec3 found in founds)
                {
                    if (RoomQuery.RoomAt(found) != null)
                    {
                        room = RoomQuery.RoomAt(found);
                        break;
                    }
                }
            }

            IEnumerable<Thing> fires = Find.ListerThings.ThingsOfDef(ThingDefOf.Fire);

           if (fires == null)
           {
                
               return fires;
                

           }
            // Correction here: (room == room OR room == null) && within distance
           else
           {
            return fires.Where<Thing>(t => (room == RoomQuery.RoomAt(t.Position) || RoomQuery.RoomAt(t.Position) == null) &&
                 t.Position.WithinHorizontalDistanceOf(position, distance));
           }
        }




        public override Material DrawMat(IntRot rot)

        {
           
            
            {
                if (charge > 0)
                {
                    return Afs_On;

                }
                else
                {
                    return Afs_Off;
                }

            }
        }



        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());


            stringBuilder.AppendLine();
            if (charge > 0 && flagWorkDone)
            {
                stringBuilder.Append("Charges: ");
                stringBuilder.Append(charge.ToString());
                stringBuilder.AppendLine();
                stringBuilder.Append("Status: Inactive");
            }
            if (charge > 0 && !flagWorkDone)
            {
                stringBuilder.Append("Charges: ");
                stringBuilder.Append(charge.ToString());
                stringBuilder.AppendLine();
                stringBuilder.Append("Status: Active");
            }
            if (charge <= 0)
            {
                stringBuilder.Append("Charges: ");
                stringBuilder.Append(charge.ToString());
                stringBuilder.AppendLine();
                stringBuilder.Append("Status: No charges, You Are DOOMED!");
            }
            return stringBuilder.ToString();
        }




    }

}