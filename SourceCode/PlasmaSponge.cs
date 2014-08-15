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
   public class PlasmaSponge : ThingWithComponents
    {
        
        private int lifeline;
        private bool bounce= false;
        private float grow;
        private int Tswitch = 0;
        private Material PlasmaF1;
        private Material PlasmaF2;
        private Material PlasmaF3;
        private CompGlower glowerComp;
        private Vector2 curDrawOffset = Vector2.zero;
        
        
       
        

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            FireKill();


            PlasmaF1 = MaterialPool.MatFrom("Clutter/Devices/IcePlasmaF1", true);
            PlasmaF2 = MaterialPool.MatFrom("Clutter/Devices/IcePlasmaF2", true);
            PlasmaF3 = MaterialPool.MatFrom("Clutter/Devices/IcePlasmaF3", true);
            glowerComp = base.GetComp<CompGlower>();
            glowerComp.Lit = true;
            this.grow = 0.1f;
            if (Find.ResearchManager.IsFinished(ResearchProjectDef.Named("AFSLife")))
            {
                this.lifeline = 1000;
            }
            else
            {
                this.lifeline = 300;
            }

            
            Vector3 spawnLoc = this.DrawPos * 0.1f;
            
            

            
            
            
            
        }


       public override Material DrawMat(IntRot rot)

        {
            
            {
               
                if (this.Tswitch<=20 )
                {
                    return (PlasmaF1);   
                }
                if (this.Tswitch >=21 && this.Tswitch <=40 )
                {
                    return (PlasmaF2);
                }
                else
                {
                    return (PlasmaF3);
                }

            }
        }
        
       
       
        public void FireKill()
        {
        
            List<Thing> list = new List<Thing>();
            foreach (Thing current in Find.ThingGrid.ThingsAt(base.Position))
            {
                if (current.def.eType == EntityType.Fire)
                {
                    list.Add(current);
                }
            }
            foreach (Thing current2 in list)
            {
                
                current2.Destroy(DestroyMode.Vanish);
            }
        }
        
        public override Vector3 DrawPos
        {
            get
            {
                return base.DrawPos + new Vector3(this.curDrawOffset.x, 0f, this.curDrawOffset.y) * this.grow;
            }
        }

        public float CurrentSize()
        {
            return this.grow;
        }

        public override void Draw()
        {
            float num = this.grow / 1.2f;
            if (num > 1.7f)
            {
                num = 1.7f;
            }
            Vector3 s = new Vector3(num, 1f, num);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(this.DrawPos, this.rotation.AsQuat, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, this.DrawMat(this.rotation), 0);
        }

        public override void Tick()
        {
            base.Tick();

                    
            
            
            if (lifeline <= 70)
            {
                this.bounce = true;
            }

            if (!this.bounce)
            {
                if (this.grow <= 1.2f)
                {
                    this.grow += 0.01f;
                }
            }
            if (this.bounce)
            {
                if(this.grow>=0f)
                {
                    this.grow -= 0.02f;
                }
            }
            
            
            if (this.Tswitch >=60)
            {
                this.Tswitch = 0;
            }

            if (Tswitch >=0 && Tswitch <1)
            {
                RefreshTextureState();
            }
            if (Tswitch >= 20 && Tswitch < 21)
            {
                RefreshTextureState();
            }
            if (Tswitch >= 40 && Tswitch < 41)
            {
                RefreshTextureState();
            }

            
            
            if (this.lifeline >0)
            {
                this.lifeline--;
                
                FireKill();
                this.Tswitch++;

            }
            if (this.lifeline <= 0)
            {
                this.glowerComp.Lit = false;
                this.Destroy(DestroyMode.Vanish);
            }
            
        }

        private void RefreshTextureState()
        {
            
            Find.MapDrawer.MapChanged(base.Position, MapChangeType.Things);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
                  
            
                stringBuilder.Append("Reversed High Resonation Plasma Cloud");
                stringBuilder.AppendLine();
                stringBuilder.Append("Alias:The Sponge");
                
           
            return stringBuilder.ToString();
        }

        

    }
 }
