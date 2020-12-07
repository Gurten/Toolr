/// The Tag Collection Parser Prototype Project
/// Author: Gurten
using System;
using Parsel.Cache.Core;

namespace Parsel.Cache.Types.Phmo
{
    public interface IPhysicsModelShapeTypes
    {
        //Add more as needed.
        ConfigConstant<UInt16> Polyhedron { get; }
        ConfigConstant<UInt16> List { get; }
    }

    public interface IPhysicsModelMotionTypes
    {
        ConfigConstant<UInt16> Keyframed { get; }
        ConfigConstant<UInt16> Fixed { get; }
    }
    public interface IPhysicsModelRigidBodyRuntimeFlags
    {
        //Add more as needed.
        ConfigConstant<UInt16> SuperImportantFlag { get; }
    }
}
