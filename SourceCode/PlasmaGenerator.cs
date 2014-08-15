using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using VerseBase;

namespace Clutter
{
    public class PlasmaGen : Building
     {
        private int counter = 10050;
        private int time = 12500;
        private int phase = 0;
        private CompGlower glowerComp;
        private CompPowerTrader powerComp;
        private static readonly SoundDef SoundHiss = SoundDef.Named("PowerOn");
       
        

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            this.powerComp = base.GetComp<CompPowerTrader>();
            this.glowerComp = base.GetComp<CompGlower>();
            this.powerComp.powerOutput = 0;
            
           
            
           
           
            
            

        }
        public override void Tick()
        {
            base.Tick();
            if (this.powerComp.PowerOn)
            {
                
                this.glowerComp.Lit = true;
                this.time--;
                this.phase++;
                


                if (this.time <= 12500 && this.time >= 2500)
                {
                    counter--;
                    this.powerComp.powerOutput = -40+(this.phase)/250;
                    
                }
                if (this.time <= 2250 && this.time >= 750)
                {
                    this.powerComp.powerOutput = 400;
                    
                }

                if (this.time <=1 && this.phase > 50)
                {
                    
                    this.powerComp.powerOutput = 0;
                    this.phase = 0;
                    this.time = 12500;
                    this.counter = 10050;
                }
               

            }
            else
            {

                this.glowerComp.Lit = false;
                this.time = 12500;
                this.phase = 0;
                this.powerComp.powerOutput = 0;
                this.counter = 10050;
            }

        }
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.Append("Power output: " +powerComp.powerOutput+ " W");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.Append("Status:");

            if (powerComp.PowerOn && time <= 12500 && time >= 2500)
            {
                stringBuilder.Append(" Charging ");
            }

            if (powerComp.PowerOn && time <= 2250 && time >= 750)
            {
                stringBuilder.Append(" Discharging");
            }
            if (!powerComp.PowerOn)
            {
                stringBuilder.Append(" Offline");
            }

            return stringBuilder.ToString();
        }
    }

}
