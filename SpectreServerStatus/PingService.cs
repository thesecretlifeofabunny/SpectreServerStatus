using System.Net.NetworkInformation;

namespace SpectreServerStatus;

public static class PingService
{
    public static void PingServer(string serverToPing)
    {
        Ping pingSender = new();
        const int timeout = 120;
        
        var reply = pingSender.Send (serverToPing, timeout);
        
        if (reply.Status != IPStatus.Success) return;
        
        Console.WriteLine ("Address: {0}", reply.Address.ToString ());
        Console.WriteLine ("RoundTrip time: {0}", reply.RoundtripTime);
        
        if (reply.Options is not null)
        {
            Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
            Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
        }

        Console.WriteLine ("Buffer size: {0}", reply.Buffer.Length);

        const int fiveSeconds = 5000;
        Console.WriteLine("Sleeping for five seconds....");
        Thread.Sleep(fiveSeconds);
        Console.WriteLine("---------------------------------------------------");
    }    
}