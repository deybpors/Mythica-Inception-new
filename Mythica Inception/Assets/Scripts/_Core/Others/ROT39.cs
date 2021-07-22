using System;

namespace _Core.Others
{
public class ROT39
{
    private const long LOWER_LIMIT = 48; //ascii for 0
    private const long UPPER_LIMIT = 125; //ascii for {
    private const long CHARMAP = 39;
    public static string DoROT39(string sData)
    {
        string tempROT39 = null;
        try
        {
            // ROT39 (a variation of the ROT13 function)
            string sReturn = null;
            long nCode = 0;
            long nData = 0;
            //initialize the byte array to the size of the string passed.
            byte[] bData =new byte[sData.Length];
            //reverse the string
            char[] tmpsData=sData.ToCharArray();
            Array.Reverse(tmpsData);
            sData="";
            foreach(char c in tmpsData)
                sData+=c.ToString();
            //cast string into the byte array
            bData = System.Text.Encoding.UTF8.GetBytes(sData);
            long tempFor1 = bData.GetUpperBound(0);
            for (nData = 0; nData <= tempFor1; nData++)
            {
                //with the ASCII value of the character
                nCode = bData[nData];
                //assure the ASCII value is between
                //the lower and upper limits
                if ((nCode >= LOWER_LIMIT) & (nCode <= UPPER_LIMIT))
                {
                    //shift the ASCII value by the CHARMAP const value
                    nCode = nCode + CHARMAP;
                    //perform a check against the upper
                    //limit. If the new value exceeds the
                    //upper limit, rotate the value to offset
                    //from the beginning of the character set.
                    if (nCode > UPPER_LIMIT)
                    {
                        nCode = nCode - UPPER_LIMIT + LOWER_LIMIT - 1;
                    }
                }
                //reassign the new shifted value to
                //the current byte
                bData[nData] = (byte)nCode;
            }
            //convert the byte array back
            //to a string and exit
            sReturn = System.Text.Encoding.UTF8.GetString(bData);
            //assign the return string
            tempROT39 = sReturn;
        }

        catch
        {
            // ignored
        }

        return tempROT39;
        }
    }
}