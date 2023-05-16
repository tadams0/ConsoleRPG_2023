using ConsoleRPG_2023.RolePlayingGame.Combat;
using ConsoleRPG_2023.RolePlayingGame.Effects.EffectObjects;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Effects
{
    /// <summary>
    /// Defines a class which initializes and creates various effect functions and populates the <see cref="EffectFunctionManager"/> with them.
    /// </summary>
    internal static class EffectFunctionCreator
    {

        public static void Init()
        {
            //Non-Targeting effects
            EffectFunctionManager.AddAction(HealHealthEffect, "HEAL_HEALTH");
            EffectFunctionManager.AddAction(HealStaminaEffect, "HEAL_STAMINA");
            EffectFunctionManager.AddAction(EffectCloudEffect, "EFFECT_CLOUD");
            EffectFunctionManager.AddAction(MapRemoval, "MAP_REMOVAL");
            EffectFunctionManager.AddAction(ApplyDamageEffect, "APPLY_DAMAGE");

            //Targeting
            EffectFunctionManager.AddAction(TargetNearbyCharacters, "TARGET_NEARBY_CHARACTERS");
            EffectFunctionManager.AddAction(TargetPlayer, "TARGET_PLAYER");
            EffectFunctionManager.AddAction(TargetNearbyTrees, "TARGET_NEARBY_TREES");
            EffectFunctionManager.AddAction(TargetCharactersInActivationTile, "TARGET_CHARACTERS_ON_TILE");
            EffectFunctionManager.AddAction(TargetTrigger, "TARGET_TRIGGER");
        }

        /// <summary>
        /// Defines a basic action for removing all targets from the map.
        /// </summary>
        public static void MapRemoval(ActiveEffect activeEffect, GameState state, Map map)
        {
            MapObject mapObj;
            foreach (var target in activeEffect.Targets)
            {
                mapObj = target as MapObject;
                if (mapObj != null)
                {
                    map.RemoveObject(mapObj);
                }
            }
        }

        /// <summary>
        /// Defines a basic effect action method for healing any effect target character's health.
        /// </summary>
        public static void HealHealthEffect(ActiveEffect activeEffect, GameState state, Map map) 
        { 
            Character character = null;
            foreach (var target in activeEffect.Targets)
            {
                character = target as Character;
                if (character != null)
                {
                    character.SetHealth((int)(character.Health + activeEffect.Effect.Magnitude));
                }
            }
        }

        /// <summary>
        /// Defines a basic effect action method for healing any effect target character's stamina.
        /// </summary>
        public static void HealStaminaEffect(ActiveEffect activeEffect, GameState state, Map map)
        {
            Character character = null;
            foreach (var target in activeEffect.Targets)
            {
                character = target as Character;
                if (character != null)
                {
                    character.SetStamina((int)(character.Stamina + activeEffect.Effect.Magnitude));
                }
            }
        }

        /// <summary>
        /// Defines a basic effect action method for applying damage to targeting characters.
        /// </summary>
        public static void ApplyDamageEffect(ActiveEffect activeEffect, GameState state, Map map)
        {
            Character character = null;
            foreach (var target in activeEffect.Targets)
            {
                character = target as Character;
                if (character != null)
                {
                    List<DamageType> damageTypes = new List<DamageType>();
                    DamageEffect effect = activeEffect.Effect as DamageEffect;
                    if (effect != null)
                    {
                        damageTypes = effect.DamageTypes;
                    }
                    else
                    {//Default to "None" type damage.
                        damageTypes.Add(DamageType.None);
                    }

                    character.ApplyDamage(activeEffect.Effect.Magnitude, damageTypes);
                }
            }
        }

        /// <summary>
        /// Defines an effect action method for targeting characters on the exact tile that the activation of the effect occured on.
        /// </summary>
        public static void TargetCharactersInActivationTile(ActiveEffect activeEffect, GameState state, Map map)
        {
            List<GameObject> targets = new List<GameObject>();

            long x = activeEffect.ActivateLocation.X;
            long y = activeEffect.ActivateLocation.Y;

            List<MapObject> foundObjects = map.GetObjects(x, y);
            Character character = null;
            foreach (var obj in foundObjects)
            {
                character = obj as Character;
                if (character != null)
                {
                    targets.Add(character);
                }
            }

            activeEffect.Targets = targets;
        }

        /// <summary>
        /// Defines an effect action method for targeting any characters within the effect range.
        /// </summary>
        public static void TargetNearbyCharacters(ActiveEffect activeEffect, GameState state, Map map)
        {
            //Note that currently this selects characters in a square pattern. Maybe in the future this can be changed to a circular selection.

            List<GameObject> targets = new List<GameObject>();
            Character character = null;
            int radiusX = activeEffect.Effect.RangeX;
            int radiusY = activeEffect.Effect.RangeY;
            for (int i = -radiusX; i < radiusX; i++)
            {
                for (int j = -radiusY; j < radiusY; j++)
                {
                    long x = i + activeEffect.ActivateLocation.X;
                    long y = j + activeEffect.ActivateLocation.Y;

                    List<MapObject> foundObjects = map.GetObjects(x, y);
                    foreach (var obj in foundObjects)
                    {
                        character = obj as Character;
                        if (character != null)
                        {
                            targets.Add(character);
                        }
                    }
                }
            }

            activeEffect.Targets = targets;
        }

        /// <summary>
        /// Defines an effect action method for targeting any trees within the effect range.
        /// </summary>
        public static void TargetNearbyTrees(ActiveEffect activeEffect, GameState state, Map map)
        {
            List<GameObject> targets = new List<GameObject>();
            MapTree tree = null;
            int radiusX = activeEffect.Effect.RangeX;
            int radiusY = activeEffect.Effect.RangeY;
            for (int i = -radiusX; i < radiusX; i++)
            {
                for (int j = -radiusY; j < radiusY; j++)
                {
                    long x = i + activeEffect.ActivateLocation.X;
                    long y = j + activeEffect.ActivateLocation.Y;

                    List<MapObject> foundObjects = map.GetObjects(x, y);
                    foreach (var obj in foundObjects)
                    {
                        tree = obj as MapTree;
                        if (tree != null)
                        {
                            targets.Add(tree);
                        }
                    }
                }
            }

            activeEffect.Targets = targets;
        }


        /// <summary>
        /// Adds the triggerer of the effect as the target.
        /// </summary>
        public static void TargetTrigger(ActiveEffect activeEffect, GameState state, Map map)
        {
            if (activeEffect.Trigger != null)
            {
                MapObject mapObject = activeEffect.Trigger as MapObject;
                if (mapObject != null)
                {
                    activeEffect.Targets = new List<GameObject>
                    {
                        mapObject
                    };
                }
            }
        }

        /// <summary>
        /// Defines an effect action method for targeting the player character.
        /// </summary>
        public static void TargetPlayer(ActiveEffect activeEffect, GameState state, Map map)
        {
            List<GameObject> targets = new List<GameObject>();
            targets.Add(state.PlayerCharacter);
            activeEffect.Targets = targets;
        }

        /// <summary>
        /// Defines an effect action method for targeting any characters within the effect range.
        /// </summary>
        public static void EffectCloudEffect(ActiveEffect activeEffect, GameState state, Map map)
        {
            Random r = new Random();

            List<PointL> castingLocations = new List<PointL>();
            if (activeEffect.Targets.Count > 0)
            {//If we have at least one target.

                foreach (var target in activeEffect.Targets)
                {
                    MapObject mapObjectTarget = target as MapObject;
                    if (mapObjectTarget != null)
                    {
                        PointL newLocation = new PointL(mapObjectTarget.X, mapObjectTarget.Y);
                        castingLocations.Add(newLocation);
                    }
                }
            }
            else
            {//If there were no targets, then we can simply use the activation location
                castingLocations.Add(activeEffect.ActivateLocation);
            }

            int radiusX = activeEffect.Effect.RangeX;
            int radiusY = activeEffect.Effect.RangeY;
            MultiMagnitudeEffect mmEffect = activeEffect.Effect as MultiMagnitudeEffect;
            int cloudLifeTime = 1;
            if (mmEffect != null && mmEffect.AdditionalMagnitudes.Count > 0)
            {
                cloudLifeTime = (int)mmEffect.AdditionalMagnitudes[0];
            }

            foreach (var location in castingLocations)
            {
                PointL centerPoint = new PointL(location.X, location.Y);

                for (int i = -radiusX; i < radiusX; i++)
                {
                    for (int j = -radiusY; j < radiusY; j++)
                    {
                        long x = i + centerPoint.X;
                        long y = j + centerPoint.Y;

                        int cloudChance = r.Next(1, 101);
                        if (cloudChance <= activeEffect.Effect.Magnitude)
                        {
                            EffectCloud cloud = new EffectCloud(cloudLifeTime, activeEffect.Effect.SubEffects, x, y);
                            cloud.SpawningEffect = activeEffect.Effect;

                            //Find and remove any existing or still persistent clouds
                            List<MapObject> existingObjects = map.GetObjects(x, y);
                            if (existingObjects != null && existingObjects.Count > 0)
                            {
                                EffectCloud oldCloud;
                                foreach (MapObject obj in existingObjects)
                                {
                                    oldCloud = obj as EffectCloud;
                                    if (oldCloud != null && cloud.SpawningEffect == oldCloud.SpawningEffect)
                                    {//If an old cloud exists and it comes from the same effect, then remove it.
                                        map.RemoveObject(obj);
                                    }
                                }
                            }

                            //Add the new cloud.
                            map.AddObject(cloud);
                        }
                    }
                }
            }
        }

    }
}
