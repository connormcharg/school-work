using CheckAndMate.Shared.Chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Utilities
{
    public static class Util
    {
        public static string GenerateNickname()
        {
            var rng = new Random();
            return $"{GetRandomWord()}{GetRandomWord()}{rng.Next(10, 100)}";
        }

        private static string GetRandomWord()
        {
            List<string> words = new List<string>
            {
                "Abyss", "Acorn", "Adventure", "Agent", "Alien", "Alpha", "Amulet", "Anvil", "Arcade", "Arrow", "Astro",
                "Aurora", "Axe", "Bandit", "Basilisk", "Beacon", "Beetle", "Blaze", "Blizzard", "Bolt", "Boomer",
                "Bounce", "Brave", "Breeze", "Brimstone", "Bronze", "Buffalo", "Cactus", "Captain", "Carnival",
                "Charger", "Chimera", "Cinder", "Clash", "Cloud", "Comet", "Cosmic", "Coyote", "Crane", "Crest",
                "Crimson", "Crypt", "Cyclone", "Dagger", "Dawn", "Daybreak", "Demon", "Desert", "Dew", "Diamond",
                "Diesel", "Dingo", "Doom", "Dragon", "Drake", "Dune", "Eagle", "Echo", "Eclipse", "Edge", "Elixir",
                "Ember", "Falcon", "Fang", "Fiend", "Fire", "Flame", "Flash", "Flint", "Frost", "Fury", "Galaxy",
                "Gale", "Ghost", "Glacier", "Glimmer", "Goblin", "Golden", "Griffin", "Grim", "Guardian", "Hammer",
                "Hawk", "Helix", "Hollow", "Horizon", "Hornet", "Hurricane", "Hydra", "Ice", "Inferno", "Ivy",
                "Jaguar", "Jester", "Jolt", "Jungle", "Knight", "Kraken", "Lance", "Laser", "Legend", "Lightning",
                "Lion", "Lunar", "Magnet", "Mammoth", "Mask", "Maverick", "Maze", "Meteor", "Midnight", "Mirage",
                "Monster", "Moon", "Mystic", "Nebula", "Neon", "Night", "Nimbus", "Ninja", "Nova", "Obsidian",
                "Ogre", "Onyx", "Oracle", "Orbit", "Outlaw", "Panther", "Phantom", "Phoenix", "Pilot", "Pixel",
                "Plasma", "Prism", "Prowler", "Pulse", "Pyro", "Quantum", "Quasar", "Quest", "Rage", "Raven",
                "Reaper", "Rebel", "Rhino", "Rider", "River", "Rocket", "Rogue", "Rune", "Saber", "Sapphire",
                "Savage", "Scarab", "Scarlet", "Scorch", "Scorpion", "Scout", "Shadow", "Shark", "Shield",
                "Shock", "Shrike", "Silver", "Skull", "Sky", "Sleet", "Smoke", "Snow", "Solar", "Sonic",
                "Sparrow", "Specter", "Spike", "Spirit", "Spire", "Splash", "Stalker", "Star", "Stealth",
                "Steel", "Stone", "Storm", "Strider", "Strike", "Stryker", "Sundown", "Sunset", "Surge",
                "Swift", "Talon", "Tempest", "Terror", "Thorn", "Thunder", "Tiger", "Titan", "Torch",
                "Tornado", "Trail", "Traveler", "Tremor", "Trooper", "Tundra", "Twilight", "Typhoon",
                "Ultra", "Valkyrie", "Vapor", "Venom", "Viper", "Void", "Vortex", "Vulcan", "Warrior",
                "Wasp", "Wave", "Whisper", "Wild", "Wisp", "Wolf", "Wrath", "Wyvern", "Zeal", "Zenith",
                "Zephyr", "Zombie", "Ace", "Atomic", "Aurora", "Ballistic", "Blitz", "Bolt", "Brawler",
                "Bullet", "Cannon", "Charge", "Clutch", "Colossus", "Crater", "Cross", "Dagger", "Delta",
                "Dire", "Drift", "Ember", "Falcon", "Flare", "Flicker", "Fury", "Gale", "Glint", "Havoc",
                "Hollow", "Horizon", "Jagged", "Knave", "Lunar", "Magnet", "Nebula", "Nexus", "Nova",
                "Orion", "Outlaw", "Phantom", "Pluto", "Raven", "Razor", "Ridge", "Rogue", "Saber",
                "Scythe", "Shade", "Skipper", "Sleet", "Sonic", "Specter", "Spike", "Stealth", "Tempest",
                "Trail", "Trigger", "Venom", "Volt", "Warden", "Wasp", "Widow", "Wrath", "Abyss", "Amulet",
                "Anchor", "Bandit", "Beacon", "Blizzard", "Bronze", "Cinder", "Cloud", "Cosmic", "Crimson",
                "Dragon", "Dune", "Echo", "Eclipse", "Elixir", "Fang", "Flint", "Frost", "Goblin", "Golden",
                "Griffin", "Inferno", "Ivy", "Kraken", "Laser", "Lightning", "Lion", "Maverick", "Meteor",
                "Monster", "Obsidian", "Ogre", "Onyx", "Panther", "Phoenix", "Pixel", "Plasma", "Prism",
                "Pyro", "Quantum", "Rider", "Rocket", "Rune", "Saber", "Savage", "Scarab", "Scarlet",
                "Scorpion", "Scout", "Shadow", "Shrike", "Silver", "Skull", "Smoke", "Solar", "Sparrow",
                "Specter", "Spike", "Spirit", "Star", "Stealth", "Stone", "Storm", "Stryker", "Swift",
                "Tempest", "Thunder", "Tiger", "Tornado", "Trooper", "Valkyrie", "Venom", "Viper",
                "Vortex", "Warrior", "Wolf", "Wrath", "Zombie", "Blaze", "Brimstone", "Bulldog", "Cobra",
                "Crater", "Cyclone", "Diablo", "Fury", "Havoc", "Jaguar", "Magma", "Nightmare", "Oblivion",
                "Predator", "Rampage", "Scourge", "Sentinel", "Shatter", "Titan", "Tyrant", "Vengeance",
                "Vindicator", "Wrath", "Xenon", "Zealot", "Zodiac", "Apex", "Blight", "Brimstone", "Cinder",
                "Colossus", "Echo", "Exile", "Fissure", "Flare", "Flicker", "Hollow", "Luminous", "Nox",
                "Rage", "Requiem", "Rift", "Spectral", "Talon", "Umbra", "Warden", "Zephyr"
            };
            var rng = new Random();
            return words[rng.Next(0, words.Count())];
        }

        public static string GetNewId()
        {
            return GenerateShortGuid(6);
        }

        private static string GenerateShortGuid(int length)
        {
            var rng = new Random();
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            return new string(Enumerable.Range(0, length)
                .Select(_ => characters[rng.Next(characters.Length)])
                .ToArray());
        }

        public static List<List<double>> Copy2dList(List<List<double>> list, bool reverse = false)
        {
            List<List<double>> newList = new List<List<double>>();

            if (reverse)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    newList.Add(new List<double>(list[i]));
                }
            }
            else
            {
                foreach (List<double> sublist in list)
                {
                    newList.Add(new List<double>(sublist));
                }
            }

            return newList;
        }

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            Random rng = new Random();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n+1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool IsPlayerWhite(Game game, bool isHost)
        {
            if (game.players.Count == 1)
            {
                return !game.players[0].isWhite;
            }
            if (game.settings.isStartingWhite)
            {
                return isHost;
            }
            var rng = new Random();
            var r = rng.Next(0, 2);
            if (r == 0)
            {
                return true;
            }
            return false;
        }
    }
}
