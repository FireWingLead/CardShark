using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeJSON
{
	public class NodeValueOutOfBoundsException : Exception
	{
		public NodeValueOutOfBoundsException(Type expectedType, object limitValue, object actualValue, bool isGreater)
			: base("Node value " + actualValue.ToString() + " was too " + (isGreater ? "large" : "small") + " for desired type \"" + expectedType.Name
			+ "\", whose " + (isGreater ? "max" : "min") + " value is " + actualValue.ToString() + ".") {
				ExpectedType = expectedType;
				LimitValue = limitValue;
				ActualValue = actualValue;
				IsGreater = isGreater;
		}
		public Type ExpectedType { get; private set; }
		public Object LimitValue { get; private set; }
		public Object ActualValue { get; private set; }
		public bool IsGreater { get; private set; }
		public bool IsLesser { get { return !IsGreater; } }
	}
}
