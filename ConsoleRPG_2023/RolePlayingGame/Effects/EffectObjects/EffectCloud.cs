using ConsoleRPG_2023.RolePlayingGame.Combat;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Effects.EffectObjects
{
    public class EffectCloud : MapObject, IUpdatable
    {
        public int MaxLifetime { get; set; }

        public int CurrentLifeTime { get; set; }

        /// <summary>
        /// The effect applied to characters that step into the region.
        /// </summary>
        public List<Effect> AppliedEffects { get; set; }

        /// <summary>
        /// The effect which spawned this cloud. Use null if no such effect was used.
        /// </summary>
        public Effect SpawningEffect { get; set; }

        public EffectCloud(int maxLifeTime, List<Effect> appliedEffects, long x, long y)
        {
            this.X = x;
            this.Y = y;
            CurrentLifeTime = 0;
            MaxLifetime = maxLifeTime;
            AppliedEffects = appliedEffects;
        }

        public void Update(GameState state, Map map)
        {
            CurrentLifeTime++;
            if (MaxLifetime > 0 && CurrentLifeTime > MaxLifetime)
            {//Remove itself if its time is up.
                map.RemoveObject(this);
            }
            else
            {//Damage any characters within its boundary.
                List<MapObject> objectsWithin = map.GetObjects(this.X, this.Y);
                Character c;
                foreach (MapObject o in objectsWithin)
                {
                    c = o as Character;
                    if (c != null && AppliedEffects.Count > 0)
                    {
                        foreach (var effect in AppliedEffects)
                        {
                            map.AddActiveEffectToObject(state, c, effect);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            if (AppliedEffects.Count == 1)
            {
                Effect firstEffect = AppliedEffects[0];
                string adjective = string.Empty;

                DamageEffect damageEffect = firstEffect as DamageEffect;
                if (damageEffect != null)
                {
                    string damageAdj;
                    foreach (DamageType type in damageEffect.DamageTypes)
                    {
                        damageAdj = Helper.DamageTypeToAdjective(type);
                        adjective += Helper.FirstCharToUpper(damageAdj) + " ";
                    }
                }
                else
                {
                    adjective = "Effect ";
                }

                return $"{adjective}Cloud";
            }
            else
            {
                return "Multi-Effect Cloud";
            }
        }

    }
}
