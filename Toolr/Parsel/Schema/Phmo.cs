/// The Tag Collection Parser Prototype Project
/// Author: Gurten
using System;
using Parsel.Schema.Core;

namespace Parsel.Schema.Phmo
{
    public interface IPhysicsModelMaterial : IStructSchema
    {
        DataField<Blamite.Blam.StringID> Name { get; }
        DataField<Int16> PhantomTypeIndex { get; }
    }

    public interface IPhysicsModelPolyhedra : IStructSchema
    {
        DataField<Blamite.Blam.StringID> Name { get; }
        DataField<byte> PhantomTypeIndex { get; }
        DataField<byte> CollisionGroup { get; }
        DataField<UInt16> Count { get; }
        /// This is a different size on different engines.
        VectorField<byte> InstanceOffset { get; }
        DataField<float> Radius { get; }
        VectorField<float> AABBHalfExtents { get; }
        VectorField<float> AABBCenter { get; }
        ISizeAndCapacityField FourVectors { get; }
        DataField<Int32> VertexCount { get; }
        ISizeAndCapacityField PlaneEquations { get; }
    }

    public interface IPhysicsModelLists : IStructSchema
    {
        /// Some odd, poorly-named field. Happens to be '128'. 
        DataField<uint> Count { get; }
        ISizeAndCapacityField ChildShapes { get; }
        VectorField<float> AABBHalfExtents { get; }
        VectorField<float> AABBCenter { get; }
    }

    public interface IPhysicsModelListShapes : IStructSchema
    {
        DataField<UInt16> ShapeType { get; }
        DataField<UInt16> ShapeIndex { get; }
        DataField<UInt16> ChildShapeCount { get; }
    }

    public interface IPhysicsModelFourVectors : IStructSchema
    {
        VectorField<float> Xs { get; }
        VectorField<float> Ys { get; }
        VectorField<float> Zs { get; }
    }

    public interface IPhysicsModelPlaneEquations : IStructSchema
    {
        VectorField<float> PlaneEquation { get; }
    }

    public interface IPhysicsModelNode : IStructSchema
    {
        DataField<Blamite.Blam.StringID> Name { get; }
        DataField<UInt16> Flags { get; }
        DataField<Int16> ParentIndex { get; }
        DataField<Int16> SiblingIndex { get; }
        DataField<Int16> ChildIndex { get; }
    }

    public interface IPhysicsModelRigidBody : IStructSchema
    {
        DataField<float> BoundingSphereRadius { get; }
        VectorField<byte> MotionType { get; }
        DataField<UInt16> ShapeType { get; }
        DataField<float> Mass { get; }
        DataField<UInt16> RuntimeFlags { get; }
        DataField<UInt16> ShapeIndex { get; }
    }

    public interface IPhysicsModel : IStructSchema, ITagRoot
    {
        ITagBlockRef<IPhysicsModelRigidBody> RigidBodyTagBlock { get; }
        ITagBlockRef<IPhysicsModelMaterial> MaterialsTagBlock { get; }
        ITagBlockRef<IPhysicsModelPolyhedra> PolyhedraTagBlock { get; }
        ITagBlockRef<IPhysicsModelFourVectors> PolyhedraFourVectorTagBlock { get; }
        ITagBlockRef<IPhysicsModelPlaneEquations> PolyhedraPlaneEquationsTagBlock { get; }
        ITagBlockRef<IPhysicsModelLists> ListsTagBlock { get; }
        ITagBlockRef<IPhysicsModelListShapes> ListsShapesTagBlock { get; }
        //ITagBlockRef<IPhysicsModelListShapes> RegionsTagBlock { get; }
        ITagBlockRef<IPhysicsModelNode> NodesTagBlock { get; }

    }
}
