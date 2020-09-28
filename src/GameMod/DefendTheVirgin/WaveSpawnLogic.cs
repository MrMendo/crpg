﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Crpg.GameMod.DefendTheVirgin
{
    internal class WaveSpawnLogic : MissionLogic
    {
        private readonly WaveController _waveController;
        private readonly WaveGroup[][] _waves;
        private readonly BasicCharacterObject _mainCharacter;

        public WaveSpawnLogic(WaveController waveController, WaveGroup[][] waves, BasicCharacterObject mainCharacter)
        {
            _waveController = waveController;
            _waves = waves;
            _mainCharacter = mainCharacter;
            _waveController.OnWaveStarted += SpawnAgents;
        }

        protected override void OnEndMission()
        {
            _waveController.OnWaveStarted -= SpawnAgents;
        }

        private void SpawnAgents(int waveNb)
        {
            // TODO: add virgin character

            // Set player character. Without this line we would just spectate bots fighting without being able to play
            Game.Current.PlayerTroop = _mainCharacter;

            Mission.SpawnTroop(new BasicBattleAgentOrigin(_mainCharacter), true, false, !_mainCharacter.Equipment.Horse.IsEmpty, false, true, 0, 0, false, true);

            WaveGroup[] wave = _waves[waveNb - 1];
            foreach (var group in wave)
            {
                var troop = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(group.Id);
                var agentOrigin = new BasicBattleAgentOrigin(troop);
                Formation formation = Mission.GetAgentTeam(agentOrigin, false).GetFormation(FormationClass.Infantry);
                formation.BeginSpawn(group.Count, false);
                Mission.Current.SpawnFormation(formation, group.Count, true, false, false);
                for (int i = 0; i < group.Count; i += 1)
                {
                    Mission.Current.SpawnTroop(agentOrigin, false, true, !troop.Equipment.Horse.IsEmpty, false, true, group.Count, i, true, true);
                }

                formation.EndSpawn();
            }
        }
    }
}
