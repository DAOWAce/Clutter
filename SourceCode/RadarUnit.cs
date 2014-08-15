using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using VerseBase;
using Verse.AI;

namespace Clutter
{
    class RadarUnit : Building
    {
        private Faction LocalFaction;
        private static Texture2D UI_Pon;
        private static Texture2D UI_Poff;
        private Material CommBaseTex;
        private int MainCat = 0;
        private int SubCat = 0;
        private bool Sending = false;
        private int dotter = 300;
        private int counter = 5000;
        private CompPowerTrader powerComp;
        private CompGlower glowerComp;
        public RadarAntena RadarRelay
        {
            get
            {
                foreach (Thing current in Find.ThingGrid.ThingsAt(base.Position))
                {
                    RadarAntena radarRelay = current as RadarAntena;
                    if (radarRelay != null)
                    {
                        return radarRelay;
                    }

                }
                return null;

            }
        }
        private string endText= " ";
        //private bool endTextSwitcher = false;
        private int powerEat = 0;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            RadarUnit.UI_Pon = ContentFinder<Texture2D>.Get("Clutter/Ui/Ui_Switch_on", true);
            RadarUnit.UI_Poff = ContentFinder<Texture2D>.Get("Clutter/Ui/Ui_Switch_off", true);
            CommBaseTex = MaterialPool.MatFrom("Clutter/Devices/ComUnitBase", MatBases.Transparent);
            powerComp = base.GetComp<CompPowerTrader>();
            glowerComp = base.GetComp<CompGlower>();
            ThingDef CompsTop = ThingDef.Named("CommUnitTop");
            
            ThingDef AntDef = ThingDef.Named("CommUnitTop");
            Thing thing = ThingMaker.MakeThing(AntDef);
            GenSpawn.Spawn(thing, Position, rotation);
            
            
        }

        public override void ExposeData()
        {
            base.ExposeData();
          Scribe_Values.LookValue<string>(ref endText, "CoutdownEndText");
          Scribe_Values.LookValue<bool>(ref Sending, "Sending");
          Scribe_Values.LookValue<int>(ref dotter, "DotRotation");
          Scribe_Values.LookValue<int>(ref counter, "Counter");
          Scribe_Values.LookValue<int>(ref powerEat, "PowerEater");
                        
        }

        public override void Tick()
        {
            base.Tick();

           
           Thing RadAnt;
           ThingDef AntDef = ThingDef.Named("CommUnitTop");
           RadAnt = Find.ThingGrid.ThingAt(base.Position, AntDef);
            
            if (RadAnt ==null)
            {           

                GenSpawn.Spawn(AntDef, base.Position);
            }
                if (RadAnt !=null && RadarRelay ==null)
            {
                RadAnt.Destroy();
                GenSpawn.Spawn(AntDef, base.Position);
            }

                if (RadAnt != null)
            {
                if (powerComp.PowerOn && !Find.RoofGrid.Roofed(this.Position))
                {

                    glowerComp.Lit = true;
                    RadarRelay.Rotation = true;
                    powerComp.powerOutput = -200 + (powerEat * 2);

                    if (Sending)
                    {
                        SignalSend();
                    }

                    if (counter < 5000 && counter > 4990)
                    {
                        EndText();
                    }

                }

                if (!powerComp.PowerOn || Find.RoofGrid.Roofed(this.Position))
                {

                    glowerComp.Lit = false;
                    RadarRelay.Rotation = false;
                    powerComp.powerOutput = -1f;
                    Sending = false;
                    counter = 5000;
                    powerEat = 0;

                }
            }
        }
        
        private void SignalSend()
        {
            if (dotter <= 170 )
            {
                dotter +=1;
            }
            if (dotter > 170)
            {
                dotter = 0;
            }
            powerEat -= 1;
            counter -= 1;
            if(counter <=0)
            {
                powerComp.powerOutput = -200f;
                Sending = false;
                counter = 5000;
                powerEat = 0;
                EventRandomiser();
            }
        
        }

