using Nancy.Hosting.Self;
using Nancy;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CGSS_Stamina_CalcAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            HostConfiguration hostConfigs = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            using (var host = new NancyHost(new Uri("http://localhost:50120"), new DefaultNancyBootstrapper(), hostConfigs))
            {
                host.Start();
                Console.WriteLine("Running on http://*:50120");
                while(true){}
            }
        }
    }
    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get("/{current:int}/{max:int}/{remaining?}", async args =>
            {
                StaminaTime requestedtime = null;
                try
                {
                    string remaining = args.remaining;
                    requestedtime = new StaminaTime(args.current, args.max, remaining ?? "00:00");
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return await this.Response.AsJson(ex.Message).WithStatusCode(400);
                }
                catch (ArgumentException ex)
                {
                    return await this.Response.AsJson(ex.Message).WithStatusCode(400);
                }
                return await this.Response.AsJson(new
                {
                    MaxStaminaTime_ISO8601 = requestedtime.MaxStaminaTime_ISO8601,
                    MaxStaminaTime_Unix = requestedtime.MaxStaminaTime_Unix,
                    MaxStaminaTime_Str = requestedtime.MaxStaminaTime_Unix_Str
                });
            });
        }
    }
    public class StaminaTime
    {
        public int CurrentStamina { get; private set; }
        public int MaxStamina { get; private set; }
        public TimeSpan CurrentRemainingTime { get; private set; }

        public DateTime MaxStaminaTime { get; private set; }
        public string MaxStaminaTime_ISO8601 { get; private set; }
        public long MaxStaminaTime_Unix { get; private set; }
        public string MaxStaminaTime_Unix_Str { get; private set; }
        public StaminaTime(int current, int max, string remainingnext)
        {
            this.CurrentStamina = current;
            this.MaxStamina = max;
            if (this.CurrentStamina >= this.MaxStamina)
            {
                throw new ArgumentException("Current Stamina Must be Lower than Max Stamina");
            }
            string[] remaining_mmssArray = remainingnext.Split(":");
            if (remaining_mmssArray.Length < 2)
            {
                throw new ArgumentException("CurrentRemainingTime Parse Error");
            }

            if (!int.TryParse(remaining_mmssArray[0], out int mm) || !int.TryParse(remaining_mmssArray[1], out int ss))
            {
                throw new ArgumentException("CurrentRemainingTime Parse Error");
            }
            if (mm > 5 || ss > 60)
            {
                throw new ArgumentOutOfRangeException("CurrentRemainingTime OutofRange");
            }
            this.CurrentRemainingTime = new TimeSpan(0, mm, ss);

            var currentTime = DateTime.Now;
            var expendedStamina = this.MaxStamina - this.CurrentStamina - 1;
            var recoveryTimeSpan = new TimeSpan(0, expendedStamina * 5, 0);
            this.MaxStaminaTime = currentTime.Add(CurrentRemainingTime).Add(recoveryTimeSpan);

            this.MaxStaminaTime_ISO8601 = this.MaxStaminaTime.ToString("yyyy-MM-dd'T'HH:mm:sszzzz");
            this.MaxStaminaTime_Unix = new DateTimeOffset(this.MaxStaminaTime.Ticks, new TimeSpan(9, 0, 0)).ToUnixTimeSeconds();
            this.MaxStaminaTime_Unix_Str = this.MaxStaminaTime_Unix.ToString();
        }
    }
}
