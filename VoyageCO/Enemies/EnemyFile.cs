using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using Violet;
using Violet.Utility;

namespace VCO.Data.Enemies
{
    internal class EnemyFile
    {

        public static EnemyFile Instance
        {
            get
            {
                EnemyFile.Load();
                return EnemyFile.INSTANCE;
            }
        }

        public static void Load()
        {
            if (EnemyFile.INSTANCE == null)
            {
                EnemyFile.INSTANCE = new EnemyFile();
            }
        }

        private EnemyFile()
        {
            this.enemyDataDict = new Dictionary<int, EnemyData>();
            foreach (string fileInfo in Directory.GetFiles(Path.Combine("Data" + Path.DirectorySeparatorChar, "Content", "") + Path.DirectorySeparatorChar + "Enemies" + Path.DirectorySeparatorChar))
            {
                if (fileInfo.Contains(".edat"))
                {

                    Console.WriteLine($"Loading .edat file {fileInfo}");
                    this.Load(fileInfo);

                }
                else
                {
                    throw new Exception($"File {fileInfo} is not of the format .edat, remove it from the enemies folder!");
                }
            }
            //
        }
        private void Load(string path)
        {
            NbtFile nbtFile = new NbtFile(path);
            EnemyData enemyData = new EnemyData(nbtFile.RootTag);
            int key = Hash.Get(enemyData.QualifiedName);
            Debug.LogInfo($"Path '{path}', qualified name is {enemyData.QualifiedName}");
            this.enemyDataDict.Add(key, enemyData);
        }

        public EnemyData GetEnemyData(string name)
        {
            int hash = Hash.Get(name);
            if (enemyDataDict.TryGetValue(hash, out EnemyData attemptData))
            {
                //Console.WriteLine($"Properly returned enemy: {name}");
                return attemptData;
            }
            Debug.LogError($"Was unable to return enemy: {name}", false);
            return attemptData;
        }

        public List<EnemyData> GetAllEnemyData()
        {
            return new List<EnemyData>(this.enemyDataDict.Values);
        }
        private static EnemyFile INSTANCE;
        private readonly Dictionary<int, EnemyData> enemyDataDict;
    }
}
