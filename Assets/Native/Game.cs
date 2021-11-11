using Taichi.Asset;
using Taichi.Async;
using Taichi.Foundation;
using Taichi.ILRuntime;
using Taichi.UNode;
using UnityEngine;

namespace Light
{
    public class Game : MonoBehaviour, IScriptLoader
    {
        [Resolve] private static IAssetFactory factory = null;
        [Resolve] private static IScriptDomain script = null;

        // Start is called before the first frame update
        private void Start()
        {
            Assembler.ImportModuleInstance<Game, Game>(this);    
        }

        private void OnResolve()
        {
            NodeLoader.Load = file =>
            {
                using (var asset = factory.Load(file, typeof(TextAsset)))
                {
                    return asset.Cast<TextAsset>().bytes;
                }
            };

            script.Loader = this;
            script.LoadAssembly("IL/Script.il");
            script.Start("Light.Script", "Main");
        }

        public byte[] Load(string file)
        {
            using (var asset = factory.Load(file, typeof(TextAsset)))
            {
                return asset.Cast<TextAsset>().bytes;
            }
        }

        public IAsync<byte[]> LoadAsync(string file)
        {
            throw new System.NotImplementedException();
        }
    }
}

