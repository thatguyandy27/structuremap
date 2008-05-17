using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Use to express the instance of a PluginType for the containing Profile
    /// </summary>
    public class InstanceDefaultExpression
    {
        private readonly ProfileExpression _parent;
        private readonly Type _pluginType;
        private Instance _instance;
        private string _instanceKey = string.Empty;

        public InstanceDefaultExpression(Type pluginType, ProfileExpression parent)
        {
            _pluginType = pluginType;
            _parent = parent;
        }

        /// <summary>
        /// Use a named, preconfigured instance as the default instance for this profile 
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public ProfileExpression UseNamedInstance(string instanceKey)
        {
            _instanceKey = instanceKey;
            return _parent;
        }

        internal void Configure(string profileName, PluginGraph pluginGraph)
        {
            // The profile instance is defined inline
            if (_instance != null)
            {
                _instanceKey = Profile.InstanceKeyForProfile(profileName);
                _instance.Name = _instanceKey;
                pluginGraph.FindFamily(_pluginType).AddInstance(_instance);
            }

            // Using a referenced key for the profile
            else if (!string.IsNullOrEmpty(_instanceKey))
            {
                _instance = new ReferencedInstance(_instanceKey);
            }

            // Set the default instance in the Profile
            if (_instance != null)
            {
                pluginGraph.ProfileManager.SetDefault(profileName, _pluginType, _instance);
            }

            // Blow up if the Profile expression is not complete.
            else
            {
                throw new StructureMapException(304, TypePath.GetAssemblyQualifiedName(_pluginType));
            }
        }

        /// <summary>
        /// Define the default instance of the PluginType for the containing Profile
        /// </summary>
        /// <param name="mementoBuilder"></param>
        /// <returns></returns>
        public ProfileExpression Use(Instance instance)
        {
            // TODO -- validate that the instance can be plugged into the PluginType

            _instance = instance;

            return _parent;
        }
    }
}