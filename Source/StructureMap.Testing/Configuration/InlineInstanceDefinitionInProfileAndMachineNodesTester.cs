using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class InlineInstanceDefinitionInProfileAndMachineNodesTester
    {
        #region Setup/Teardown


        private PluginGraph graph
        {
            get
            {
                string xml = @"
<StructureMap MementoStyle='Attribute'>
  <Assembly Name='StructureMap.Testing.Widget'/>

  <Profile Name='Blue'>
    <Override Type='StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget'>
      <Instance Type='Color' Color='Blue'/>
    </Override>
    <Override Type='StructureMap.Testing.Widget.Rule,StructureMap.Testing.Widget'>
      <Instance Type='Color' Color='Blue'/>
    </Override>
  </Profile>

  <Profile Name='Green'>
    <Override Type='StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget'>
      <Instance Type='Color' Color='Green'/>
    </Override>
    <Override Type='StructureMap.Testing.Widget.Rule,StructureMap.Testing.Widget'>
      <Instance Type='Color' Color='Green'/>
    </Override>
  </Profile>

  <Machine Name='GREEN-BOX' Profile='Green'/>

  <Machine Name='ORANGE-BOX'>
    <Override Type='StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget'>
      <Instance Type='Color' Color='Orange'/>
    </Override>
  </Machine>
</StructureMap>
";


                return DataMother.CreateFromXml(xml);                
            }
        }

        #endregion


        public void TearDown()
        {
            ProfileBuilder.ResetMachineName();
        }

        [Test]
        public void CanFindTheTwoPluginFamilies()
        {
            Assert.IsTrue(graph.PluginFamilies.Contains(typeof(IWidget)));
            Assert.IsTrue(graph.PluginFamilies.Contains(typeof(Rule)));
        }

        [Test]
        public void CanRenameInstanceMemento()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "name");

            Assert.AreEqual("name", memento.InstanceKey);
            memento.InstanceKey = "Elvis";

            Assert.AreEqual("Elvis", memento.InstanceKey);
        }

        [Test]
        public void GettingTheRightInstanceKeyWhenUsingAMAchineOverrideInCombinationWithProfile()
        {
            ProfileBuilder.OverrideMachineName("GREEN-BOX");
            InstanceManager manager = new InstanceManager(graph);

            ColorWidget widget = (ColorWidget) manager.CreateInstance<IWidget>();
            Assert.AreEqual("Green", widget.Color);
        }

        [Test]
        public void GotTheInstanceForTheMachineOverride()
        {
            ProfileBuilder.OverrideMachineName("ORANGE-BOX");
            InstanceManager manager = new InstanceManager(graph);

            ColorWidget widget = (ColorWidget)manager.CreateInstance<IWidget>();
            Assert.AreEqual("Orange", widget.Color);
        }

        [Test]
        public void HasADefaultInstanceKey()
        {
            InstanceManager manager = new InstanceManager(graph);
            
            manager.SetDefaultsToProfile("Green");

            ColorWidget widget = (ColorWidget)manager.CreateInstance<IWidget>();
            Assert.AreEqual("Green", widget.Color);
        }

        [Test]
        public void HasTheOverrideForProfile()
        {
            InstanceManager manager = new InstanceManager(graph);
            manager.SetDefaultsToProfile("Blue");

            ColorRule rule = (ColorRule) manager.CreateInstance<Rule>();
            Assert.AreEqual("Blue", rule.Color);

            ColorWidget widget = (ColorWidget) manager.CreateInstance<IWidget>();
            Assert.AreEqual("Blue", widget.Color);

        }


        [Test]
        public void InlineMachine1()
        {
            ProfileBuilder.OverrideMachineName("ORANGE-BOX");
            InstanceManager manager = new InstanceManager(graph);

            ColorWidget widget = (ColorWidget)manager.CreateInstance(typeof(IWidget));
            Assert.AreEqual("Orange", widget.Color);
        }

        [Test]
        public void InlineMachine2()
        {
            ProfileBuilder.OverrideMachineName("GREEN-BOX");
            InstanceManager manager = new InstanceManager(graph);
            
            ColorWidget widget = (ColorWidget)manager.CreateInstance(typeof(IWidget));
            Assert.AreEqual("Green", widget.Color);
        }

        [Test]
        public void SetTheProfile()
        {
            InstanceManager manager = new InstanceManager(graph);
            manager.SetDefaultsToProfile("Green");

            ColorRule greenRule = (ColorRule)manager.CreateInstance(typeof(Rule));
            Assert.AreEqual("Green", greenRule.Color);

            manager.SetDefaultsToProfile("Blue");

            ColorRule blueRule = (ColorRule)manager.CreateInstance(typeof(Rule));
            Assert.AreEqual("Blue", blueRule.Color);
        }
    }
}