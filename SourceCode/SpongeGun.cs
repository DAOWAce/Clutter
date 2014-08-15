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
    public class SpongeGun : Verb_LaunchProjectile
    {


       private int AmmoCout = 5;
       private Pawn pawn;
       public Equipment primary;
       //Thing SpongeGunX;
       private int boom = UnityEngine.Random.Range(0, 3);
       public override string InfoTextFull
       {
           get
           {
               StringBuilder stringBuilder = new StringBuilder();
               stringBuilder.Append(this.verbProps.description);
               stringBuilder.AppendLine();
               stringBuilder.AppendLine();
               stringBuilder.Append("Damage: " + this.verbProps.projectileDef.projectile.damageAmountBase);
               stringBuilder.AppendLine();
               stringBuilder.Append("Range: " + this.verbProps.range);
               stringBuilder.AppendLine();
               stringBuilder.Append("Accuracy: " + this.verbProps.AccuracySummaryString);
               stringBuilder.AppendLine();
               if (this.verbProps.burstShotCount > 1)
               {
                   stringBuilder.Append("Automatic fire. Shots per burst: " + this.verbProps.burstShotCount);
               }
               else
               {
                   stringBuilder.Append("Single-shot");
               }
               stringBuilder.AppendLine();
               stringBuilder.Append("Aim time: " + this.verbProps.warmupTicks.TickstoSecondsString());
               return stringBuilder.ToString();
           }
       }
		protected override int ShotsPerBurst
		{
			get
			{
				return this.verbProps.burstShotCount;
			}
		}
		protected override void InitCast()
		{
			base.InitCast();
            
			if (base.OwnerIsPawn && base.OwnerPawn.skills != null && this.AmmoCout >0)
			{
				float xp = 10f;
				if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.eType == EntityType.Pawn)
				{
					if (this.currentTarget.Thing.HostileTo(this.owner))
					{
						xp = 200f;
					}
					else
					{
						xp = 50f;
					}
				}
				base.OwnerPawn.skills.Learn(SkillDefOf.Shooting, xp);
			}
		}
		protected override bool TryShotSpecialEffect()
		{
            if (this.AmmoCout <= 1)
                {
                    this.equipment.verb.castCompleteCallback = new Action(this.BurstComplete);
                }
            if (this.AmmoCout >0)
            {
                this.AmmoCout -= 1;
                
			    if (base.TryShotSpecialEffect())
			    {
                  MoteMaker.ThrowFlash(this.owner.Position, "ShotFlash", 9f);
			    	return true;
			    }
               
            }

			return false;
		}
        public void BurstComplete()
        {

            
            if (boom <= 0)
            {
                this.owner.TryIgnite(1.2f);
            }
            this.pawn = this.OwnerPawn;
            //this.pawn.equipment.DropAllEquipment();
            this.pawn.equipment.TryDropEquipment(this.equipment, out this.equipment, this.owner.Position,false);
            
            this.equipment.Destroy();
            //SpongeGunX = Find.ThingGrid.ThingAt(this.owner.Position, ThingDef.Named("SSGun_Pistol"));
            //if (SpongeGunX != null)
           // {
            //    SpongeGunX.Destroy();
           // }
           
            
        }
	}
}


