using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Il2CppDumper
{
    public static class Il2CppDumper
    {
        public static bool PerformDump(string gameAssemblyPath, string metadataDatPath, string outputDirectoryPath,
	        Config config, Action<string> reportProgressAction)
        {
	        Init(gameAssemblyPath, metadataDatPath, config, reportProgressAction, out var metadata, out var il2Cpp);

	        reportProgressAction("Dumping...");
	        var executor = new Il2CppExecutor(metadata, il2Cpp);
	        var decompiler = new Il2CppDecompiler(executor);
	        reportProgressAction("Done!");
	        if (config.GenerateStruct)
	        {
		        decompiler.Decompile(config, outputDirectoryPath, reportProgressAction);
                reportProgressAction("Generate struct...");
		        var scriptGenerator = new StructGenerator(executor);
		        scriptGenerator.WriteScript(outputDirectoryPath);
		        reportProgressAction("Done!");
	        }
	        if (config.GenerateDummyDll)
	        {
		        reportProgressAction("Generate dummy dll...");
		        DummyAssemblyExporter.Export(executor, outputDirectoryPath, config.DummyDllAddToken);
		        reportProgressAction("Done!");
	        }

	        return true;
        }

        public static void Init(string il2cppPath,
                                 string metadataPath,
                                 Config config,
                                 Action<string> reportProgressAction,
                                 out Metadata metadata,
                                 out Il2Cpp il2Cpp)
        {
            reportProgressAction("Initializing metadata...");
            var metadataBytes = File.ReadAllBytes(metadataPath);
            metadata = new Metadata(new MemoryStream(metadataBytes));
            reportProgressAction($"Metadata Version: {metadata.Version}");

            reportProgressAction("Initializing il2cpp file...");
            var il2cppBytes = File.ReadAllBytes(il2cppPath);
            var il2cppMagic = BitConverter.ToUInt32(il2cppBytes, 0);
            var il2CppMemory = new MemoryStream(il2cppBytes);
            switch (il2cppMagic)
            {
                default:
                    throw new NotSupportedException("ERROR: il2cpp file not supported.");
                case 0x6D736100:
                    var web = new WebAssembly(il2CppMemory, reportProgressAction);
                    il2Cpp = web.CreateMemory();
                    break;
                case 0x304F534E:
                    var nso = new NSO(il2CppMemory, reportProgressAction);
                    il2Cpp = nso.UnCompress();
                    break;
                case 0x905A4D: //PE
                    il2Cpp = new PE(il2CppMemory, reportProgressAction);
                    break;
                case 0x464c457f:             //ELF
                    if (il2cppBytes[4] == 2) //ELF64
                    {
                        il2Cpp = new Elf64(il2CppMemory, reportProgressAction);
                    }
                    else
                    {
                        il2Cpp = new Elf(il2CppMemory, reportProgressAction);
                    }

                    break;
                case 0xCAFEBABE: //FAT Mach-O
                case 0xBEBAFECA:
                    throw new InvalidOperationException("FAT Mach-O format is currently not supported.");
                    // var machofat = new MachoFat(new MemoryStream(il2cppBytes));
                    // reportProgressAction("Select Platform: ");
                    // for (var i = 0; i < machofat.fats.Length; i++)
                    // {
                    //     var fat = machofat.fats[i];
                    //     Console.Write(fat.magic == 0xFEEDFACF ? $"{i + 1}.64bit " : $"{i + 1}.32bit ");
                    // }
                    //
                    // Console.WriteLine();
                    // var key = Console.ReadKey(true);
                    // var index = int.Parse(key.KeyChar.ToString()) - 1;
                    // var magic = machofat.fats[index % 2].magic;
                    // il2cppBytes = machofat.GetMacho(index % 2);
                    // il2CppMemory = new MemoryStream(il2cppBytes);
                    // if (magic == 0xFEEDFACF)
                    //     goto case 0xFEEDFACF;
                    // else
                    //     goto case 0xFEEDFACE;
                case 0xFEEDFACF: // 64bit Mach-O
                    il2Cpp = new Macho64(il2CppMemory, reportProgressAction);
                    break;
                case 0xFEEDFACE: // 32bit Mach-O
                    il2Cpp = new Macho(il2CppMemory, reportProgressAction);
                    break;
            }

            var version = config.ForceIl2CppVersion ? config.ForceVersion : metadata.Version;
            il2Cpp.SetProperties(version, metadata.maxMetadataUsages);
            reportProgressAction($"Il2Cpp Version: {il2Cpp.Version}");
            if (il2Cpp.Version >= 27 && il2Cpp is ElfBase elf && elf.IsDumped)
            {
                reportProgressAction("Input global-metadata.dat dump address:");
                metadata.Address = Convert.ToUInt64(Console.ReadLine(), 16);
            }


            reportProgressAction("Searching...");
            try
            {
                var flag = il2Cpp.PlusSearch(metadata.methodDefs.Count(x => x.methodIndex >= 0),
                    metadata.typeDefs.Length, metadata.imageDefs.Length);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (!flag && il2Cpp is PE)
                    {
                        reportProgressAction("Use custom PE loader");
                        il2Cpp = PELoader.Load(il2cppPath, reportProgressAction);
                        il2Cpp.SetProperties(version, metadata.maxMetadataUsages);
                        flag = il2Cpp.PlusSearch(metadata.methodDefs.Count(x => x.methodIndex >= 0),
                            metadata.typeDefs.Length, metadata.imageDefs.Length);
                    }
                }

                if (!flag)
                {
                    flag = il2Cpp.Search();
                }

                if (!flag)
                {
                    flag = il2Cpp.SymbolSearch();
                }

                if (!flag)
                {
                    reportProgressAction("ERROR: Unable to automatically determine CodeRegistration and MetadataRegistration properties");
                    // Console.Write("Input CodeRegistration: ");
                    // var codeRegistration = Convert.ToUInt64(Console.ReadLine(), 16);
                    // Console.Write("Input MetadataRegistration: ");
                    // var metadataRegistration = Convert.ToUInt64(Console.ReadLine(), 16);
                    // il2Cpp.Init(codeRegistration, metadataRegistration);
                }
            }
            catch (Exception e)
            {
                reportProgressAction($"ERROR: An error occurred while processing: {e}");
            }
        }
    }
}