/// The Tag Collection Parser Prototype Project
/// Author: Gurten
using System;
using System.IO;
using Blamite.Blam;
using Blamite.Injection;
using Blamite.IO;
using Blamite.Util;
using Parsel.Cache.Core;
using Parsel.Cache.MccReach.Context;
using Parsel.Cache.Types.Phmo;
using Parsel.Schema.MccReach.Phmo;
using Parsel.Schema.Phmo;
using Parsel.TagSerialization.ContainerBuilder;

using SimpleJSON;
using NUnit.Framework;
using Parsel.TagSerialization;
using Parsel.Schema.MccHalo3.Phmo;
using Parsel.Schema.Coll;
using Parsel.Schema.MccReach.Coll;
using Parsel.AssetReaders.Geometry;

namespace Parsel
{
    class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine("program <halo3|reach> <path to haloce-coll> <output path>");
        }

        static void Main(string[] args)
        {
            ICollisionModel config = null;
            ICacheContext context = new MCCReachContext();
            //Console.WriteLine(args[0]);
            if (args.Length < 3)
            {
                PrintUsage();
                return;
            }

            switch (args[0])
            {
                case "halo3":
                    config = null; //
                    throw new NotImplementedException();
                    break;
                case "reach":
                    config = new MCCReachCollisionModel();
                    break;
                default:
                    PrintUsage();
                    return;
            }

            CollisionGeometryBuilder builder = new CollisionGeometryBuilder();
            string inputPath = args[1];
            if (builder.ParseFromFile(config, context, inputPath))
            {
                string outputPath = args[2];

                ;
                var blocks = builder.Build();

                string tagName = outputPath; //early guess. Gets corrected if there are path-separators.
                outputPath = outputPath.Replace("/", "\\");
                int lastPathSepIndex = outputPath.LastIndexOf("\\");
                if (lastPathSepIndex > 0)
                {
                    Directory.CreateDirectory(outputPath.Substring(0, lastPathSepIndex));
                    tagName = outputPath.Substring(lastPathSepIndex + 1);
                }

                tagName = tagName.Replace(".", "_");
                TagContainer container = new TagContainer();
                container.AddTag(new ExtractedTag(new DatumIndex(), 0, CharConstant.FromString("coll"), tagName));
                foreach (var b in blocks) { container.AddDataBlock(b); }

                using (var writer = new EndianWriter(File.Open(outputPath, FileMode.Create, FileAccess.Write), context.Endian))
                {
                    TagContainerWriter.WriteTagContainer(container, writer);
                }
            }

        }

    }
}
