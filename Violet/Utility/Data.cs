using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Violet.Utility
{
    /// <summary>
    /// Handles the loading of things in the data folder.
    /// </summary>
    public class Data
    {
        public static Data instance;

        public static void Initialize() {
            if (instance != null) {
                Debug.LogWarning($"Another instance of the Data class tried to be initialized!");
                return;
            }

            instance = new Data();
        }

        public void Load(string file) {
        
        }
    }
}
