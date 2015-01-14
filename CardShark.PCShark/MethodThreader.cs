using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Threading;
using System.Text;

namespace CardShark.PCShark
{
	/// <summary>
	/// A class that can be used to wrap around a static method, or non-static method of an object-instance, and make asynchronous calls to that method
	/// using a new thread for each call.
	/// </summary>
	public class MethodThreader : IDisposable
	{
		private object callObject;
		private MethodInfo callMethod;
		private object[] parameters = null;
		private Thread callThread = null;
		private string useThreadName = null;

		/// <summary>
		/// Event that is raised after the method called by this MethodThreader returns. Is executed on the same thread as the returning method call.
		/// 
		/// Can be used to retrieve the value returned by the method, since that is contained in the ThreadCompletedEventArgs object passed to handlers
		/// of this event.
		/// </summary>
		public event EventHandler<ThreadCompletedEventArgs> OnThreadCompleted;
		/// <summary>
		/// <para>Event that is raised when an exception occurs on a thread that was started my this MethodThreader. Is executed on the thread that threw the exception.
		/// If no handler is attached to this event, the MethodThreader will not attempt to catch such exceptions. Otherwise, it raises this event when they are caught.</para>
		/// <para> </para>
		/// <example><code>
		/// <para>Exception childThreadEx = null;</para>
		/// <para>bool waitOnChild = true;</para>
		/// <para>MethodThreader mThrdr = new MethodThreader(someObj, theFunc);</para>
		/// <para>mThrdr.OnExceptionThrown = (object sender, ExceptionThrownEventArgs e) => {</para>
		///	<para>	//Do exception stuff on child thread...</para>
		///	<para>	childThreadEx = e.Exception;</para>
		///	<para>	waitOnChild = false;</para>
		/// <para>};</para>
		/// <para>mThrdr.OnThreadCompleted = (object sender, ThreadCompletedEventArgs e) => {</para>
		///	<para>	waitOnChild = false;</para>
		/// <para>};</para>
		/// <para> </para>
		/// <para>mThrdr.Start(someArgArray);</para>
		/// <para>while (waitOnChild) ; //Have parent thread wait on child thread...</para>
		/// <para>if (childThreadEx != null) {</para>
		///	<para>	//Do Exception stuff on parent thread...</para>
		///	<para>}</para>
		/// </code></example>
		/// </summary>
		public event EventHandler<ExceptionThrownEventArgs> OnExceptionThrown;

		/// <summary>Gets a value indicating if the method that this MethodThreader calls takes any arguments: true if it does, otherwise false.</summary>
		public bool MethodHasParameters { get; private set; }
		/// <summary>Gets a value indicating if the method that this MethodThreader calls returns a value: true if it does, false if it is a "void" type method.</summary>
		public bool MethodReturnsValue { get; private set; }
		/// <summary>The name of the thread created by the call to this method threader, or that will be given to the thread when the call is made. 
		/// Equals "ThreadedCallTo_" + the name of the method to be called.</summary>
		public string ThreadName {
			get { return callThread == null ? useThreadName : callThread.Name; }
			private set { useThreadName = value; if (callThread != null) callThread.Name = value; }
		}
		/// <summary>An identifying # for use by the program using the method threader to identify this particular method threader and the call it makes.</summary>
		public int CallID { get; set; }
		/// <summary>
		/// A test description of the method this MethodThreader will call. Includes the method's name, followed by a CSV list of its parameter types 
		/// and names enclosed in parentheses.
		/// </summary>
		public string MethodSignature { get; private set; }
		/// <summary>
		/// Indicates if the MethodThreader has been disposed for garbage collection. A disposed MethodThreader will throw errors if calls are attempted with it. It also no
		/// longer keeps track of the thread it started if a call was made before it was disposed, but it does not terminate the thread when it IS disposed.  When
		/// appropriate, that thread will still call any handlers that were attached to the MethodThreader's OnThreadCompleted or OnExceptionThrown events.
		/// </summary>
		public bool Disposed { get; private set; }
		/// <summary>
		/// Indicates if the call made by the MethodThreader is unfinished. This property is not reliable if the MethodThreader has already been
		/// disposed. Also, if the call has completed, but its thread is still executing this MethodThreader's OnThreadCompleted event handler, it is considered finished.
		/// </summary>
		public bool CallIsRunning { get { return Disposed ? false : callThread.IsAlive; } }
		/// <summary>
		/// Indicates if the call made by the MethodThreader has finished running (or not yet started). This property is not reliable if the MethodThreader has already been
		/// disposed. Also, if the call has completed, but its thread is still executing this MethodThreader's OnThreadCompleted event handler, it is considered finished.
		/// </summary>
		public bool CallIsFinished { get { return Disposed ? true : !callThread.IsAlive; } }

