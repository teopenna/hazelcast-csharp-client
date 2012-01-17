using System;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Concurrent;
using Hazelcast.Client.Impl;

namespace Hazelcast.Client
{
	public class InThread
	{
		
		private TcpClient tcpClient;
		private ConcurrentDictionary<long, Call> calls;
		private ListenerManager listenerManager;
		private volatile bool headerRead = false;
		
		
		public InThread (TcpClient tcpClient, ConcurrentDictionary<long, Call> calls, ListenerManager listenerManager)
		{
			this.tcpClient = tcpClient;
			this.calls = calls;
			this.listenerManager = listenerManager;
		}
		
		public void run(){
			while(true){
				if(!headerRead)
				{
					byte[] header = new byte[3];
					this.tcpClient.GetStream().Read(header, 0, 3);
					if(equals(header, Packet.HEADER)){
						
					}
					headerRead = true;
				}
				NetworkStream stream = tcpClient.GetStream();
				if(!stream.CanRead){
					Thread.Sleep(100);
					continue;
				}
				Packet packet = new Packet();
				packet.read(stream);
				if(calls.ContainsKey(packet.callId)){
					Call call = calls[packet.callId];
					call.setResult(packet);	
				}
				else{
					if(packet.operation == (byte)ClusterOperation.EVENT){
						listenerManager.enQueue(packet);		
					} 
					else
					{
						Console.WriteLine("Unkown call result: " + packet.callId + ", " + packet.operation);
					}
					
				}
				
				
			}
			
		}
		
		public static bool equals(byte[] b1, byte[] b2){
			if(b1.Length!=b2.Length){
				return false;
			}
			for(int i=0;i<b1.Length;i++){
				if(b1[i]!=b2[i]){
					return false;
				}
			}
			
			return true;
			
		}
		
		
		
		
		
		public static InThread start(TcpClient tcpClient, ConcurrentDictionary<long, Call> calls, ListenerManager listenerManager){
			InThread inThread = new InThread(tcpClient, calls, listenerManager);
			Thread thread =  new Thread(new ThreadStart(inThread.run));
			thread.Start();
			return inThread;
		}
	}
}