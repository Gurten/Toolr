/// The Tag Collection Parser Prototype Project
/// Author: Gurten
using System;
using Parsel.Schema.Core;

namespace Parsel.Schema.Coll
{
    public interface IMaterial : IStructSchema
    {
        DataField<Blamite.Blam.StringID> Name { get; }
    }

    public interface IRegion : IStructSchema
    {
        DataField<Blamite.Blam.StringID> Name { get; }

        ITagBlockRef<region.IPermutation> Permutations { get; }
    }

    namespace region
    {
        public interface IPermutation : IStructSchema
        {
            DataField<Blamite.Blam.StringID> Name { get; }
            ITagBlockRef<permutation.IBSP> BSPs { get; }
        }

        namespace permutation
        {
            public interface IBSP : IStructSchema
            {
                DataField<Int16> NodeIndex { get; }
				ITagBlockRef<bsp.IBSP3DNode> BSP3DNodes { get; }
				ITagBlockRef<bsp.IPlane> Planes { get; }
				ITagBlockRef<bsp.ILeafNode> Leaves { get; }
				ITagBlockRef<bsp.IBSP2DReference> BSP2DReferences { get; }
				ITagBlockRef<bsp.IBSP2DNode> BSP2DNodes { get; }
				ITagBlockRef<bsp.ISurface> Surfaces { get; }
				ITagBlockRef<bsp.IEdge> Edges { get; }
				ITagBlockRef<bsp.IVertex> Vertices { get; }
			}

            namespace bsp
            {
				public interface IBSP3DNode : IStructSchema
				{
					// These fields can be compressed. Often appear as 24-bit integers in the cache.
					VectorField<byte> BackChild { get; }
					VectorField<byte> FrontChild { get; }
					DataField<Int16> PlaneIndex { get; }
				}

				public interface IPlane : IStructSchema
				{
					VectorField<float> EquationIJKD { get; }
				}

				public interface ILeafNode : IStructSchema
				{
					DataField<Int16> Flags { get; }
					DataField<Int16> BSP2DReferenceCount { get; }
					DataField<Int16> Unknown { get; }
					DataField<Int16> FirstBSP2DReference { get; }
				}

				public interface IBSP2DReference : IStructSchema
				{
					// Something about the plane is used to disregard on axis 
					// of 3D BSP to move the traversal into 2D BSP. The rules
					// for projecting 3d->2d happen to be consistent.
					DataField<Int16> PlaneIndex { get; }
					DataField<Int16> BSP2DNodeIndex { get; }
				}

				public interface IBSP2DNode : IStructSchema
				{
					VectorField<float> Plane2DEquationIJD { get; }
					DataField<Int16> LeftChild { get; }
					DataField<Int16> RightChild { get; }
				}

				public interface ISurface : IStructSchema
				{
					DataField<Int16> PlaneIndex { get; }
					DataField<Int16> FirstEdgeIndex { get; }
					DataField<Int16> MaterialIndex { get; }
					DataField<Int16> BreakableSurface { get; }
					DataField<UInt16> Flags { get; }
				}

				public interface IEdge : IStructSchema
				{
					DataField<Int16> StartVertexIndex { get; }
					DataField<Int16> EndVertexIndex { get; }
					DataField<Int16> ForwardEdgeIndex { get; }
					DataField<Int16> ReverseEdgeIndex { get; }
					DataField<Int16> LeftSurfaceIndex { get; }
					DataField<Int16> RightSurfaceIndex { get; }
				}

				public interface IVertex : IStructSchema
				{
					VectorField<float> PointXYZ { get; }
					DataField<Int16> FirstEdgeIndex { get; }
				}
			}
        }
    }

	public interface IPathFindingSphere : IStructSchema
	{
		DataField<Int16> NodeIndex { get; }
		DataField<UInt16> Flags { get; }
		VectorField<float> SphereLocation { get; }
		DataField<float> SphereRadius { get;  }
	}

	public interface INode : IStructSchema 
	{
		DataField<Blamite.Blam.StringID> Name { get; }
		DataField<Int16> Unknown { get; }
		DataField<Int16> ParentNodeIndex { get; }
		DataField<Int16> NextSiblingNodeIndex { get; }
		DataField<Int16> FirstChildNodeIndex { get; }
	}


	public interface ICollisionModel : IStructSchema, ITagRoot
    {
        ITagBlockRef<IMaterial> Materials { get; }
        ITagBlockRef<IRegion> Regions { get; }
		ITagBlockRef<IPathFindingSphere> PathFindingSpheres { get; }
		ITagBlockRef<INode> Nodes { get; }
	}

}