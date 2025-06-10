using System;

public static class GloTestColor
{
    // Usage: GloTestColor.RunTests(testLog);
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestColorString(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloTestColor RunTests // Exception: ", false, ex.Message);
            return;
        }
    }


    // Test to create a basic color - convert it to JSON (text) and then back again, to ensure all the lists are correctly serialized and deserialized.
    public static void TestColorString(GloTestLog testLog)
    {
        // Test the serialization of colors into hex strings of different lengths
        {
            var color1 = new GloColorRGB(255, 0, 0);
            string strCol = GloColorIO.RBGtoHexStringShort(color1);
            testLog.AddComment($"GloColorIO RBGtoHexStringShort (3 char) 255,0,0: {strCol}");

            var color2 = new GloColorRGB(255, 34, 17, 34);
            string strCol2 = GloColorIO.RBGtoHexStringShort(color2);
            testLog.AddComment($"GloColorIO RBGtoHexStringShort (4 char) 255,34,17,34: {strCol2}");

            var color3 = new GloColorRGB(255, 32, 16, 255);
            string strCol3 = GloColorIO.RBGtoHexStringShort(color3);
            testLog.AddComment($"GloColorIO RBGtoHexStringShort (6 char) 255,32,16,255: {strCol3}");

            var color4 = new GloColorRGB(250, 33, 17, 134);
            string strCol4 = GloColorIO.RBGtoHexStringShort(color4);
            testLog.AddComment($"GloColorIO RBGtoHexStringShort (8 char) 250,33,17,134: {strCol4}");
        }

        // Test the deserialization of colors from hex strings
        {
            var color1 = GloColorIO.HexStringToRGB("F00");
            testLog.AddComment($"GloColorIO HexStringToRGB (3 char) F00: {GloColorIO.RBGtoHexStringShort(color1)}");

            var color2 = GloColorIO.HexStringToRGB("#F234");
            testLog.AddComment($"GloColorIO HexStringToRGB (4 char) #F234: {GloColorIO.RBGtoHexStringShort(color2)}");

            var color3 = GloColorIO.HexStringToRGB("FF2356");
            testLog.AddComment($"GloColorIO HexStringToRGB (6 char) FF2356: {GloColorIO.RBGtoHexStringShort(color3)}");

            var color4 = GloColorIO.HexStringToRGB("FA211086");
            testLog.AddComment($"GloColorIO HexStringToRGB (8 char) FA211086: {GloColorIO.RBGtoHexStringShort(color4)}");
        }
    }
}


