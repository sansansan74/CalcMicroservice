using CalcMicroservice.Services;

namespace CalcMicroservice.Utils
{
    public static class ModuleStartManager
    {
        readonly static ParamsStarterService _paramStarterService = new();

        public static void Init(string[] args)
        {
            _paramStarterService.Init(args);
        }

        public static bool ContainsModule(string module)
        {
            return _paramStarterService.ContainsModule(module);
        }
    }

}
