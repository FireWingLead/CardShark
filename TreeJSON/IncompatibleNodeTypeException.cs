using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeJSON
{
	public class IncompatibleNodeTypeException : Exception
	{
		public IncompatibleNodeTypeException(NodeTypes expectedType, NodeTypes actualType, object actualValue)
			: base("Could not convert node value from type \"" + expectedType.ToString() + "\" to type \"" + actualType.ToString() + "\".") {
				ExpectiedType = expectedType;
				ActualType = actualType;
				ActualValue = actualValue;
		}

		public NodeTypes ExpectiedType { get; private set; }
		public NodeTypes ActualType { get; private set; }
		public object ActualValue { get; private set; }
	}
}
