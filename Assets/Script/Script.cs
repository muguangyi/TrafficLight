using Taichi.Foundation;
using Taichi.Logger;

namespace Light
{
    public sealed class Script
    {
        public static void Main()
        {
            Log.Info("Hello Light!");

            Assembler.ImportModuleInstance<ICity, City>(new City());
        }
    }
}

