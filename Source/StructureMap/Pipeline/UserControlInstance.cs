using System;
using System.Web.UI;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class UserControlInstance : ExpressedInstance<UserControlInstance>
    {
        private readonly string _url;

        public UserControlInstance(string url)
        {
            _url = url;
        }

        protected override UserControlInstance thisInstance
        {
            get { return this; }
        }


        public string Url
        {
            get { return _url; }
        }

        protected override object build(Type pluginType, IBuildSession session)
        {
            Control control = new Page().LoadControl(_url);

            Type pluggedType = control.GetType();
            if (!TypeRules.CanBeCast(pluginType, pluggedType))
            {
                throw new StructureMapException(303, pluginType, pluggedType);
            }

            return control;
        }

        protected override string getDescription()
        {
            return "UserControl at " + _url;
        }
    }
}