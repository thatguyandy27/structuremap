using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Interceptors;

namespace StructureMap.Graph
{
    /// <summary>
    /// Manages a list of InstanceFactoryInterceptor's.  Design-time model of an array
    /// of decorators to alter the InstanceFactory behavior for a PluginType.
    /// </summary>
    [Obsolete] public class InterceptionChain : IEnumerable<InstanceFactoryInterceptor>, IEquatable<InterceptionChain>
    {
        private List<InstanceFactoryInterceptor> _interceptorList;

        public InterceptionChain()
        {
            _interceptorList = new List<InstanceFactoryInterceptor>();
        }

        public int Count
        {
            get { return _interceptorList.Count; }
        }

        public InstanceFactoryInterceptor this[int index]
        {
            get { return _interceptorList[index]; }
        }

        #region IEnumerable<InstanceFactoryInterceptor> Members

        IEnumerator<InstanceFactoryInterceptor> IEnumerable<InstanceFactoryInterceptor>.GetEnumerator()
        {
            return _interceptorList.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _interceptorList.GetEnumerator();
        }

        #endregion

        public IInstanceFactory WrapInstanceFactory(IInstanceFactory factory)
        {
            IInstanceFactory outerFactory = factory;

            for (int i = _interceptorList.Count - 1; i >= 0; i--)
            {
                InstanceFactoryInterceptor interceptor = _interceptorList[i];
                interceptor.InnerInstanceFactory = outerFactory;
                outerFactory = interceptor;
            }

            return outerFactory;
        }

        public void AddInterceptor(InstanceFactoryInterceptor interceptor)
        {
            _interceptorList.Add(interceptor);
        }

        public bool Contains(Type interceptorType)
        {
            foreach (InstanceFactoryInterceptor interceptor in _interceptorList)
            {
                if (interceptor.GetType() == interceptorType)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Equals(InterceptionChain interceptionChain)
        {
            if (interceptionChain == null) return false;

            
            if (!Equals(_interceptorList.Count, interceptionChain._interceptorList.Count)) return false;

            for (int i = 0; i < _interceptorList.Count; i++)
            {
                if (!Equals(_interceptorList[i], interceptionChain._interceptorList[i])) return false;
                
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as InterceptionChain);
        }

        public override int GetHashCode()
        {
            return _interceptorList != null ? _interceptorList.GetHashCode() : 0;
        }
    }
}