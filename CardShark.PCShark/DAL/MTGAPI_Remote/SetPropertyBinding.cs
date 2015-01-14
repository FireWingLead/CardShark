using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TreeJSON;

namespace CardShark.PCShark.DAL.MTGAPI_Remote
{
	public abstract class SetPropertyBinding : PropertyBinding
	{
		internal SetPropertyBinding(PropertyInfo localProp, string remoteName, NodeTypes remoteType)
			: base(localProp, remoteName, remoteType) { }

		public abstract object RemoteToLocal(object remVal);
		public abstract object LocalToRemote(object locVal);

		public static readonly SetPropertyBinding<string, string> Code = new SetPropertyBinding<string, string>(
			SetAdapter.LOCAL_MODEL_TYPE.GetProperty("Code"), "code", NodeTypes.String, val => val, val => val
		);
		public static readonly SetPropertyBinding<string, string> Name = new SetPropertyBinding<string, string>(
			SetAdapter.LOCAL_MODEL_TYPE.GetProperty("Name"), "name", NodeTypes.String, val => val, val => val
		);
	}

	public class SetPropertyBinding<LocType, RemType> : SetPropertyBinding
	{
		internal SetPropertyBinding(PropertyInfo localProp, string remoteName, NodeTypes remoteType,
			Func<RemType, LocType> remoteToLocal, Func<LocType, RemType> localToRemote)
			: base(localProp, remoteName, remoteType) {
			RemToLoc = remoteToLocal;
			LocToRem = localToRemote;
		}

		public readonly Func<RemType, LocType> RemToLoc;
		public readonly Func<LocType, RemType> LocToRem;

		public override object RemoteToLocal(object remVal) { return RemToLoc((RemType)remVal); }
		public override object LocalToRemote(object locVal) { return LocToRem((LocType)locVal); }
	}
}
