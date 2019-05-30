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
                        
            // let's build demo hierarchy
            long assemblyGuid = stepFile.AddGroup(
                name: "assembly",
                center: new Vector3(0, 0, 0),               
                rotation: new Vector3(0, 0, 0),
                parentGuid: 0
               );

            stepFile.AddBox(
                name: "box1",
                center: new Vector3(600, 0, 700),
                dimension: new Vector3(100, 600, 1400),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green,
                parentGuid: assemblyGuid
               );

            stepFile.AddBox(
               name: "box2",
               center: new Vector3(600, 0, 700),
               dimension: new Vector3(100, 600, 1400),
               rotation: new Vector3(0, 0, 0),
               color: Color.Green,
               parentGuid: assemblyGuid
              );

            stepFile.AddBox(
              name: "box3",
              center: new Vector3(600, 0, 700),
              dimension: new Vector3(100, 600, 1400),
              rotation: new Vector3(0, 0, 0),
              color: Color.Green,
              parentGuid: 0
             );

            /*
            // debug origin cube
            stepFile.AddBox(
                name: "origin_box_10mm_10mm_10mm_red",
                center: new Vector3(0, 0, 0),
                dimension: new Vector3(10, 10, 10),
                rotation: new Vector3(0, 0, 0),
                color: Color.Red
            );

            // let's build a shelf
            
            // left shelf stand
            stepFile.AddBox(
             name: "shelf_stand_left",
             center: new Vector3(-600, 0, 700),
             dimension: new Vector3(100, 600, 1400),
             rotation: new Vector3(0, 0, 0),
             color: Color.Green
            );
            // right shelf stand
            stepFile.AddBox(
                name: "shelf_stand_right",
                center: new Vector3(600, 0, 700),
                dimension: new Vector3(100, 600, 1400),
                rotation: new Vector3(0, 0, 0),
                color: Color.Green
            );
            // top shelf level
            stepFile.AddBox(
                name: "shelf_level_top",
                center: new Vector3(0, 0, 1000),
                dimension: new Vector3(1100, 500, 50),
                rotation: new Vector3(20, 0, 0),
                color: Color.Blue
            );
            // bottom shelf level
            stepFile.AddBox(
                name: "shelf_level_bottom",
                center: new Vector3(0, 0, 500),
                dimension: new Vector3(1100, 500, 50),
                rotation: new Vector3(20, 0, 0),
                color: Color.Yellow
            );
            */

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
