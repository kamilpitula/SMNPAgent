using System.Collections.Generic;
using System.Linq;

namespace MIBParser
{
    public static class MIBTreeExtensions
    {
        public static IEnumerable<MIBNode> GetMibNodeStack(this MIBNode root)
        {
            var nodes = new Stack<MIBNode>(new[] { root });
            while (nodes.Any())
            {
                MIBNode node = nodes.Pop();
                yield return node;
                foreach (var n in node.Children) nodes.Push(n);
            }
        }
    }
}