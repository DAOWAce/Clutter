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
    class RadarAntena : Building
    {
        private CompGlower glowerComp;
        private Material AntenaTex;
        private float CurRotationInt =0f;
        private float Rnum = 0.1f;
        public bool Rotation = false;
        public RadarUnit RadarBase
        {
            get
            {
                foreach (Thing current in Find.ThingGrid.ThingsAt(base.Position))
                {
                    RadarUnit radarBase = current as RadarUnit;
                    if (radarBase != null)
                    {
                        return radarBase;
                    }

                }
                return null;

            }
        }
        

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            AntenaTex = MaterialPool.MatFrom("Clutter/Devices/ComUnitTop", MatBases.Transparent);
            glowerComp = base.GetComp<CompGlower>();
            Thing RadBase;
            ThingDef RadBaseDef = ThingDef.Named("CommUnitBase");
            RadBase = Find.ThingGrid.ThingAt(base.Position, RadBaseDef);

            if (RadBase == null)
            {

                this.Destroy();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();


            Scribe_Values.LookValue<float>(ref CurRotationInt, "CurrentRotation");
            Scribe_Values.LookValue<bool>(ref Rotation, "Rotation");


        }

        public override void Tick()
        {
            base.Tick();
            Thing RadBase;
            ThingDef RadBaseDef = ThingDef.Named("CommUnitBase");
            RadBase = Find.ThingGrid.ThingAt(base.Position, RadBaseDef);

            if (RadBase == null)
            {

                this.Destroy();
            }
            if (RadBase != null)
            {


                if (Rotation)
                {
                    glowerComp.Lit = true;
                    CurRotationInt += Rnum;
                }
                else if (!Rotation)
                {
                    glowerComp.Lit = false;
                }
                if (RadarBase == null)
                {
                    Rotation = false;
                    this.Destroy();
                }
            }
        }

        public override void Draw()
        {

            
            Matrix4x4 matrix = default(Matrix4x4);
            Vector3 s = new Vector3(1.7f, 1f, 1.7f);
            matrix.SetTRS(DrawPos + Altitudes.AltIncVect, CurRotationInt.ToQuat(), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, DrawMat(rotation), 0);
        }
        
    }
}
