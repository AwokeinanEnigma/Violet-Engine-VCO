using Violet;

namespace VCO.Lua
{
    public class LUAManager
    {
        public static LUAManager instance;

        public LUAManager() {
            Debug.Log("Lua");
        }

        public static void Initialize()
        {
            // return if we exist and give a warning
            if (instance != null)
            {
                Debug.LWarning($"Another instance of the LUAManager class tried to be created!");
                return;
            }
            instance = new LUAManager();
        }
    }
}
