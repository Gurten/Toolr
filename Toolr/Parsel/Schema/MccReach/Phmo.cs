/// The Tag Collection Parser Prototype Project
/// Author: Gurten
using Blamite.Blam;
using Blamite.IO;
using System;
using Parsel.Schema.Core;
using Parsel.Schema.Phmo;
using NewGenSizeAndCapacityField = Parsel.Schema.Core.NewGenSizeAndCapacityField;

namespace Parsel.Schema.MccReach.Phmo
{
    public class MCCReachPhysicsModelMaterial : IPhysicsModelMaterial
    {
        UInt32 IStructSchema.Size => 0x10;
        UInt32 IStructSchema.Alignment => 4;

        DataField<StringID> IPhysicsModelMaterial.Name => new DataField<StringID>(0);
        DataField<Int16> IPhysicsModelMaterial.PhantomTypeIndex => new DataField<Int16>(0xc);
    }

    public class MCCReachPhysicsModelPolyhedra : IPhysicsModelPolyhedra, IStructWithDataFixup
    {
        public UInt32 Size => 0xb0;
        UInt32 IStructSchema.Alignment => 0x10;

        DataField<Blamite.Blam.StringID> IPhysicsModelPolyhedra.Name
            => new DataField<Blamite.Blam.StringID>(0);
        DataField<byte> IPhysicsModelPolyhedra.PhantomTypeIndex => new DataField<byte>(0x1E);
        DataField<byte> IPhysicsModelPolyhedra.CollisionGroup => new DataField<byte>(0x1F);
        DataField<UInt16> IPhysicsModelPolyhedra.Count => new DataField<UInt16>(0x2A);
        /// <summary>
        /// This is a uint64 (8-bytes) in Halo Reach.
        /// </summary>
        public VectorField<byte> InstanceOffset => new VectorField<byte>(0x30, 8);
        DataField<float> IPhysicsModelPolyhedra.Radius => new DataField<float>(0x40);
        VectorField<float> IPhysicsModelPolyhedra.AABBHalfExtents => new VectorField<float>(0x50, 4);
        VectorField<float> IPhysicsModelPolyhedra.AABBCenter => new VectorField<float>(0x60, 4);
        ISizeAndCapacityField IPhysicsModelPolyhedra.FourVectors
            => new NewGenSizeAndCapacityField(0x78);
        DataField<Int32> IPhysicsModelPolyhedra.VertexCount => new DataField<Int32>(0x80);
        ISizeAndCapacityField IPhysicsModelPolyhedra.PlaneEquations
            => new NewGenSizeAndCapacityField(0x98);

        public void VisitInstance(IWriter writer, uint index)
        {
            writer.SeekTo(this.InstanceOffset[0].Offset);
            writer.WriteUInt64(32 + this.Size * index);
        }
    }

    public class MCCReachPhysicsModelLists : IPhysicsModelLists, IStructWithDataFixup
    {
        UInt32 IStructSchema.Size => 0x90;
        UInt32 IStructSchema.Alignment => 0x10;

        public DataField<uint> Count => new DataField<uint>(0xA);
        ISizeAndCapacityField IPhysicsModelLists.ChildShapes => new NewGenSizeAndCapacityField(0x38);
        VectorField<float> IPhysicsModelLists.AABBHalfExtents => new VectorField<float>(0x50, 4);
        VectorField<float> IPhysicsModelLists.AABBCenter => new VectorField<float>(0x60, 4);

        void IStructWithDataFixup.VisitInstance(IWriter writer, uint index)
        {
            Count.Visit(writer, 128);
        }
    }

    public class MCCReachPhysicsModelListShapes : IPhysicsModelListShapes
    {
        UInt32 IStructSchema.Size => 0x20;
        UInt32 IStructSchema.Alignment => 0x4;

        DataField<ushort> IPhysicsModelListShapes.ShapeType => new DataField<UInt16>(0);
        DataField<ushort> IPhysicsModelListShapes.ShapeIndex => new DataField<UInt16>(2);
        DataField<ushort> IPhysicsModelListShapes.ChildShapeCount => new DataField<UInt16>(0x10);
    }

    public class PhysicsModelFourVectors : IPhysicsModelFourVectors
    {
        UInt32 IStructSchema.Size => 0x30;
        UInt32 IStructSchema.Alignment => 0x10;
        VectorField<float> IPhysicsModelFourVectors.Xs => new VectorField<float>(0, 4);
        VectorField<float> IPhysicsModelFourVectors.Ys => new VectorField<float>(0x10, 4);
        VectorField<float> IPhysicsModelFourVectors.Zs => new VectorField<float>(0x20, 4);
    }

