using System;
using SimpleStepWriter;
using SimpleStepWriter.Helper;

namespace SimpleStepWriterCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            // http://patorjk.com/software/taag/#p=testall&f=Graffiti&t=Step%20Writer%20
            Console.WriteLine(@"               
   _________________    _      __    _ __         
  / __/_  __/ __/ _ \  | | /| / /___(_) /____ ____
 _\ \  / / / _// ___/  | |/ |/ / __/ / __/ -_) __/
/___/ /_/ /___/_/      |__/|__/_/ /_/\__/\__/_/   

            ");

            Console.WriteLine(
                "Write simple geometry data into STEP AP214 (ISO-10303) file format.\n"
                + "version: " + typeof(StepFile).Assembly.GetName().Version + " [pre-release] \n"
            );
            
            const string FILEPATH = @"C:\Users\me\Documents\local\file.step";

            StepFile stepFile = new StepFile(FILEPATH, "RootAssembly");

            stepFile.AddBox(
               name: "origin",
               center: new Vector3(0, 0, 0),
               dimension: new Vector3(10, 10, 10),
               rotation: new Vector3(0, 0, 0),
               color: Color.Red,
               parentId: 0
            );
             
            // let's build a workstation

            int workstationId = stepFile.AddGroup(
               name: "workstation",
               center: new Vector3(0, 0, 0),
               rotation: new Vector3(0, 0, 0),
               parentId: 0
              );

            int tableId = stepFile.AddGroup(
                name: "table",
                center: new Vector3(0, 0, 0),               
                rotation: new Vector3(0, 0, 0),
                parentId: workstationId
               );

            int stuffId = stepFile.AddGroup(
                name: "stuff",
                center: new Vector3(0, 0, 630),
                rotation: new Vector3(0, 0, 0),
                parentId: workstationId
             );

            int foundationId = stepFile.AddGroup(
                name: "foundation",
                center: new Vector3(0, 0, 0),
                rotation: new Vector3(0, 0, 0),
                parentId: tableId
               );

            stepFile.AddBox(
                name: "leg1",
                center: new Vector3(-750, -250, 300),
                dimension: new Vector3(100, 100, 600),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green,
                parentId: foundationId
               );

            stepFile.AddBox(
                name: "leg2",
                center: new Vector3(750, -250, 300),
                dimension: new Vector3(100, 100, 600),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green,
                parentId: foundationId
               );

            stepFile.AddBox(
                name: "leg3",
                center: new Vector3(-750, 250, 300),
                dimension: new Vector3(100, 100, 600),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green,
                parentId: foundationId
               );

            stepFile.AddBox(
                name: "leg4",
                center: new Vector3(750, 250, 300),
                dimension: new Vector3(100, 100, 600),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green,
                parentId: foundationId
               );

            stepFile.AddBox(
                name: "tabletop",
                center: new Vector3(0, 0, 615),
                dimension: new Vector3(1600, 600, 30),
                rotation: new Vector3(0, 0, 0),
                color: Color.Blue,
                parentId: foundationId
               );

            int drawerId = stepFile.AddGroup(
               name: "drawer",
               center: new Vector3(-550, 0, 450),
               rotation: new Vector3(0, 0, 0),
               parentId: tableId
              );

            stepFile.AddBox(
                name: "walls",
                center: new Vector3(0, 50, 0),
                dimension: new Vector3(300, 500, 300),
                rotation: new Vector3(0, 0, 0),
                color: Color.Yellow,
                parentId: drawerId
               );

            stepFile.AddBox(
                name: "knob1",
                center: new Vector3(0, -225, 50),
                dimension: new Vector3(100, 50, 35),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentId: drawerId
               );

            stepFile.AddBox(
                name: "knob2",
                center: new Vector3(0, -225, -60),
                dimension: new Vector3(100, 50, 35),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentId: drawerId
               );

            stepFile.AddBox(
                name: "book1",
                center: new Vector3(540, 35, 30),
                dimension: new Vector3(250, 360, 60),
                rotation: new Vector3(0, 0, -35),
                color: Color.White,
                parentId: stuffId
               );

            stepFile.AddBox(
                name: "book2",
                center: new Vector3(540, 35, 90),
                dimension: new Vector3(250, 360, 60),
                rotation: new Vector3(0, 0, -38),
                color: Color.Yellow,
                parentId: stuffId
               );

            int monitorId = stepFile.AddGroup(
               name: "monitor",
               center: new Vector3(0, 170, 0),
               rotation: new Vector3(0, 0, -4),
               parentId: stuffId
              );

            stepFile.AddBox(
                name: "socket",
                center: new Vector3(0, 0, 20),
                dimension: new Vector3(250, 150, 40),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentId: monitorId
               );

            stepFile.AddBox(
                name: "arm",
                center: new Vector3(0, 0, 190),
                dimension: new Vector3(100, 30, 300),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentId: monitorId
               );

            stepFile.AddBox(
                name: "display",
                center: new Vector3(0, -10, 340),
                dimension: new Vector3(600, 20, 338),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentId: monitorId
               );

            stepFile.AddBox(
                name: "keyboard",
                center: new Vector3(-110, -75, 0),
                dimension: new Vector3(560, 185, 40),
                rotation: new Vector3(0, 0, -6),
                color: Color.Red,
                parentId: stuffId
               );
            
            Console.WriteLine("Want to write the STEP file with sample content to:\n" + FILEPATH + " ?");           
            Console.ReadKey();

            bool result = stepFile.WriteFile();

            if (result)
                Console.WriteLine("Success.");
            else
                Console.WriteLine("Failure.");

            Console.ReadKey();
        }
    }    
}