		/// <summary>
		/// Creates a new MethodThreader instance which can be used to make a call to a given method on a new thread.
		/// </summary>
		/// <param name="callOn">The instance of the method's declaring type that the method will be called on if the method is not static. Can be null if the method IS static.</param>
		/// <param name="call">The method to call.</param>
		/// <exception cref="ArgumentNullException">Thrown if call is null, or if call is not static, and callOn is null.</exception>
		public MethodThreader(object callOn, MethodInfo call) : this(callOn, call, null, null) { }
		/// <summary>
		/// Creates a new MethodThreader instance which can be used to make a call to a given method on a new thread.
		/// </summary>
		/// <param name="callOn">The instance of the method's declaring type that the method will be called on if the method is not static. Can be null if the method IS static.</param>
		/// <param name="call">The method to call.</param>
		/// <param name="defaultArgs">The arguments that will be used automatically if the method has parameters and the call is made without specifying any (by calling thisMethodThreader.Start()).
		/// Can be null if the method takes no arguments, or if arguments will be specified when the call is made (by using thisMethodThreader.Start(object)). Must have the same number of elements, 
		/// of the same type, and in the same order, as the parameters of the method.</param>
		/// <exception cref="ArgumentNullException">Thrown if call is null, or if call is not static, and callOn is null.</exception>
		public MethodThreader(object callOn, MethodInfo call, object[] defaultArgs) : this(callOn, call, defaultArgs, null) { }
		/// <summary>
		/// Creates a new MethodThreader instance which can be used to make a call to a given method on a new thread.
		/// </summary>
		/// <param name="callOn">The instance of the method's declaring type that the method will be called on if the method is not static. Can be null if the method IS static.</param>
		/// <param name="call">The method to call.</param>
		/// <param name="defaultArgs">The arguments that will be used automatically if the method has parameters and the call is made without specifying any (by calling thisMethodThreader.Start()).
		/// Can be null if the method takes no arguments, or if arguments will be specified when the call is made (by using thisMethodThreader.Start(object)). Must have the same number of elements, 
		/// of the same type, and in the same order, as the parameters of the method.</param>
		/// <param name="onComplete">An event handler method to call after the call to the method finishes. Executes on the same thread that was created for the call, just before that thread exits. 
		/// More handlers can be added or removed later using the OnThreadCompleted event.
		/// <seealso cref="OnThreadCompleted"/></param>
		/// <exception cref="ArgumentNullException">Thrown if call is null, or if call is not static, and callOn is null.</exception>
		public MethodThreader(object callOn, MethodInfo call, object[] defaultArgs, EventHandler<ThreadCompletedEventArgs> onComplete)
			: this(callOn, call, defaultArgs, onComplete, null) { }
		/// <summary>
		/// Creates a new MethodThreader instance which can be used to make a call to a given method on a new thread.
		/// </summary>
		/// <param name="callOn">The instance of the method's declaring type that the method will be called on if the method is not static. Can be null if the method IS static.</param>
		/// <param name="call">The method to call.</param>
		/// <param name="defaultArgs">The arguments that will be used automatically if the method has parameters and the call is made without specifying any (by calling thisMethodThreader.Start()).
		/// Can be null if the method takes no arguments, or if arguments will be specified when the call is made (by using thisMethodThreader.Start(object)). Must have the same number of elements, 
		/// of the same type, and in the same order, as the parameters of the method.</param>
		/// <param name="onComplete">An event handler method to call after the call to the method finishes. Executes on the same thread that was created for the call, just before that thread exits. 
		/// More handlers can be added or removed later using the OnThreadCompleted event.
		/// <seealso cref="OnThreadCompleted"/></param>
		/// <param name="onException">An event handler method to call if an exception is thrown on the thread started by the MethodThreader. Executes on the same thread that was created for the call. 
		/// More handlers can be added or removed later using the OnExceptionThrown event.
		/// <seealso cref="OnExceptionThrown"/></param>
		/// <exception cref="ArgumentNullException">Thrown if call is null, or if call is not static, and callOn is null.</exception>
		public MethodThreader(object callOn, MethodInfo call, object[] defaultArgs, EventHandler<ThreadCompletedEventArgs> onComplete, EventHandler<ExceptionThrownEventArgs> onException) {
			if (call == null) throw new ArgumentNullException("call", "Cannot create a MethodThreader withohut a method to call.");
			this.callMethod = call;

			ParameterInfo[] parms = call.GetParameters();

			MethodHasParameters = parms.Length > 0;
			MethodReturnsValue = callMethod.ReturnType != typeof(void);

			StringBuilder bldr = new StringBuilder();
			for (int i = 0; i < parms.Length; i++) {
				bldr.Append(parms[i].ParameterType.Name);
				bldr.Append(' ');
				bldr.Append(parms[i].Name);
				if (i < parms.Length - 1)
					bldr.Append(", ");
			}
			string paramList = bldr.ToString();
			ThreadName = "ThreadedCallTo_" + call.Name + "(" + bldr.ToString() + ")";

			if (callMethod.IsStatic)
				this.callObject = null;
			else {
				if (callOn == null) {
					bldr.Clear();
					bldr.Append("Cannot create a MethodThreader without an instance to call a method on when the method used (\"");
					bldr.Append(call.DeclaringType.Name);
					bldr.Append('.');
					bldr.Append(call.Name);
					bldr.Append('(');
					bldr.Append(paramList);
					bldr.Append(')');
					bldr.Append(")\") is not a static method.");
					throw new ArgumentNullException("callOn", bldr.ToString());
				}
				this.callObject = callOn;
			}

			if (MethodHasParameters)
				this.parameters = defaultArgs;
			else
				this.parameters = null;

			if (onComplete != null)
				OnThreadCompleted += onComplete;
			if (onException != null)
				OnExceptionThrown += onException;

			Disposed = false;
		}

