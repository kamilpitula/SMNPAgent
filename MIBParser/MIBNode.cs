using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBParser
{
    public class MIBNode
    {
        private List<MIBNode> children;
        private int nodeId;
        private string nodeName;

        public MIBNode(List<MIBNode> children, int nodeId, string nodeName)
        {
            this.children = children;
            this.nodeId = nodeId;
            this.nodeName = nodeName;
            children = new List<MIBNode>();
        }

        public void AddChild(MIBNode child)
        {
            children.Add(child);
        }
    }
}
