using System;
using System.Collections;

namespace InfoHub.Common
{
	/// <summary>
	/// Interface for a thread-safe queue class, which provides a threading-aware
	/// Queue container
	/// </summary>
	public interface IThreadsafeQueue : ICollection, IEnumerable {
		/*
		 * not sure this is what I want.  For most of my queues, what would be really cool
		 * is a queue class that can sink a particular event, queue it, then expose it
		 * to another thread on the other end.  if the queue can persist its state to an xml
		 * document on command, that's all the cooler.
		 * 
		 * This way, I could synchronously couple systems if I wanted to, or not
		 * if I didn't, simply by adjusting the Spring.NET config.
		 */
	}
}
