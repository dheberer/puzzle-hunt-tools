using System;
using System.Collections;
using System.IO;

namespace WordSearch
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    /// 
    public struct advance
    {
        public int row;
        public int column;
    };

    public struct location
    {
        public int row;
        public int column;
        public advance direction;
        public int length;
    };

    class WordSearchApp
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main(string[] args)
        {
            ArrayList passedIn = new ArrayList();
            SortedList wordList = new SortedList();
            ArrayList FoundWords = new ArrayList();

            bool readFile = false;
            bool wordListSupplied = false;
            bool outputUnused = false;
            int minLength = 0;
            string replaceWith = " ";

            advance[] incrementBy = new advance[8];

            int tempIndex = 0;
            for (int x = -1; x<2; x++)
            {
                for (int y = -1; y<2; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    incrementBy[tempIndex].row = x;
                    incrementBy[tempIndex++].column = y;
                }        
            }

            //string fakeArgs = "-f test.txt -l 5 -w wordlist.txt -o"; 
            //char[] chars = new char[1];
            //chars[0] = ' ';
            //args = fakeArgs.Split(chars);
            
            foreach (string s in args)
            {
                if (s == "-s")
                {
                    replaceWith = "*";
                }
                
                if (s == "-w")
                {
                    wordListSupplied = true;
                    continue;
                }

                if (s == "-f")
                {
                    readFile = true;
                    continue;
                }
                if (s == "-?")
                {
                    Console.WriteLine("Treats text file as a word search puzzle.  wordsearch looks for words in\nall directions, and will return unused letters and words found.");
                    Console.WriteLine("Usage:wordsearch -f file\n");
                    Console.WriteLine("Optional parameters:");
                    Console.WriteLine("-w file    use file as a wordlist, only looking for these words");    
                    Console.WriteLine("-l num     only find words num or longer");
                    Console.WriteLine("-o         replace all used letters with ' ' (default)");
                    Console.WriteLine("-s         replace all used letters with '*', use with -o");
                    Console.WriteLine("-? this help.");
                    return;
                }
                if (s == "-l")
                {
                    minLength =-1;
                    continue;
                }

                if (s == "-o")
                {
                    outputUnused = true;
                    continue;
                }

                if (minLength < 0)
                {
                    minLength = Convert.ToInt32(s);
                    continue;
                }

                if (readFile)
                {
                    // We treat the string following -f as a filename.
                    StreamReader sr = null;
                    string line;
                    try
                    {
                        sr = new StreamReader(s);
                    }
                    catch(FileNotFoundException)
                    {
                        Console.WriteLine("The file " + s + " was not found.");
                    }
                    while ((line = sr.ReadLine()) != null) 
                    {   
                        passedIn.Add(line.ToUpper());
                    }
                    // Done reading file, close up and check for more command line parameters.
                    sr.Close();
                    readFile = false;
                    continue;
                }

                if (wordListSupplied)
                {
                    // We treat the string following -w as a filename.
                    StreamReader sr = null;
                    string line;
                    try
                    {
                        sr = new StreamReader(s);
                    }
                    catch(FileNotFoundException)
                    {
                        Console.WriteLine("The file " + s + " was not found.");
                    }
                    ulong key = 0;
                    while ((line = sr.ReadLine()) != null) 
                    {
                        wordList.Add(key++, line.ToUpper());
                    }
                    // Done reading file, close up and check for more command line parameters.
                    sr.Close();
                    wordListSupplied = false;
                    continue;                    
                }

            }
            
            // done getting info, time to look for stuff.
            
            //load the words.
            if (wordList.Count == 0)
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader("words.lst");
                }
                catch(FileNotFoundException)
                {
                    Console.WriteLine("Couldn't find the word list.  it's called words.lst and should be in the same directory as the application.");
                    return;
                }
                string line;
                double key =0;
                while ((line = sr.ReadLine()) != null) 
                {
                    wordList.Add(key++, line.ToUpper());
                }
            }
            Console.WriteLine("Loaded {0} entries into the word list", wordList.Count);
            // got list, let's look for stuff.

            int width = ((string)passedIn[0]).Length;
            int height = passedIn.Count;

            foreach (advance currentA in incrementBy)
            {
                //Let's look at each character and go from there.
                // while we haven't left the grid of letters, we should search.
                int currentRow = 0;
                int currentColumn = 0;
                while (currentRow < height)
                {
                    // Get the full string, we're greedy.
                    int tempRow = currentRow;
                    int tempColumn = currentColumn;
                    string workingString = "";
                    try
                    {
                        while (tempRow >=0 && tempRow <height && tempColumn >= 0 && tempColumn < width)
                        {
                            workingString = workingString + ((string)passedIn[tempRow])[tempColumn];
                            tempRow = tempRow + currentA.row;
                            tempColumn = tempColumn + currentA.column;
                        }
                    }
                    catch(IndexOutOfRangeException)
                    {
                        // the first line is longer than this one.
                        // we'll take what we can get.
                    }
                    while (workingString.Length > 0)
                    {
                        if (wordList.ContainsValue(workingString))
                        {
                            break;
                        }
                        workingString = workingString.Remove(workingString.Length-1,1);
                    }
                    if (workingString.Length > 0 && workingString.Length >= minLength)
                    {
                        Console.WriteLine("Found {0} at Row:{1} Column:{2} Direction:{3},{4}", workingString,currentRow,currentColumn,currentA.row,currentA.column);
                        location tempLoc = new location();
                        tempLoc.column = currentColumn;
                        tempLoc.row = currentRow;
                        tempLoc.direction = currentA;
                        tempLoc.length = workingString.Length;
                        FoundWords.Add(tempLoc);
                    }
                    currentColumn++;
                    if (currentColumn == width)
                    {
                        currentColumn = 0;
                        currentRow++;
                    }
                }
            }
            // ok done looking, now we need to clear out all letters used (if specified)

            if (outputUnused == true)
            {
                foreach(location l in FoundWords)
                {
                    int tempRow = l.row;
                    int tempColumn = l.column;
                    advance a = l.direction;
                    int length = l.length;
                    while (length > 0)
                    {
                        string tempString = (string)(passedIn[tempRow]);
                        tempString = tempString.Remove(tempColumn,1);
                        tempString = tempString.Insert(tempColumn,replaceWith);
                        
                        passedIn.RemoveAt(tempRow);
                        passedIn.Insert(tempRow,tempString);
                        tempRow += a.row;
                        tempColumn += a.column;
                        length--;
                    }
                }

                Console.WriteLine("-------------------------------------------------");
                foreach(string s in passedIn)
                {
                    Console.WriteLine(s);
                }
            }
            
        }

    }
}

