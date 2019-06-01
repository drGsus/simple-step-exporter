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
     _______.___________. _______ .______      ____    __    ____ .______       __  .___________. _______ .______      
    /       |           ||   ____||   _  \     \   \  /  \  /   / |   _  \     |  | |           ||   ____||   _  \     
   |   (----`---|  |----`|  |__   |  |_)  |     \   \/    \/   /  |  |_)  |    |  | `---|  |----`|  |__   |  |_)  |    
    \   \       |  |     |   __|  |   ___/       \            /   |      /     |  |     |  |     |   __|  |      /     
.----)   |      |  |     |  |____ |  |            \    /\    /    |  |\  \----.|  |     |  |     |  |____ |  |\  \----.
|_______/       |__|     |_______|| _|             \__/  \__/     | _| `._____||__|     |__|     |_______|| _| `._____|                                                                                                                     
                ");
            Console.WriteLine("Write simple geometry data into STEP AP214 (ISO-10303) file format.");
            Console.WriteLine();

            const string FILEPATH = @"C:\Users\me\Documents\local\file.step";

            StepFile stepFile = new StepFile(FILEPATH, "RootAssembly");

            stepFile.AddBox(
               name: "origin",
               center: new Vector3(0, 0, 0),
               dimension: new Vector3(10, 10, 10),
               rotation: new Vector3(0, 0, 0),
               color: Color.Red,
               parentGuid: 0
              );

            // let's build a workstation

            long workstationGuid = stepFile.AddGroup(
               name: "workstation",
               center: new Vector3(0, 0, 0),
               rotation: new Vector3(0, 0, 0),
               parentGuid: 0
              );

            long tableGuid = stepFile.AddGroup(
                name: "table",
                center: new Vector3(0, 0, 0),               
                rotation: new Vector3(0, 0, 0),
                parentGuid: workstationGuid
               );

            long stuffGuid = stepFile.AddGroup(
              name: "stuff",
              center: new Vector3(0, 0, 630),
              rotation: new Vector3(0, 0, 0),
              parentGuid: workstationGuid
             );

            long foundationGuid = stepFile.AddGroup(
                name: "foundation",
                center: new Vector3(0, 0, 0),
                rotation: new Vector3(0, 0, 0),
                parentGuid: tableGuid
               );

            stepFile.AddBox(
                name: "leg1",
                center: new Vector3(-750, -250, 300),
                dimension: new Vector3(100, 100, 600),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green,
                parentGuid: foundationGuid
               );

            stepFile.AddBox(
                name: "leg2",
                center: new Vector3(750, -250, 300),
                dimension: new Vector3(100, 100, 600),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green,
                parentGuid: foundationGuid
               );

            stepFile.AddBox(
                name: "leg3",
                center: new Vector3(-750, 250, 300),
                dimension: new Vector3(100, 100, 600),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green,
                parentGuid: foundationGuid
               );

            stepFile.AddBox(
                name: "leg4",
                center: new Vector3(750, 250, 300),
                dimension: new Vector3(100, 100, 600),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green,
                parentGuid: foundationGuid
               );

            stepFile.AddBox(
                name: "tabletop",
                center: new Vector3(0, 0, 615),
                dimension: new Vector3(1600, 600, 30),
                rotation: new Vector3(0, 0, 0),
                color: Color.Blue,
                parentGuid: foundationGuid
               );

            long drawerGuid = stepFile.AddGroup(
               name: "drawer",
               center: new Vector3(-550, 0, 450),
               rotation: new Vector3(0, 0, 0),
               parentGuid: tableGuid
              );

            stepFile.AddBox(
                name: "walls",
                center: new Vector3(0, 50, 0),
                dimension: new Vector3(300, 500, 300),
                rotation: new Vector3(0, 0, 0),
                color: Color.Yellow,
                parentGuid: drawerGuid
               );

            stepFile.AddBox(
                name: "knob1",
                center: new Vector3(0, -225, 50),
                dimension: new Vector3(100, 50, 35),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentGuid: drawerGuid
               );

            stepFile.AddBox(
                name: "knob2",
                center: new Vector3(0, -225, -60),
                dimension: new Vector3(100, 50, 35),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentGuid: drawerGuid
               );

            stepFile.AddBox(
                name: "book1",
                center: new Vector3(540, 35, 30),
                dimension: new Vector3(250, 360, 60),
                rotation: new Vector3(0, 0, -35),
                color: Color.White,
                parentGuid: stuffGuid
               );

            stepFile.AddBox(
                name: "book2",
                center: new Vector3(540, 35, 90),
                dimension: new Vector3(250, 360, 60),
                rotation: new Vector3(0, 0, -38),
                color: Color.Yellow,
                parentGuid: stuffGuid
               );

            long monitorGuid = stepFile.AddGroup(
               name: "monitor",
               center: new Vector3(0, 170, 0),
               rotation: new Vector3(0, 0, -4),
               parentGuid: stuffGuid
              );

            stepFile.AddBox(
                name: "socket",
                center: new Vector3(0, 0, 20),
                dimension: new Vector3(250, 150, 40),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentGuid: monitorGuid
               );

            stepFile.AddBox(
                name: "arm",
                center: new Vector3(0, 0, 190),
                dimension: new Vector3(100, 30, 300),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentGuid: monitorGuid
               );

            stepFile.AddBox(
                name: "display",
                center: new Vector3(0, -10, 340),
                dimension: new Vector3(600, 20, 338),
                rotation: new Vector3(0, 0, 0),
                color: Color.Black,
                parentGuid: monitorGuid
               );

            stepFile.AddBox(
                name: "keyboard",
                center: new Vector3(-110, -75, 0),
                dimension: new Vector3(560, 185, 40),
                rotation: new Vector3(0, 0, -6),
                color: Color.Red,
                parentGuid: stuffGuid
               );
            
            Console.WriteLine("Want to write the file to:\n" + FILEPATH + " ?");           
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
