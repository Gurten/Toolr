﻿/// The Tag Collection Parser Prototype Project
/// Author: Gurten
using System;
using System.Collections.Generic;
using Parsel.Cache.Core;
using Blamite.IO;
using Parsel.Cache.Types.Phmo;
using Parsel.Cache.MccReach.Phmo;

namespace Parsel.Cache.MccReach.Context
{
    public class MCCReachContext : ICacheContext
    {
        private Dictionary<Type, object> _handlers = new Dictionary<Type, object>()
        {
            { typeof(IPhysicsModelShapeTypes), new PhysicsModelShapeTypes() },
            { typeof(IPhysicsModelMotionTypes), new PhysicsModelMotionTypes() },
            { typeof(IPhysicsModelRigidBodyRuntimeFlags), new PhysicsModelRigidBodyRuntimeFlags() },

            
        };

        public Blamite.IO.Endian Endian => Endian.LittleEndian;

        public T Get<T>()
        {
            object obj = 0;
            if (_handlers.TryGetValue(typeof(T), out obj))
            {
                return (T)obj;
            }

            throw new NotImplementedException();
        }

        
    }
}
