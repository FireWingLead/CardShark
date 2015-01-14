using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TreeJSON;

namespace CardShark.PCShark.DAL.MTGAPI_Remote
{
	public abstract class PropertyBinding
	{
		internal PropertyBinding(PropertyInfo localProp, string remoteName, NodeTypes remoteType) {
			RemoteType = remoteType;
			LocalProperty = localProp;
			RemoteName = remoteName;
		}
		
		public readonly PropertyInfo LocalProperty;
		public readonly string RemoteName;
		public readonly NodeTypes RemoteType;
	}
}