        private void EventRandomiser()
        {
            MainCat = UnityEngine.Random.Range(1, 5);
            //Traders 1
            //Drops 2
            //Raider 3
          
            IncidentDef localDef;
            IncidentParms parms = new IncidentParms();

            //Traders 1
            if (MainCat==1 || MainCat==4)
            {
                SubCat = UnityEngine.Random.Range(1, 5);
                if(SubCat == 1)
                {
                    string text = "Comms Unit Raport: Message was send but no one responded";
                    Find.History.AddGameEvent(text, GameEventType.BadNonUrgent, true, this.Position, string.Empty);
                    //Messages.Message("Signal was send, but no one responded");
                    
                }
                else if(SubCat ==2)
                {
                    Messages.Message("General Traders Responded");
                    localDef = IncidentDef.Named("TraderArrivalGeneral");
                    if (localDef.pointsScaleable)
                    {
                        parms = Find.Storyteller.incidentMaker.ParmsNow();
                    }
                    localDef.Worker.TryExecute(parms);
                }
                else if (SubCat == 3)
                {
                    Messages.Message("Slavers Responded");
                    localDef = IncidentDef.Named("TraderArrivalSlaver");
                    if (localDef.pointsScaleable)
                    {
                        parms = Find.Storyteller.incidentMaker.ParmsNow();
                    }
                    localDef.Worker.TryExecute(parms);
                    
                }
                else if (SubCat == 4)
                {


                    string text = "Comms Unit Raport: White noise everyware";
                    Find.History.AddGameEvent(text, GameEventType.BadNonUrgent, true, this.Position, string.Empty);
                   

                }
            }
            //Other Events
            else if (MainCat == 2)
            {
                SubCat = UnityEngine.Random.Range(1, 3);
                if (SubCat == 1)
                {
                    Messages.Message("Signal bounced off from atmosphere back to earth");
                    localDef = IncidentDef.Named("AnimalInsanity");
                    if (localDef.pointsScaleable)
                    {
                        parms = Find.Storyteller.incidentMaker.ParmsNow();
                    }
                    localDef.Worker.TryExecute(parms);
                }

                else if (SubCat == 2)
                {
                    Messages.Message("Signal activeted something on the orbit");
                    localDef = IncidentDef.Named("ResourcePodCrash");
                    if (localDef.pointsScaleable)
                    {
                        parms = Find.Storyteller.incidentMaker.ParmsNow();
                    }
                    localDef.Worker.TryExecute(parms);

                }

                else if (SubCat == 3)
                {
                    string text = "Comms Unit Raport: White noise everyware";
                    Find.History.AddGameEvent(text, GameEventType.BadNonUrgent, true, this.Position, string.Empty);
                }

            }
            //Raider 3
            else if (MainCat == 3)
            {
                
                SubCat = UnityEngine.Random.Range(1, 3);
                
                if (SubCat == 1)
                {

                    localDef = IncidentDef.Named("RaidEnemy");
                   if (localDef.pointsScaleable)
                   {
                       parms = Find.Storyteller.incidentMaker.ParmsNow();
                   }
                   localDef.Worker.TryExecute(parms);
                   

                }
                else if (SubCat == 2)
                {
                                            
                        localDef = IncidentDef.Named("RaidEnemy");
                        if (localDef.pointsScaleable)
                        {
                            if(LocalFaction == null || !LocalFaction.IsHostileToward(Faction.OfColony))
                            {    
                                (from LocFac in Find.FactionManager.AllFactions
                                 where LocFac.IsHostileToward(Faction.OfColony) && LocFac.def.canSiege
                                     select LocFac).TryRandomElement(out LocalFaction);
                            }

                            parms = Find.Storyteller.incidentMaker.ParmsNow();
                            parms.raidStyle = RaidStyle.Siege;
                            parms.faction = LocalFaction;
                        }
                        parms.raidStyle = RaidStyle.Siege;
                        localDef.Worker.TryExecute(parms);
                 
                }
                else if (SubCat == 3)
                {

                    string text = "Comms Unit Raport: White noise everyware";
                    Find.History.AddGameEvent(text, GameEventType.BadNonUrgent, true, this.Position, string.Empty);
                   

                }
            }
         }


        public override IEnumerable<Command> GetCommands()
        {
            IList<Command> list = new List<Command>();
            Command_Action command_Action = new Command_Action();

            if (powerComp.PowerOn && !Sending && !Find.RoofGrid.Roofed(this.Position))
            {
                command_Action.icon = RadarUnit.UI_Pon;
                command_Action.defaultDesc = "On";
                command_Action.action = new Action(SendingOn);

            }

            if (Sending || !powerComp.PowerOn || Find.RoofGrid.Roofed(this.Position))
            {
                command_Action.icon = RadarUnit.UI_Poff;
                command_Action.defaultDesc = "Off";
                command_Action.Disable();

            }

            command_Action.activateSound = SoundDef.Named("Click");
            command_Action.groupKey = 887767441;
            list.Add(command_Action);
            IEnumerable<Command> commands = base.GetCommands();
            return (commands == null) ? list.AsEnumerable<Command>() : list.AsEnumerable<Command>().Concat(commands);
        }

        private void SendingOn()
        {
            if(!Sending && counter == 5000)
            {
            Sending = true;
            }
        }
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append(base.GetInspectString());
            stringBuilder.AppendLine();
            //stringBuilder.Append("Coutdown: " + counter + " ");
            stringBuilder.AppendLine();
            stringBuilder.Append("Status: ");
            if (powerComp.PowerOn && !Find.RoofGrid.Roofed(this.Position))
            {
                if (!Sending)
                {
                    stringBuilder.Append("On Standby");
                }
                else if (Sending)
                {
                    if (counter > 3000)
                    {
                        stringBuilder.Append("Encrypting Message");
                    }
                    if (counter < 3000 && counter > 1000)
                    {
                        stringBuilder.Append("Sending Message");
                    }
                    if (counter < 1000)
                    {
                        stringBuilder.Append(endText);
                    }

                    if (dotter > 40)
                    {
                        stringBuilder.Append(".");
                    }
                    if (dotter > 80)
                    {
                        stringBuilder.Append(".");
                    }
                    if (dotter > 120)
                    {
                        stringBuilder.Append(".");
                    }
                }
            }
            else if (!powerComp.PowerOn)
            {
                stringBuilder.Append("Offline");
            }
            else if (Find.RoofGrid.Roofed(this.Position))
            {
                stringBuilder.Append("Signal is blocked by roof");
            }
            stringBuilder.AppendLine();
            stringBuilder.Append("Power Needed: " + powerComp.powerOutput + " W");
            return stringBuilder.ToString();
        }
        private void EndText()
        {
            int endtxtroll = UnityEngine.Random.Range(0, 5);

            if (endtxtroll ==0)
            {
                endText = "Looking Busy";
            }

            if (endtxtroll == 1)
            {
                endText = "Playing chess with trans squierrl overlord";
            }

            if (endtxtroll == 2)
            {
                endText = "Updating profile on Ai_Singles.com";
            }

            if (endtxtroll == 3)
            {
                endText = "Trolling forums";
            }

            if (endtxtroll == 4)
            {
                endText = "Collecting dust";
            }
            if (endtxtroll == 5)
            {
                endText = "Looking Busy";
            }

        }
    }
}
