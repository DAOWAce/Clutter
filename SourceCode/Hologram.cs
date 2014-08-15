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
    class Hologram : ThingWithComponents
    {
        public HoloStatus holostatus = HoloStatus.piramid;
        private static Texture2D UI_Holo1;
        private static Texture2D UI_Holo2;
        private static Texture2D UI_Holo3;
        private static Texture2D UI_Rot1;
        private static Texture2D UI_Rot2;
        private Texture2D Uisave;
        private Material Holo1;
        private Material Holo2;
        private Material Holo3;
        private Material HoloSave;
        private bool SpawnSec = true;
        private bool DespawnSec = false;
        private Vector2 curDrawOffset = Vector2.zero;
        public bool Rswitch = false;
        private float CurRotationInt;
        private CompGlower glowerComp;
        private float grow;
        private float rnum;
        private bool DestroySec = false;
        private int Boom = 1;
        private bool loaded = false;
        

        //public HoloBase MyEmitter;
        public HoloBase HoloEmiter // making a connection i hope, if not its not needed
        {
            get
            {
                foreach (Thing current in Find.ThingGrid.ThingsAt(base.Position))
                {
                    HoloBase BaseEmitter = current as HoloBase;
                    if (BaseEmitter != null)
                    {
                        return BaseEmitter;
                    }

                }
                return null;

            }
        }


        //Wierd list thingy
        public enum HoloStatus
        {
            recalibrating,
            plant,
            piramid,
            balls
        }


        //Stuff on spawn
        public override void SpawnSetup()
        {
            base.SpawnSetup();


            //UpdateHoloStatus();
            //holostatus = HoloStatus.piramid;
            Hologram.UI_Rot1 = ContentFinder<Texture2D>.Get("Clutter/Ui/Ui_Rotation_On", true);
            Hologram.UI_Rot2 = ContentFinder<Texture2D>.Get("Clutter/Ui/Ui_Rotation_Off", true);
            Hologram.UI_Holo3 = ContentFinder<Texture2D>.Get("Clutter/Holo/Holo_Object2", true);
            Hologram.UI_Holo1 = ContentFinder<Texture2D>.Get("Clutter/Holo/Holo_Object1", true);
            Hologram.UI_Holo2 = ContentFinder<Texture2D>.Get("Clutter/Holo/Holo_plant", true);
            Holo1 = MaterialPool.MatFrom("Clutter/Holo/Holo_Object1", MatBases.Transparent);
            Holo2 = MaterialPool.MatFrom("Clutter/Holo/Holo_plant", MatBases.Transparent);
            Holo3 = MaterialPool.MatFrom("Clutter/Holo/Holo_Object2", MatBases.Transparent);

            if (loaded)
            {
                UpdateHoloStatus();
                return;
            }

            if (HoloSave == null)
            {
                HoloSave = Holo1;
                Uisave = UI_Holo1;
            }
            glowerComp = base.GetComp<CompGlower>();
            glowerComp.Lit = true;
            if (HoloEmiter != null)
            {
                InfoFromBase();
            }


        }


        //Save stuff
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<float>(ref CurRotationInt, "CurrentRotationAngel");
            Scribe_Values.LookValue<bool>(ref Rswitch, "CurrentTotationStatus");
            Scribe_Values.LookValue<bool>(ref SpawnSec, "SpawnSequence");
            Scribe_Values.LookValue<bool>(ref DespawnSec, "DespawnSequence");
            Scribe_Values.LookValue<HoloStatus>(ref holostatus, "ImageStatus");

            loaded = true;
        }


        //list thingy variables
        private void UpdateHoloStatus()
        {
            switch (holostatus)
            {

                case HoloStatus.piramid:
                    HoloSave = Holo1;
                    Uisave = UI_Holo2;
                    rnum = 0.21f;
                    break;

                case HoloStatus.plant:
                    HoloSave = Holo2;
                    Uisave = UI_Holo3;
                    rnum = 0.09f;
                    break;

                case HoloStatus.balls:
                    HoloSave = Holo3;
                    Uisave = UI_Holo1;
                    rnum = 0.30f;
                    break;
            }
        }

        //Holobase update with wierd list variables
        private void UpdateHoloBaseStatus()
        {
           
            

            if (holostatus == HoloStatus.piramid)
            {
                HoloEmiter.savestat = 0;
            }
            else if (holostatus == HoloStatus.plant)
            {
                HoloEmiter.savestat = 1;
            }
            else if (holostatus == HoloStatus.balls)
            {
                HoloEmiter.savestat = 2;
            }
            if (Rswitch)
            {
                HoloEmiter.Rsave = true;
            }
            else if (!Rswitch)
            {
                HoloEmiter.Rsave = false;
            }
            HoloEmiter.DataRecived = true;
        }

        //Info from holobase
        private void InfoFromBase()
        {
            
            if (HoloEmiter.savestat == 0)
            {
                holostatus = HoloStatus.piramid;
            }
            if (HoloEmiter.savestat == 1)
            {
                holostatus = HoloStatus.plant;
            }
            if (HoloEmiter.savestat == 2)
            {
                holostatus = HoloStatus.balls;
            }
            if (HoloEmiter.Rsave == true)
            {
                Rswitch = true;
            }
            if (HoloEmiter.Rsave == false)
            {
                Rswitch = false;
            }
        }

        //Despawning sequence 
        private void DespawningSec()
        {

            if (grow < 0.1f)
            {
                DespawnSec = false;
                SpawnSec = true;
                // UpdateHoloBaseStatus();
            }
            else if (grow >= 0.1f)
            {
                grow -= 0.05f;
            }

        }

        //Spawning sequence
        private void SpawningSec()
        {
            UpdateHoloStatus();
            if (grow >= 1.2f)
            {
                grow = 1.2f;
                SpawnSec = false;
                // UpdateHoloBaseStatus();
            }
            else
            {
                grow += 0.05f;
            }
        }

        //Destroy sequence
        private void HoloDestroySec()
        {
            if (grow < 0.1f)
            {
                Boom = 0;
            }
            else if (grow >= 0.1f)
            {
                grow -= 0.05f;
            }
        }



        // Normal Ticks
        public override void Tick()
        {
            loaded = false;


            // Destroing is initiated => Try to destroy
            if (DespawnSec && DestroySec && !SpawnSec)
            {
                Destroy(DestroyMode.Vanish);
                //return;
            }

            base.Tick();


            SelfOwn();
            if (DespawnSec)
            {
                if (holostatus == HoloStatus.piramid || holostatus == HoloStatus.plant || holostatus == HoloStatus.balls)
                {
                    DespawningSec();

                }
            }

            if (!DespawnSec && SpawnSec)
            {
                if (holostatus == HoloStatus.piramid || holostatus == HoloStatus.plant || holostatus == HoloStatus.balls)
                {
                    SpawningSec();
                }

            }

            if (Rswitch)
            {
                if (holostatus == HoloStatus.piramid || holostatus == HoloStatus.plant || holostatus == HoloStatus.balls)
                {
                    CurRotationInt += rnum;
                }

            }
            if (!Rswitch)
            {
                CurRotationInt = 0;
            }
            if (DestroySec)
            {
                HoloDestroySec();
            }


        }


        //Buttons
        public override IEnumerable<Command> GetCommands()
        {
            IList<Command> list = new List<Command>();
            Command_Action ca0 = new Command_Action();
            ca0.icon = Uisave;
            ca0.defaultDesc = "Switch Image";
            ca0.activateSound = SoundDef.Named("Click");
            ca0.action = new Action(TexSwitch);
            ca0.hotKey = KeyCode.F;
            ca0.groupKey = 887761181;
            list.Add(ca0);

            Command_Action ca1 = new Command_Action();
            if (!Rswitch)
            {
                ca1.icon = UI_Rot1;
            }
            else
            {
                ca1.icon = UI_Rot2;
            }
            ca1.defaultDesc = "Toggle rotation";
            ca1.activateSound = SoundDef.Named("Click");
            ca1.action = new Action(RotSwitch);
            ca1.hotKey = KeyCode.X;
            ca1.groupKey = 887761189;
            list.Add(ca1);

            IEnumerable<Command> commands = base.GetCommands();
            IEnumerable<Command> result;
            if (commands != null)
                result = list.AsEnumerable<Command>().Concat(commands);
            else
                result = list.AsEnumerable<Command>();
            return result;
        }

        //Undies in other words
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            stringBuilder.Append("Active image:");

            if (holostatus == HoloStatus.plant)
            {
                stringBuilder.Append("Plant");
            }
            else if (holostatus == HoloStatus.balls)
            {
                stringBuilder.Append("Balls");
            }
            else if (holostatus == HoloStatus.piramid)
            {
                stringBuilder.Append("Piramid");
            }
            else //error???
            {
                stringBuilder.Append("Undefined");
            }

            stringBuilder.AppendLine();
            stringBuilder.Append("Rotation: ");
            if (Rswitch)
                stringBuilder.Append("On");
            else
                stringBuilder.Append("Off");

            return stringBuilder.ToString();
        }


        //Vector 3 for growing
        public override Vector3 DrawPos
        {
            get
            {
                return base.DrawPos + new Vector3(curDrawOffset.x, 0f, curDrawOffset.y) * grow;
            }
        }

        //Texture drawing 
        public override void Draw()
        {


            Matrix4x4 matrix = default(Matrix4x4);

            if (SpawnSec || DespawnSec)
            {
                float num = grow / 1.2f;
                if (num > 1.2f)
                {
                    num = 1.2f;
                }
                // Used for grow
                Vector3 s = new Vector3(num, 1f, num);
                matrix.SetTRS(DrawPos, rotation.AsQuat, s);
            }
            if (!SpawnSec && !DespawnSec)
            {
                // Used for rotation
                Vector3 s = new Vector3(1f, 1f, 1f);
                matrix.SetTRS(DrawPos + Altitudes.AltIncVect, CurRotationInt.ToQuat(), Vector3.one);
            }

            Graphics.DrawMesh(MeshPool.plane10, matrix, DrawMat(rotation), 0);
        }

        // Texture material
        public override Material DrawMat(IntRot rot)
        {
            return HoloSave;
        }

        // Something to do with Draw ...
        public float CurrentSize()
        {
            return grow;
        }

        // State switcher for wierd list
        private void TexSwitch()
        {

            if (holostatus == HoloStatus.plant)
            {
                holostatus = HoloStatus.balls;
            }
            else if (holostatus == HoloStatus.balls)
            {
                holostatus = HoloStatus.piramid;
            }
            else if (holostatus == HoloStatus.piramid)
            {
                holostatus = HoloStatus.plant;
            }
            DespawnSec = true;
            UpdateHoloBaseStatus();


        }

        // Rotation toggle
        private void RotSwitch()
        {

            Rswitch = !Rswitch;
            UpdateHoloBaseStatus();

        }


        //Scanner for holo base
        public void SelfOwn()
        {

            Thing Suicade;
            Suicade = Find.ThingGrid.ThingAt(base.Position, ThingDef.Named("HoloEmitter"));
            if (Suicade == null)
            {
                Destroy(DestroyMode.Vanish);
            }
        }

        //Destroy
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {

            DespawnSec = true;
            DestroySec = true;
            SpawnSec = false;

            if (Boom <= 0)
            {
                base.Destroy(mode = 0);
            }
        }
    }
}