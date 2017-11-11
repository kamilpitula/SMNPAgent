using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static string GetString(this MIBNode root, string nodeName=null)
        {
            var builder = new StringBuilder();


            foreach (var child in root.Children)
            {
                if (String.IsNullOrEmpty(nodeName)&&child.Children.Count==0)
                {
                    BuildString(builder, child);
                }
                
                if (child.NodeName == nodeName)
                {
                    BuildString(builder, child);
                }
                builder.Append(child.GetString(nodeName));
                
            }

            return builder.ToString();
        }

        private static void BuildString(StringBuilder builder, MIBNode child)
        {
            MIBNode temp = child;
            var idList = new List<int>();
            while (temp.Parent != null)
            {

                temp = temp.Parent;
                idList.Add(temp.NodeId);
            }
            for (int j = idList.Count - 1; j >= 0; j--)
            {
                builder.Append(idList[j].ToString());
                if (j != 0)
                    builder.Append('.');
            }
            builder.Append(' ');
            builder.AppendLine(child.NodeName);
        }
    }
}