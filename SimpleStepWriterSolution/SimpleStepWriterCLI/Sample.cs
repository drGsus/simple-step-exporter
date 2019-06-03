using SimpleStepWriter;
using SimpleStepWriter.Helper;

/// <summary>
/// Sample class that is not used for anything except demonstration purposes.
/// </summary>
class Sample
{
    public void MySampelMethod()
    {
        // This will create the following CAD hierarchy:
        //-------------------------------------------------
        //
        //   + MyRootAssembly       [assembly root]
        //   |
        //   +---+- sample-group    [empty part]    
        //       |  
        //       +- sample-box      [part with box geometry]    
        //

        // example file path where the STEP file will be written
        const string FILEPATH = @"C:\Users\me\Documents\file.step";

        // create new StepFile instance
        StepFile stepFile = new StepFile(FILEPATH, "MyRootAssembly");

        // add a Group to StepFile and get the groups id
        int groupId = stepFile.AddGroup(
            name: "sample-group",                       // name of the part
            position: new Vector3(0, 0, 0),             // Group position relative to parent position
            rotation: new Vector3(90, 0 ,0),            // Group rotation relative to parent rotation
            parentId: stepFile.ASSEMBLY_ROOT_ID         // id of parent object (ASSEMBLY_ROOT_ID is 0)
        );

        // add a Box to StepFile
        stepFile.AddBox(
            name: "sample-box",                         // name of the part
            position: new Vector3(0, 0, 0),               // Box position (based on the center of the box) relative to parent position 
            dimension: new Vector3(10, 10, 10),         // dimension of the box (length, width, height)
            rotation: new Vector3(0, 0, 0),             // Box rotation (rotated around box center) relative to parent rotation
            color: Color.Red,                           // color of the Box
            parentId: groupId                           // id of the parent object
        );

        // write the StepFile to file system
        bool result = stepFile.WriteFile();

    }

}
