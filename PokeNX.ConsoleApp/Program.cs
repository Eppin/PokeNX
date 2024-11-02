using PokeNX.ConsoleApp;
using PokeNX.Core;
using PokeNX.Core.Generators;
using PokeNX.Core.Models.Enums;

Console.WriteLine("Hello, World!");

Console.WriteLine($"{2147483648:X16} vs {18446744071562088538:X16}");

var eggGen81 = new EggGenerator8();
//eggGen81.Generate(0x2b2d03059a0f0a0, 0x66873774675c9d10);
// eggGen81.Generate(0x2b2d03059a0f0ad, 0x66873774675c9d1c);
// eggGen81.Generate(10, 1);
// Console.WriteLine("---");
// eggGen81.Generate(1423436453, 923445423);
eggGen81.Generate(0, 10);
return;

var s = new DiamondPearlService();
s.Connect("192.168.25.220", 6000);

var (s0, s1) = s.MainRNG();
Console.WriteLine($"[S0]: {s0:X16}, [S1]: {s1:X16}, [{s0},{s1}]");

var eggGen8 = new TestEgg8();
eggGen8.Generate(s0, s1);

return;

// var tmpRNG = new XorShift(s0, s1);
// Console.WriteLine($"{tmpRNG.Next():X32}");
// Console.WriteLine($"{tmpRNG.State():X32}");
// TestMainRNG(s, tmpRNG);

while (true)
{
    var eggDetails = s.GetDayCareDetails(Game.BrilliantDiamond);
    Console.WriteLine($"Exists: {eggDetails.Exists}, Seed: {eggDetails.Seed:X8}, Step count: {eggDetails.StepCount}");
    await Task.Delay(100);
}

// void TestMainRNG(DiamondPearlService b, XorShift tmpRNG)
// {
//     var advances = 0;
//
//     var (tmpS0, tmpS1) = tmpRNG.Seed();
//
//     while (true)
//     {
//         var (ramS0, ramS1) = b.MainRNG();
//
//         while (ramS0 != tmpS0 || ramS1 != tmpS1)
//         {
//             tmpRNG.Next();
//             (tmpS0, tmpS1) = tmpRNG.Seed();
//             advances++;
//
//             if (ramS0 == tmpS0 && ramS1 == tmpS1)
//             {
//                 var rngClone = new XorShift(ramS0, ramS1);
//
//                 Console.WriteLine($"Advances: {advances}");
//                 Console.WriteLine(tmpRNG);
//                 Console.WriteLine();
//             }
//         }
//     }
// }
