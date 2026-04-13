using System;
using System.Drawing;
using System.Media;
using System.Security.Policy;

namespace ai_chat
{//Start of namespace
    public class voice_logo
    {// Start of Class 

        // Auto get the path directory
        private string full_path = AppDomain.CurrentDomain.BaseDirectory;

        public voice_logo()
        {//Start of Constructor

            //calling sound method
            greetings();

            //call the logo method
            asci();

            DisplayLogo();

            Console.ReadLine();
           

        }//End of Constructor

        //method to play the sound 
        private void greetings() {// Start of greetings method

            //check if the path is auto collected
            Console.WriteLine(full_path);

            //then replce the \bin\Debug
            string correct_path = full_path.Replace(@"\bin\Debug\" ,@"\greet.wav");
            //check if audio found
            Console.WriteLine(correct_path);

            //use the sound player class to play the audio
            //creating an instance for the soundPlayer class
            //with an object name greet
            SoundPlayer greet = new SoundPlayer( correct_path );
            //then play the sound using the play method
            greet.Play();

           // Console.ReadLine();
        
        
        
        }//End of greetings method

        //method to turn logo to ascii
        private void asci()
        {
            //path of the logo [ where the logo is ]
            string path = full_path.Replace(@"\bin\Debug\" ,@"\logo.png");

            Bitmap image = new Bitmap(path);

            // Resize for better console fit
            int width = 150;
            int height = 70; //(image.Height * width) / image.Width;
            Bitmap resized = new Bitmap(image, new Size(width, height));

            // Default color , you can set yours before this line
            string asciiChars = "@#S%?*+;:,. ";

            //start by the height
            for (int y = 0; y < resized.Height; y++)
            {
                //then width
                for (int x = 0; x < resized.Width; x++)
                {
                    //color the pixel on x and y
                    Color pixel = resized.GetPixel(x, y);

                    // Convert to grayscale
                    int gray = (pixel.R + pixel.G + pixel.B) / 3;

                    // Map grayscale to ASCII
                    int index = (gray * (asciiChars.Length - 1)) / 255;

                    Console.Write(asciiChars[index]);
                }
                Console.WriteLine();
            }
        }

        private void DisplayLogo()
        {
         Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(@"

        _____  _           _          ____ _           _
       / ____|| |__   ____| |_       / ___| |__   ____| |_
       \____ \|  _ \ / _  | __|     | |   |  _ \ / _  | __|
        ____) | | | | (_| | |_      | |___| | | | (_| | |_
       |_____/|_| |_|\__,_|\__|      \____|_| |_|\__,_|\__|

");
     
            Console.ResetColor();
            Console.WriteLine("Welcome to SmartBot!\n What is your name?");
            

        }
        public void PlaySampleSound()
        {

            //we play a sample sound in this method

            try
            {
                Console.Beep();
                Console.WriteLine();
                Console.WriteLine("Welcome to my ChatBot(Beep)");
            }
            catch
            {
                Console.WriteLine("WECOME TO chatBot");
            }
        }


    }//End of Class
}//End of Class