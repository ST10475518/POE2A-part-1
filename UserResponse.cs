using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace ai_chat
{// Start of namespace
    public class UserResponse
    { // Start of class
        public UserResponse()
        { // Start of Constructor
            Console.Title = "Cybersecurity Awareness Bot";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("====================================");
            Console.WriteLine("   Cybesecurity Awareness Bot");
            Console.WriteLine("====================================");
            Console.ResetColor();
          
            Console.WriteLine("what will you like to talk about ? \n password \n phishing \n virus \n 2fa \n encryption ");
            Console.WriteLine("Type 'exit' to quit. \n");

            while (true)
            {
                Console.ForegroundColor= ConsoleColor.Green;
                Console.Write("You: ");
                Console.ResetColor();
                string userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    ShowErrorResponse("You didn't type anything . please enter message.");
                    continue;

                }
                 
                if (userInput.Length > 200)
                {
                    ShowErrorResponse("Your message is too long. please keep it under 200 characters.");
                    continue;
                }

                if (Regex.IsMatch(userInput,@"[\!@#$%^&**():?><]"))
                {
                    ShowErrorResponse("Your message contains invalid Characters. please use normal text.");
                    continue ;
                }

                if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                { 
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Bot: stay safe online! Goodbye.");
                    Console.ResetColor();
                    break;
                }

                if (!IsSupportedQuery(userInput))
                {
                    ShowErrorResponse("I Didn't quite understand that. Could you rephrase?");
                    continue;

                }
                RespondToUser(userInput);

            }
         
        } // End of Constructor

        private void ShowErrorResponse(string customMessage = null)
        {
            Console.ForegroundColor=ConsoleColor.Yellow;
            if (customMessage != null)
            
                Console.WriteLine($"Bot: {customMessage}");
            
            else
            
                Console.WriteLine("BoT: I Didn't quite understand that. Could you Rephrase?");
            
            Console.ResetColor ();

        }

        private bool IsSupportedQuery(string input) 
        {
            string lower = input.ToLower();
            string[] keywords = { "password", "phishin", "virus", "malware", "hack", "cyber", "2fa", "encryption" };
            foreach (string keyword in keywords)
            {
                if (lower.Contains(keyword))
                    return true;
            }
            return false;
        }

        private void RespondToUser(string input)
        {
            string lower = input.ToLower ();
            Console.ForegroundColor = ConsoleColor.Blue;

            if (lower.Contains("password"))
                Console.WriteLine("Bot: use strong, unique passwords and enable two factor authentication (2FA).");
            else if (lower.Contains("phishing"))
                Console.WriteLine("Bot: never click suspicious liks. Always verify the send's email address.");
            else if (lower.Contains("virus"))
                Console.WriteLine("Bot: keep your antivirus updated and avoid downloading from untrusted sites.");
            else if (lower.Contains("2fa"))
                Console.WriteLine("Bot: 2FA add an extra layer of security. Use an authenticator app, not SMS.");
            else if (lower.Contains("encryption"))
                Console.WriteLine("Bot: Encypt sensitive files and use HTTPS websites for secure communication.");
            else
                Console.WriteLine("Bot:that's a good cybersecurity question. let me think...");

            Console.ResetColor ();
        }

        internal static void Respond(string input)
        {
            throw new NotImplementedException();
        }
    }//end of class
}// end of namespace)