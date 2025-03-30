using LogicAPI.Client;
using LogicLog;

namespace LWGlass
{
    public class LWGClient : ClientMod
    {

        static LWGClient() {}

        protected override void Initialize()
        {
            Logger.Info("LWGClient - Loaded Client");
        }
    }
}