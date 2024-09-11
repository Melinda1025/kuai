// See https://aka.ms/new-console-template for more information

using SDBS3000.Core.Utils;

var err = ArtDAQ.ArtDAQ_CreateTask("test", out IntPtr task);
ArtDAQ.ArtDAQ_StopTask(task);
ArtDAQ.ArtDAQ_ClearTask(task);
Console.WriteLine(err);