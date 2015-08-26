using System;
using System.Threading;
using Hazelcast.Client.Test;
using Hazelcast.Core;
using Hazelcast.Net.Ext;
using Hazelcast.Transaction;
using NUnit.Framework;

namespace Hazelcast.Client.Test
{
	[TestFixture]
	public class ClientTxnTest:HazelcastBaseTest
	{

        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public static void Destroy()
        {
        }
		/// <exception cref="System.Exception"></exception>
		[Test,Ignore]
		public virtual void TestTxnRollback()
		{
		    var name = Name;
			ITransactionContext context = Client.NewTransactionContext();
            CountdownEvent latch = new CountdownEvent(1);
			try
			{
				context.BeginTransaction();
				Assert.IsNotNull(context.GetTxnId());
                var queue = context.GetQueue<object>(name);
				queue.Offer("item");

                //FIXME At this point server should get shut down
				//server.getLifecycleService().shutdown();
				context.CommitTransaction();
				Assert.Fail("commit should throw exception!!!");
			}
			catch (Exception)
			{
				context.RollbackTransaction();
				latch.Signal();
			}
			Assert.IsTrue(latch.Wait(TimeSpan.FromSeconds(10)));
			//
            IQueue<object> q = Client.GetQueue<object>(name);
			Assert.IsNull(q.Poll());
			Assert.AreEqual(0, q.Count);
		}
	}
}
