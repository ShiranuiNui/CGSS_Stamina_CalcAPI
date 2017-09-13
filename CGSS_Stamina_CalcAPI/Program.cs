﻿using Nancy.Hosting.Self;
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
            Console.WriteLine("Hello World!");
            HostConfiguration hostConfigs = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            using (var host = new NancyHost(new Uri("http://localhost:50120"), new DefaultNancyBootstrapper(), hostConfigs))
            {
                host.Start();
                Console.WriteLine("Running on http://*:50120");
                Console.ReadLine();
            }
        }
    }
    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get("/", args => "RUNNING!!!");
        }
    }
    public class StaminaTime
    {
        public int CurrentStamina { get; private set; }
        public int MaxStamina { get; private set; }
        public TimeSpan CurrentRemainingTime { get; private set; }

        public DateTime MaxStaminaTime { get; private set; }
        public long MaxStaminaTime_Unix { get; private set; }
        public string MaxStaminaTime_Unix_Str { get; private set; }
        public StaminaTime(int current, int max, string remainingnext)
        {
            this.CurrentStamina = current;
            this.MaxStamina = max;

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
                throw new ArgumentOutOfRangeException("CurrentRemainingTime OutofRange Error");
            }
            this.CurrentRemainingTime = new TimeSpan(0, mm, ss);

            var currentTime = DateTime.Now;
            var expendedStamina = this.MaxStamina - this.CurrentStamina - 1;
            var recoveryTimeSpan = new TimeSpan(0, expendedStamina * 5, 0);
            this.MaxStaminaTime = currentTime.Add(CurrentRemainingTime).Add(recoveryTimeSpan);
        }
    }
}
