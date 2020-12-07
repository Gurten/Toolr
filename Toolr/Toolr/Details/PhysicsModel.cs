using Blamite.Blam;
using Blamite.Injection;
using Blamite.IO;
using Blamite.Util;
using Parsel.Cache.Core;
using Parsel.Cache.Types.Phmo;
using Parsel.Schema.Phmo;
using Parsel.TagSerialization.ContainerBuilder;
using System;
using System.IO;
using TagTool.Geometry;

namespace Toolr.Details
{
    public static class PhysicsModel
    {

        public static void Generate(IPhysicsModel config, ICacheContext context,
            Uri inputUri, Uri outputUri)
        {
            //TODO
            if (!inputUri.IsFile)
            {
                throw new ArgumentException("Only currently supports files as inputs.");
            }

            if (!outputUri.IsFile)
            {
                throw new ArgumentException("Only currently supports files as outputs.");
            }

#if False
            // Reuse tag-tool logic.
            PhysicsModelBuilder builder = new PhysicsModelBuilder();

            if (!builder.ParseFromFile(inputUri.AbsolutePath))
            {
                throw new InvalidOperationException("subprogram could not parse phmo");
            }

            var phmo = builder.Build();
            var blocks = Convert(config, context, phmo);
# else
            // Use port of older Halo Online Tag Tool version.
            var blocks = Parsel.AssetReaders.Geometry.BlenderPhmoReader.Build(config, context, inputUri.AbsolutePath);

#endif
            TagContainer container = new TagContainer();
            container.AddTag(new ExtractedTag(new DatumIndex(), 0, CharConstant.FromString("phmo"), "unnamed"));
            foreach (var b in blocks) { container.AddDataBlock(b); }

            using (var writer = new EndianWriter(File.Open(outputUri.AbsolutePath, FileMode.Create, FileAccess.Write), context.Endian))
            {
                TagContainerWriter.WriteTagContainer(container, writer);
            }
        }

