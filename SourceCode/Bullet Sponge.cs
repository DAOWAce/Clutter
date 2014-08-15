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
    public class Bullet_Sponge : Projectile
    {
        private const float StunChance = 0.1f;
        private Vector2 curDrawOffset = Vector2.zero;
       
       

        protected override void Impact(Thing hitThing)
        {
            landed = true;


            base.Impact(hitThing);
            ThingDef spongeDef = ThingDef.Named("PlasmaSponge");
            IntVec3 Bloc = this.Position + IntVec3.zero;

           
            GenSpawn.Spawn(spongeDef, Bloc);
        }
    }
}