    public class PhysicsModelPlaneEquations : IPhysicsModelPlaneEquations
    {
        UInt32 IStructSchema.Size => 0x10;
        UInt32 IStructSchema.Alignment => 0x10;
        VectorField<float> IPhysicsModelPlaneEquations.PlaneEquation => new VectorField<float>(0, 4);
    }
    public class PhysicsModelNode : IPhysicsModelNode
    {
        UInt32 IStructSchema.Size => 0xC;
        UInt32 IStructSchema.Alignment => 4;

        DataField<StringID> IPhysicsModelNode.Name => new DataField<StringID>(0);
        DataField<UInt16> IPhysicsModelNode.Flags => new DataField<UInt16>(4);
        DataField<Int16> IPhysicsModelNode.ParentIndex => new DataField<Int16>(6);
        DataField<Int16> IPhysicsModelNode.SiblingIndex => new DataField<Int16>(8);
        DataField<Int16> IPhysicsModelNode.ChildIndex => new DataField<Int16>(10);
    }
    public class MCCReachPhysicsModelRigidBody : IPhysicsModelRigidBody
    {
        UInt32 IStructSchema.Size => 208;
        UInt32 IStructSchema.Alignment => 4;

        DataField<float> IPhysicsModelRigidBody.BoundingSphereRadius => new DataField<float>(20);
        VectorField<byte> IPhysicsModelRigidBody.MotionType => new VectorField<byte>(0x1c, 1);
        DataField<UInt16> IPhysicsModelRigidBody.ShapeType => new DataField<UInt16>(168);
        DataField<float> IPhysicsModelRigidBody.Mass => new DataField<float>(0xB0);
        DataField<UInt16> IPhysicsModelRigidBody.RuntimeFlags => new DataField<ushort>(0xBA);
        DataField<UInt16> IPhysicsModelRigidBody.ShapeIndex => new DataField<UInt16>(0xAA);
    }

    public class MCCReachPhysicsModel : IPhysicsModel
    {
        UInt32 IStructSchema.Size => 412;
        UInt32 IStructSchema.Alignment => 4;

        ITagBlockRef<IPhysicsModelRigidBody> IPhysicsModel.RigidBodyTagBlock
            => new NewGenTagBlockRef<IPhysicsModelRigidBody>(92, new MCCReachPhysicsModelRigidBody());
        ITagBlockRef<IPhysicsModelMaterial> IPhysicsModel.MaterialsTagBlock
            => new NewGenTagBlockRef<IPhysicsModelMaterial>(0x68, new MCCReachPhysicsModelMaterial());
        ITagBlockRef<IPhysicsModelPolyhedra> IPhysicsModel.PolyhedraTagBlock
            => new NewGenTagBlockRef<IPhysicsModelPolyhedra>(0xb0, new MCCReachPhysicsModelPolyhedra());
        ITagBlockRef<IPhysicsModelFourVectors> IPhysicsModel.PolyhedraFourVectorTagBlock
            => new NewGenTagBlockRef<IPhysicsModelFourVectors>(0xbc, new PhysicsModelFourVectors());
        ITagBlockRef<IPhysicsModelPlaneEquations> IPhysicsModel.PolyhedraPlaneEquationsTagBlock
            => new NewGenTagBlockRef<IPhysicsModelPlaneEquations>(0xc8, new PhysicsModelPlaneEquations());
        ITagBlockRef<IPhysicsModelLists> IPhysicsModel.ListsTagBlock
            => new NewGenTagBlockRef<IPhysicsModelLists>(0xe0, new MCCReachPhysicsModelLists());
        ITagBlockRef<IPhysicsModelListShapes> IPhysicsModel.ListsShapesTagBlock
            => new NewGenTagBlockRef<IPhysicsModelListShapes>(0xec, new MCCReachPhysicsModelListShapes());
        //ITagBlockRef<IPhysicsModelListShapes> IPhysicsModel.RegionsTagBlock => throw new NotImplementedException();
        ITagBlockRef<IPhysicsModelNode> IPhysicsModel.NodesTagBlock
            => new NewGenTagBlockRef<IPhysicsModelNode>(0x13c, new PhysicsModelNode());
    }
}
