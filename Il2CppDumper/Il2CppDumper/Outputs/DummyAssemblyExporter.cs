using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Il2CppDumper
{
    public static class DummyAssemblyExporter
    {
        public static void Export(Il2CppExecutor il2CppExecutor, string outputDir, bool addToken)
        {
	        string dummyDllPath = Path.Combine(outputDir, "DummyDll");

            if (Directory.Exists(dummyDllPath))
                Directory.Delete(dummyDllPath, true);
            Directory.CreateDirectory(dummyDllPath);

            var dummy = new DummyAssemblyGenerator(il2CppExecutor, addToken);
            foreach (var assembly in dummy.Assemblies)
            {
	            string path = Path.Combine(dummyDllPath, assembly.MainModule.Name);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    assembly.Write(stream);
                    assembly.Dispose();
                }
            }
        }
    }
}