		/// <summary>
		/// Creates a new thread and calls this MethodThreader's method on that thread, returning immediately without waiting for the call to complete. If the method takes
		/// arguments, uses the default arguments specified at the creation time of this MethodThreader.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the method threader has been disposed, or is still completing a previous call.</exception>
		public void Start() {
			if (Disposed)
				throw new InvalidOperationException("Cannot make calls with a disposed MethodThreader.");
			if (callThread != null && callThread.IsAlive)
				throw new InvalidOperationException("Cannot make calls with a method threader that is still completing a previous call.");

			if (OnExceptionThrown == null)
				callThread = new Thread(parameterlessStart) { Name = useThreadName };
			else
				callThread = new Thread(safeParamlessStart) { Name = useThreadName };
			callThread.Start();
		}
		/// <summary>
		/// Creates a new thread and calls this MethodThreader's method on that thread, returning immediately without waiting for the call to complete. Ignores the default
		/// arguments specified at the creation time of this MethodThreader, attempting to use the ones specified by the "args" parameter instead, even if it is null or contains
		/// null values.
		/// </summary>
		/// <param name="args">The argument to pass to the method on this call. If the method takes multiple arguments, this should be an object array with the same type, #, and order as the
		/// parameters of the method. If the method only takes one argument, it can simply be an object of the same type as that parameter. If the method takes no arguments, this parameter will
		/// be ignored.</param>
		public void Start(object args) {
			if (Disposed)
				throw new InvalidOperationException("Cannot make calls with a disposed MethodThreader.");
			if (callThread != null && callThread.IsAlive)
				throw new InvalidOperationException("Cannot make calls with a method threader that is still completing a previous call.");

			if (OnExceptionThrown == null)
				callThread = new Thread(parameterizedStart) { Name = useThreadName };
			else
				callThread = new Thread(safeParamStart) { Name = useThreadName };
			callThread.Start(args);
		}

