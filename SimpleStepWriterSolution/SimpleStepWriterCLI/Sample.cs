using SimpleStepWriter;
using SimpleStepWriter.Helper;

/// <summary>
/// Sample class that is not used for anything except demonstration purposes.
/// </summary>
class Sample
{
    public void MySampelMethod()
    {
        // example file path where the STEP file will be written
        const string FILEPATH = @"C:\Users\me\Documents\file.step";

        // create new StepFile instance
        StepFile stepFile = new StepFile(FILEPATH, "MyRootAssembly");

        // add a Box to StepFile
        stepFile.AddBox(
            name: "origin_10mm_dimensions",             // name of the part
            center: new Vector3(0, 0, 0),               // box position in world space (based on the center of the box)
            dimension: new Vector3(10, 10, 10),         // dimension of the box (length, width, height)
            rotation: new Vector3(0, 0, 0),             // box rotation (rotated around box center)
            color: Color.Red                            // color of the box
        );

        // write the StepFile to file system
        bool result = stepFile.WriteFile();
    }

}

