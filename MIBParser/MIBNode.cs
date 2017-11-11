using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBParser
{
    public class MIBNode
    {
        public List<MIBNode> Children { get; private set; }
        public int NodeId { get; private set; }
        public string NodeName { get; private set; }

        public MIBNode(int nodeId, string nodeName)
        {
            NodeId = nodeId;
            NodeName = nodeName;
            Children = new List<MIBNode>();
        }

        public void AddChild(MIBNode child)
        {
            Children.Add(child);
        }
    }
}
