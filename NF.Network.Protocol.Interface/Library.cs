namespace NF.Network.Protocol.Interface
{
	// ref: https://github.com/SaladLab/Akka.Interfaced/blob/master/core/Akka.Interfaced-Base/IInterfacedActor.cs
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public interface IInterfacedActor
	{
	}

	public enum TaskStatus
	{
		Created,
		WaitingForActivation,
		WaitingToRun,
		Running,
		WaitingForChildrenToComplete,
		RanToCompletion,
		Canceled,
		Faulted
	}

	public interface Task
	{
		object WaitHandle { get; }
		Exception Exception { get; }
		TaskStatus Status { get; }
	}

	public interface Task<T> : Task
	{
		T Result { get; }
	}

	public interface Sender<Q>
	{
            Task<R> Send<R> (Q msg) where R : new();
	}
}
