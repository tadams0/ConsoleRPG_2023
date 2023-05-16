using ConsoleRPG_2023.RolePlayingGame.Combat;
using ConsoleRPG_2023.RolePlayingGame.Effects;
using ConsoleRPG_2023.RolePlayingGame.Effects.EffectObjects;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Text;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    /// <summary>
    /// Defines a class speciallized to render characters.
    /// </summary>
    public class EffectCloudRenderer : MapObjectRenderer
    {
        public static readonly string cloudDisplayChar100 = "▓";
        public static readonly string cloudDisplayChar75 = "▓";
        public static readonly string cloudDisplayChar50 = "▒";
        public static readonly string cloudDisplayChar25 = "░";

        private Dictionary<DamageType, List<Color>> cloudColorsByDamageType = new Dictionary<DamageType, List<Color>>();

        private List<Color> defaultCloudColors = new List<Color>(4);

        private static Random rand = new Random();

        public EffectCloudRenderer() 
        {
            renderedType = typeof(EffectCloud);

            defaultCloudColors.Add(Color.FromArgb(245, 245, 245));
            defaultCloudColors.Add(Color.FromArgb(200, 200, 200));
            defaultCloudColors.Add(Color.FromArgb(170, 170, 170));
            defaultCloudColors.Add(Color.FromArgb(120, 120, 120));

            cloudColorsByDamageType[DamageType.Fire] = new List<Color>()
            {
                Color.FromArgb(255, 95, 25),
                Color.FromArgb(255, 125, 70),
                Color.FromArgb(255, 150, 110),
                Color.FromArgb(255, 175, 145)
            };

            cloudColorsByDamageType[DamageType.Poison] = new List<Color>()
            {
                Color.FromArgb(70, 255, 64),
                Color.FromArgb(80, 255, 80),
                Color.FromArgb(106, 252, 100),
                Color.FromArgb(149, 255, 145)
            };

            cloudColorsByDamageType[DamageType.Corruption] = new List<Color>()
            {
                Color.FromArgb(150, 31, 255),
                Color.FromArgb(167, 66, 255),
                Color.FromArgb(185, 102, 255),
                Color.FromArgb(200, 140, 255)
            };

            cloudColorsByDamageType[DamageType.Frost] = new List<Color>()
            {
                Color.FromArgb(158, 253, 255),
                Color.FromArgb(190, 254, 255),
                Color.FromArgb(224, 254, 255),
                Color.FromArgb(242, 255, 255)
            };

            cloudColorsByDamageType[DamageType.None] = defaultCloudColors;
        }

        public override string GetDisplayCharacter(MapObject obj, Tile tile, GameState state)
        {
            EffectCloud cloud = obj as EffectCloud;
            if (cloud != null)
            {
                double lifeTimePercent = 1 - (double)cloud.CurrentLifeTime / cloud.MaxLifetime;
                if (lifeTimePercent > 0.75 || cloud.MaxLifetime <= 1)
                {
                    return cloudDisplayChar100;
                }
                else if (lifeTimePercent > 0.50)
                {
                    return cloudDisplayChar75;
                }
                else if (lifeTimePercent > 0.25)
                {
                    return cloudDisplayChar50;
                }
                else
                {
                    return cloudDisplayChar25;
                }
            }

            return cloudDisplayChar100;
        }

        public override ConsoleColor GetDisplayColor(MapObject obj, Tile tile, GameState state)
        {
            return ConsoleColor.Gray;
        }

        public override string GetForegroundColor(MapObject obj, Tile tile, GameState state)
        {
            Color c = Color.White;
            EffectCloud cloud = obj as EffectCloud;
            if (cloud != null)
            {
                double lifeTimePercent = 1 - ((double)cloud.CurrentLifeTime / cloud.MaxLifetime);
                int index = 0;
                if (lifeTimePercent > 0.75 || cloud.MaxLifetime <= 1)
                {
                    index = 0;
                }
                else if (lifeTimePercent > 0.50)
                {
                    index = 1;
                }
                else if (lifeTimePercent > 0.25)
                {
                    index = 2;
                }
                else
                {
                    index = 3;
                }
                List<Color> colorList = defaultCloudColors;

                if (cloud.AppliedEffects.Count > 0)
                { //If there is more than 1 effect..
                    //Cycle through the effects
                    Effect e = cloud.AppliedEffects[index % cloud.AppliedEffects.Count];

                    //Get the color associated with the effect
                    c = GetColorForEffect(e, index);
                }
                else
                {
                    c = colorList[index];
                }
            }
            return VirtualConsoleSequenceBuilder.GetColorForegroundSequence(c);
        }

        /// <summary>
        /// Converts the given effect into a corresponding color.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="index">The index along the progress of the effect cloud.</param>
        /// <returns></returns>
        private Color GetColorForEffect(Effect e, int index)
        {
            DamageEffect damageEffect = e as DamageEffect;
            if (damageEffect != null)
            {
                DamageType type = damageEffect.DamageTypes[rand.Next(0, damageEffect.DamageTypes.Count)];

                return GetColorForDamageType(type, index);
            }
            else
            {//For now, we just return the default color set for non damage effects.
                return defaultCloudColors[index];
            }
        }

        private Color GetColorForDamageType(DamageType t, int index)
        {
            return cloudColorsByDamageType[t][index];
        }

    }
}
