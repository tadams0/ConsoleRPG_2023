using ConsoleRPG_2023.RolePlayingGame.Maps;
using System;
using System.Collections.Generic;
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
            EffectFunctionManager.AddAction(HealHealthEffect, "HEAL_HEALTH");
            EffectFunctionManager.AddAction(HealStaminaEffect, "HEAL_STAMINA");
            EffectFunctionManager.AddAction(TargetNearbyCharacters, "TARGET_NEARBY_CHARACTERS");
            EffectFunctionManager.AddAction(TargetPlayer, "TARGET_PLAYER");
            EffectFunctionManager.AddAction(TargetNearbyTrees, "TARGET_NEARBY_TREES");
            EffectFunctionManager.AddAction(MapRemoval, "MAP_REMOVAL");
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
                    character.Health = Math.Clamp((int)(character.Health + activeEffect.Effect.Magnitude), character.MinHealth, character.MaxHealth);
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
                    character.Stamina = Math.Clamp((int)(character.Stamina + activeEffect.Effect.Magnitude), character.MinStamina, character.MaxStamina);
                }
            }
        }

        /// <summary>
        /// Defines an effect action method for targeting any characters within the effect range.
        /// </summary>
        public static void TargetNearbyCharacters(ActiveEffect activeEffect, GameState state, Map map)
        {
            //Note that currently this selects characters in a square pattern. Maybe in the future this can be changed to a circular selection.

            List<GameObject> targets = new List<GameObject>();
            Character character = null;
            int radius = activeEffect.Effect.Range;
            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
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
            //Note that currently this selects characters in a square pattern. Maybe in the future this can be changed to a circular selection.

            List<GameObject> targets = new List<GameObject>();
            MapTree tree = null;
            int radius = activeEffect.Effect.Range;
            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
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
        /// Defines an effect action method for targeting the player character.
        /// </summary>
        public static void TargetPlayer(ActiveEffect activeEffect, GameState state, Map map)
        {
            List<GameObject> targets = new List<GameObject>();
            targets.Add(state.PlayerCharacter);
            activeEffect.Targets = targets;
        }

    }
}