		private void parameterizedStart(object arg) {
			object returnVal = null;
			if (MethodHasParameters) {
				if (!(arg is Array))
					arg = new object[] { arg };
				if (MethodReturnsValue)
					returnVal = callMethod.Invoke(callObject, (object[])arg);
				else
					callMethod.Invoke(callObject, (object[])arg);
			}
			else {
				if (MethodReturnsValue)
					returnVal = callMethod.Invoke(callObject, null);
				else
					callMethod.Invoke(callObject, null);
			}
			callThread = null;
			if (OnThreadCompleted != null)
				OnThreadCompleted(this, new ThreadCompletedEventArgs(returnVal));
		}
		private void parameterlessStart() {
			object returnVal = null;
			if (MethodReturnsValue)
				returnVal = callMethod.Invoke(callObject, parameters);
			else
				callMethod.Invoke(callObject, parameters);
			callThread = null;
			if (OnThreadCompleted != null)
				OnThreadCompleted(this, new ThreadCompletedEventArgs(returnVal));
		}
		private void safeParamStart(object arg) {
			try { parameterizedStart(arg); }
			catch (Exception ex) { OnExceptionThrown(this, new ExceptionThrownEventArgs(ex)); }
		}
		private void safeParamlessStart() {
			try { parameterlessStart(); }
			catch (Exception ex) { OnExceptionThrown(this, new ExceptionThrownEventArgs(ex)); }
		}

		/// <summary>
		/// <para>Detaches all objects referenced by the MethodThreader from the MethodThreader (its current call thread, default parameters, etc...).
		/// Use when all calls that will be made with the threader have been made and completed.
		/// A disposed MethodThreader will throw errors if calls are attempted with it. It also no longer keeps track of any threads that it started for calls made before it
		/// was disposed, but it does not terminate those threads when it IS disposed.  When appropriate, those threads will still call any handlers that were attached to
		/// the MethodThreader's OnThreadCompleted or OnExceptionThrown events.</para>
		/// <para>!!NOTE!!: If an object array was given to the threader to use as default parameters, it will set all members of that array to null! Keep 
		/// this in mind if using that same array instance elswhere in your program.</para>
		/// </summary>
		public void Dispose() {
			Disposed = true;
			callObject = null;
			callMethod = null;
			for (int i = 0; i < parameters.Length; i++)
				parameters[i] = null;
			parameters = null;
			callThread = null;
		}
	}

	/// <summary>
	/// Represents info about the event that happens when a multi-threaded asynchronous call is made to method, after that that method returns. Contains the value returned by
	/// that method (if any).
	/// </summary>
	public class ThreadCompletedEventArgs : EventArgs
	{
		public ThreadCompletedEventArgs() : this(null) { }
		public ThreadCompletedEventArgs(object returnVal) { this.ReturnValue = returnVal; }
		/// <summary>The value returned by the method call, or null if the method was a void method or actually returned null.</summary>
		public object ReturnValue { get; set; }
		/// <summary>The thread that raised the OnThreadCompleted event. Usually the same as Thread.CurrentThread when executing within the method that handles the
		/// OnThreadCompleted event.</summary>
		public Thread CompletedThread { get; set; }
	}
	/// <summary>
	/// Represents info about the event that happens when an exception is thrown on a thread started by a MethodThreader object. Contains the exception thrown.
	/// </summary>
	public class ExceptionThrownEventArgs : EventArgs
	{
		public ExceptionThrownEventArgs() : this(null) { }
		public ExceptionThrownEventArgs(Exception exception) { this.Exception = exception; }
		/// <summary>The Exception that was thrown.</summary>
		public Exception Exception { get; set; }
		/// <summary>The thread that threw the exeption and raised the OnExceptionThrown event. Usually the same as Thread.CurrentThread when executing within the method
		/// that handles the OnExceptionThrown event.</summary>
		public Thread ThrowingThread { get; set; }
	}
}