        private static DataBlock[] Convert(IPhysicsModel config, ICacheContext context, 
            TagTool.Tags.Definitions.PhysicsModel phmo)
        {
            var sc = ContainerBuilder.CreateSerializationContext(config, context);
            var materialSc = sc.GetSerializationContext((phmo2) => phmo2.MaterialsTagBlock);
            if(phmo.Materials != null)
            foreach (var material in phmo.Materials)
            {
                materialSc.Add().Serialize((writer, a) => {
                    a.Name.Visit(writer, new StringID(material.Name.Value)); // This ought to not be the same
                    a.PhantomTypeIndex.Visit(writer, material.PhantomType);
                });
            }

            var nodeSc = sc.GetSerializationContext((phmo2) => phmo2.NodesTagBlock);
            if(phmo.Nodes != null)
            foreach (var node in phmo.Nodes)
            {
                nodeSc.Add().Serialize((writer, a) => {
                    a.Name.Visit(writer, new StringID(node.Name.Value)); // This ought to not be the same
                    a.ChildIndex.Visit(writer, node.Child);
                    a.ParentIndex.Visit(writer, node.Parent);
                    a.SiblingIndex.Visit(writer, node.Sibling);
                });
            }


            var rigidBodySc = sc.GetSerializationContext((phmo2) => phmo2.RigidBodyTagBlock);
            if(phmo.RigidBodies != null)
            foreach (var rigidBody in phmo.RigidBodies)
            {
                rigidBodySc.Add().Serialize((writer, a) =>
                {
                    a.BoundingSphereRadius.Visit(writer, rigidBody.BoundingSphereRadius);
                    a.MotionType.Visit(writer, new ConfigConstant<short>((short)rigidBody.MotionType));
                    a.Mass.Visit(writer, rigidBody.Mass);
                    a.ShapeIndex.Visit(writer, (ushort)rigidBody.ShapeIndex);

                    var runtimeFlags = context.Get<IPhysicsModelRigidBodyRuntimeFlags>();
                    // Makes things collidable!
                    a.RuntimeFlags.Visit(writer, runtimeFlags.SuperImportantFlag);

                    var shapeTypeOptions = context.Get<IPhysicsModelShapeTypes>();
                    a.ShapeType.Visit(writer, (ushort)rigidBody.ShapeType);

                });
            }

            var listShapesSc = sc.GetSerializationContext((phmo2) => phmo2.ListsShapesTagBlock);
            if(phmo.ListShapes != null)
            foreach (var listShape in phmo.ListShapes)
            {
                listShapesSc.Add().Serialize((writer, a) => {
                    a.ShapeIndex.Visit(writer, (ushort)listShape.ShapeIndex);
                    a.ChildShapeCount.Visit(writer, (ushort)listShape.NumChildShapes);
                    a.ShapeType.Visit(writer, (ushort)listShape.ShapeType);
                });
            }

            var listSc = sc.GetSerializationContext((phmo2) => phmo2.ListsTagBlock);
            if(phmo.Lists != null)
            foreach (var list in phmo.Lists)
            {
                listSc.Add().Serialize((writer, a) => {
                    a.ChildShapes.Visit(writer, (UInt32)list.ChildShapesSize); // put the actual value here.
                });
            }

            var polyhedraSc = sc.GetSerializationContext((phmo2) => phmo2.PolyhedraTagBlock);
            if(phmo.Polyhedra != null)
            foreach (var polyhedra in phmo.Polyhedra)
            {
                polyhedraSc.Add().Serialize((writer, a) => {
                    a.Name.Visit(writer, new StringID(polyhedra.Name.Value)); // This ought to not be the same
                    a.FourVectors.Visit(writer, (UInt32)polyhedra.FourVectorsSize);
                    a.PlaneEquations.Visit(writer, (UInt32)polyhedra.PlaneEquationsSize);
                    a.AABBHalfExtents.Visit(writer, new float[] { 
                        polyhedra.AabbHalfExtents.I, polyhedra.AabbHalfExtents.J,
                        polyhedra.AabbHalfExtents.K, polyhedra.AabbHalfExtentsRadius });
                    a.AABBCenter.Visit(writer, new float[] { 
                        polyhedra.AabbCenter.I, polyhedra.AabbCenter.J, 
                        polyhedra.AabbCenter.K, polyhedra.AabbCenterRadius});

                    a.PhantomTypeIndex.Visit(writer, (byte)polyhedra.PhantomIndex);
                });
            }

            var planesSc = sc.GetSerializationContext((phmo2) => phmo2.PolyhedraPlaneEquationsTagBlock);
            if(phmo.PolyhedronPlaneEquations != null)
            foreach (var plane in phmo.PolyhedronPlaneEquations)
            {
                planesSc.Add().Serialize((writer, a) => {
                    a.PlaneEquation.Visit(writer, new float[] {
                        plane.PlaneEquation.I,
                        plane.PlaneEquation.J,
                        plane.PlaneEquation.K,
                        plane.PlaneEquation.D,
                    });
                });
            }

            var fourVectorSc = sc.GetSerializationContext((phmo2) => phmo2.PolyhedraFourVectorTagBlock);
            if(phmo.PolyhedronFourVectors != null)
            foreach (var fourVector in phmo.PolyhedronFourVectors)
            {
                fourVectorSc.Add().Serialize((writer, a) =>
                {
                    a.Xs.Visit(writer, new float[] { 
                        fourVector.FourVectorsX.I,
                        fourVector.FourVectorsX.J,
                        fourVector.FourVectorsX.K,
                        fourVector.FourVectorsXRadius,
                    });
                    a.Ys.Visit(writer, new float[] {
                        fourVector.FourVectorsY.I,
                        fourVector.FourVectorsY.J,
                        fourVector.FourVectorsY.K,
                        fourVector.FourVectorsYRadius,
                    });
                    a.Zs.Visit(writer, new float[] {
                        fourVector.FourVectorsZ.I,
                        fourVector.FourVectorsZ.J,
                        fourVector.FourVectorsZ.K,
                        fourVector.FourVectorsZRadius,
                    });
                });
            }

            return sc.Finish();
        }

    }
}
