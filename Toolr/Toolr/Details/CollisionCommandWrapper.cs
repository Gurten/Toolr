using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NSubstitute;
using TagTool.Cache;
using TagTool.Cache.Gen3;
using TagTool.Cache.HaloOnline;
using TagTool.Tags;
using TagTool.Tags.Definitions;


namespace Toolr.Details
{
    public static class CollisionCommandWrapper
    {
        public static void run(Uri input)
        {
            var gameCache = Substitute.For<TagTool.Cache.GameCache>();

            TagTool.Commands.CollisionModels.ImportCollisionGeometryCommand cmd = new TagTool.Commands.CollisionModels.ImportCollisionGeometryCommand(gameCache);

            var tagCache = Substitute.For<TagCache>();
            gameCache.OpenCacheReadWrite().Returns((Stream)null);
            tagCache.TagTable.Returns(new List<CachedTagHaloOnline>());
            var cachedTag = Substitute.For<CachedTag>();
            tagCache.AllocateTag(Arg.Any<TagGroup>(), Arg.Any<String>()).Returns(cachedTag);
            gameCache.TagCache.Returns(tagCache);
            tagCache.TagDefinitions = new TagDefinitionsGen3();

            var stringTable = Substitute.For<StringTable>();
            stringTable.GetStringId(Arg.Any<String>()).Returns(new TagTool.Common.StringId());
            gameCache.StringTable.Returns(stringTable);

            CollisionModel retrievedModel = null;
            gameCache.When(x => x.Serialize(Arg.Is((Stream)null), Arg.Any<CachedTag>(), Arg.Any<CollisionModel>())).Do(x => { retrievedModel = x.Arg<CollisionModel>(); });
            cmd.Execute(new System.Collections.Generic.List<string> { input.AbsolutePath, "" });

            Console.WriteLine("Successfully ran ImportCollisionGeometryCommand and obtained collision model" + retrievedModel.ToString());

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(retrievedModel, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            Console.WriteLine(jsonString);

        }
    }
}
