using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Synthesis;        // ADDED for text-to-speech
using System.Speech.Recognition;      // ADDED for voice recognition

namespace list_view_chats
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {//start of class

        //creating an instance for the class Array
        ArrayList reply = new ArrayList();
        ArrayList ignore = new ArrayList();

        //username variable
        string username = string.Empty;

        ///////////// ADDED NEW VARIABLES BELOW /////////////
        // For random responses (multiple answers per keyword)
        private Dictionary<string, ArrayList> keywordMultipleResponses = new Dictionary<string, ArrayList>();
        private Random randomSelector = new Random();

        // For sentiment detection
        private string userSentiment = "neutral";

        // For voice
        private SpeechSynthesizer speechSynthesizer;
        private SpeechRecognitionEngine speechRecognizer;
        private bool voiceEnabled = false;
        ///////////// END OF ADDED VARIABLES /////////////

        public MainWindow()
        {
            InitializeComponent();



            new respond(reply, ignore) { };

            ///////////// ADDED NEW LINES BELOW /////////////
            // Load cybersecurity responses into your reply ArrayList
            LoadCybersecurityResponses();

            // Display ASCII art in chat
            ShowAsciiArt();

            // Initialize voice features
            InitializeVoiceFeatures();
            ///////////// END OF ADDED LINES /////////////
        }

        private void send(object sender, RoutedEventArgs e)
        {//start of send method

            //get the question from the design
            //user input
            string questions = user_question.Text.ToString();

            //show what the user tyeped
            error_method(username, questions);

            //if statement to check if user entered a questio or not
            if (questions == "")
            {
                //call the error method
                error_method("ChatBot", "please enter a question");
            }
            else
            {//start of else


                //temp varaibles and arrays
                string[] words = questions.Split(' ');

                bool found = false;
                string message = string.Empty;

                Random indexer = new Random();

                ArrayList per_word = new ArrayList();
                ArrayList answers_found = new ArrayList();

                //alterate per word from the words array
                foreach (string word in words)
                {//start of the main foreach


                    //check if the word is allowed or not
                    if (!ignore.Contains(word.ToLower()))
                    {//start of check word if

                        // MessageBox.Show( word +" allowed" );
                        per_word.Clear();


                        // check if the word interested id found
                        if (word.ToLower().Contains("interested"))
                        {//start of interested if

                            //getb what the user is interested in only
                            string store_interests = string.Empty;
                            bool found_intersts = false;
                            //loop each word
                            foreach (string interests in words)
                            {
                                if (!ignore.Contains(interests) && interests != "interested")
                                {
                                    //then append what they are interested in
                                    found_intersts |= true;
                                    store_interests += interests + ", ";

                                }

                            }

                            //store the interests in a text file
                            if (found_intersts)
                            {//start
                                //filename
                                string filename = "interested_topic.txt";
                                File.AppendAllText(filename, username + " " + store_interests);

                                // ADDED - Store interest for memory recall
                                StoreUserInterest(store_interests.TrimEnd(',', ' '), username);

                                // ADDED - Sentiment adjustment for response
                                string positiveResponse = "Great!  I will remember that you are interested in " + store_interests +
                                                          ". I can share more cybersecurity tips on these topics! ";
                                answers_found.Add(positiveResponse);
                                ///////////// END OF ADDED /////////////
                            }//end
                            else
                            {
                                // ADDED - Better error message with sentiment
                                string errorMsg = "Sorry, please make sure the topics (" + store_interests + ") are cybersecurity related. Try: passwords, phishing, malware, etc.";
                                answers_found.Add(errorMsg);
                                ///////////// END OF ADDED /////////////
                            }



                        }//end of interested if
                        //foreach to search for the answer of the word allowed
                        foreach (string answer in reply)
                        {//start of answer loop

                            //check and store
                            if (answer.Contains(word.ToLower()))
                            {//start of check answer if

                                found = true;

                                //store all answers for the word
                                per_word.Add(answer);

                            }//end of check answer if

                        }//end of answer loop

                        //then check if found is true and store
                        //per random
                        if (found)
                        {//start of found if

                            //get the random indexer
                            int indexing = indexer.Next(0, per_word.Count);

                            //store one answer per word now
                            answers_found.Add(per_word[indexing]);



                        }//end of found if


                    }//end of check word if



                }//end of the main foreach


                //check and show the user the answers
                if (found)
                {//start of found if true

                    //get all of answers and show to the user
                    foreach (string per_answer in answers_found)
                    {//start of show answer loop

                        //append all message
                        message += per_answer + "\n";

                    }//end of show answer loop

                    //add the message or answers to the list view
                    //chats.Items.Add( message  );

                    ///////////// ADDED - Detect sentiment and adjust response /////////////
                    string sentiment = DetectSentiment(questions);
                    userSentiment = sentiment;
                    message = AdjustResponseForSentiment(message, sentiment);

                    // ADDED - Speak the response (voice output)
                    if (!string.IsNullOrEmpty(message) && message.Length < 200) // Don't speak very long messages
                    {
                        SpeakText(message);
                    }
                    ///////////// END OF ADDED /////////////

                    error_method("ChatBot", message);

                    //auto scroll
                    chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);


                }//end of found if true





            }//end of else


        }//end of send method


        //error method
        private void error_method(string name, string message)
        {//star of error method

            //call the chats which is a listview
            chats.Items.Add(
                new TextBlock
                {
                    Inlines = {
                     new Run{
                     Text=name + " : ",
                     Foreground =Brushes.Blue

                     }   ,
                     new Run {
                     Text="" +message ,
                     Foreground =Brushes.Red

                     }

                    }

                }

                );

        }//end of error method

        private void submit_name(object sender, RoutedEventArgs e)
        {//start of

            //temp variables
            string filename = "user_names.txt";

            //check if the filename exists or not , then auto create
            if (!File.Exists(filename))
            {
                //auto create the file using AppendAllText() function
                File.AppendAllText(filename, "auto_create\n");

            }//end

            //temp variables
            string name = user_name.Text.ToString();
            bool found = check_name(name);

            //check if the user is found or not and write the name in a text file
            if (!found)
            {//start of if
                //write the name in a text file
                File.AppendAllText(filename, name + "\n");

                //then welcome the user
                error_method("ChatBot", "hey" + " " + name + " " + "welcome to ai cybersecurity ");

            }//end of if
            else
            {//start of else
                //welcome the user back
                error_method("ChatBot", "hey" + " " + name + " " + "welcome back how can i help you today ");

            }//end of else
            //hide username grid and set the chats grid vissible
            name_grid.Visibility = Visibility.Hidden;
            Chat_Grid.Visibility = Visibility.Visible;

            //assign the username to the global variable
            username = name;

            ///////////// ADDED NEW CODE BELOW /////////////
            // Check if user had previous interests
            string previousInterest = RecallUserInterest(username);
            if (!string.IsNullOrEmpty(previousInterest))
            {
                error_method(" CyberBot", $"I remember you were interested in {previousInterest} before! Would you like more tips on this topic?");
            }
            ///////////// END OF ADDED /////////////

        }//end of

        //method to check name of the user
        private Boolean check_name(string name)
        {//start
            //TEMP VARIABLE
            string filename = "user_names.txt";

            bool found_name = false;

            //store or get all the names in the text file and store in a 1D array
            string[] names = File.ReadAllLines(filename);

            //foreach to search the namae of the user
            foreach (string name_found in names)
            {//start of loop

                //if statement  to check for the username
                if (name_found.ToLower() == name.ToLower())
                {//start of if
                    //found_name set to true
                    found_name = true;
                }//end of if

            }//end of loop

            //return the status of found or not [true or false ]
            return found_name;


        }//end

        ///////////// ALL ADDED METHODS BELOW (Your original code above remains unchanged) /////////////

        // ADDED METHOD - Loads cybersecurity responses into your existing 'reply' ArrayList
        private void LoadCybersecurityResponses()
        {
            // Add cybersecurity responses to your existing 'reply' ArrayList
            // This works WITH your existing keyword matching system

            reply.Add("password: Use strong passwords with 12+ characters! Combine letters, numbers, and symbols.");
            reply.Add("password: Never reuse passwords across different accounts.");
            reply.Add("password: Enable two-factor authentication for extra security.");
            reply.Add("password: Consider using a password manager like Bitwarden.");

            reply.Add("phishing: Don't click suspicious links! Always check the sender's email address.");
            reply.Add("phishing: Legitimate companies never ask for passwords via email.");
            reply.Add("phishing: Look for spelling errors - that's a common phishing sign.");
            reply.Add("phishing: When in doubt, type the website URL directly into your browser.");

            reply.Add("malware: Keep your antivirus software updated at all times.");
            reply.Add("malware: Avoid downloading files from unknown sources.");
            reply.Add("malware: Run regular system scans to detect malware early.");
            reply.Add("virus: Run regular antivirus scans to detect viruses early!");
            reply.Add("virus: Keep your operating system and software updated.");
            reply.Add("vpn: VPNs encrypt your connection on public Wi-Fi networks.");
            reply.Add("vpn: Always use VPN when accessing sensitive information remotely.");
            reply.Add("backup: Follow the 3-2-1 backup rule for important files.");
            reply.Add("backup: Test your backups regularly to ensure they work.");
            reply.Add("2fa: Two-factor authentication adds a critical security layer!");
            reply.Add("2fa: Use authenticator apps instead of SMS when possible.");
            reply.Add("ransomware: Never pay the ransom - it encourages more attacks.");
            reply.Add("ransomware: Regular offline backups protect against ransomware.");
            reply.Add("hello: Hello! How can I help you with cybersecurity today?");
            reply.Add("hello: Hi there! Ready to learn about online safety?");
            reply.Add("help: I can help with: Passwords, Phishing, Malware, VPNs, Backups, 2FA, and Ransomware!");

            // Also setup multiple responses for random selection
            SetupMultipleResponses();
        }

        // ADDED METHOD - Sets up multiple responses for random selection
        private void SetupMultipleResponses()
        {
            // This works WITH your existing reply ArrayList but adds random selection capability
            foreach (string item in reply)
            {
                string itemStr = item.ToString();
                if (itemStr.Contains(":"))
                {
                    string keyword = itemStr.Substring(0, itemStr.IndexOf(":")).ToLower();
                    if (!keywordMultipleResponses.ContainsKey(keyword))
                    {
                        keywordMultipleResponses[keyword] = new ArrayList();
                    }
                    keywordMultipleResponses[keyword].Add(itemStr);
                }
            }
        }

        // ADDED METHOD - Gets random response for a keyword
        private string GetRandomResponse(string keyword)
        {
            if (keywordMultipleResponses.ContainsKey(keyword.ToLower()))
            {
                ArrayList responses = keywordMultipleResponses[keyword.ToLower()];
                int randomIndex = randomSelector.Next(0, responses.Count);
                return responses[randomIndex].ToString();
            }
            return string.Empty;
        }

        // ADDED METHOD - Detects user sentiment from their message
        private string DetectSentiment(string message)
        {
            string lowerMessage = message.ToLower();

            // Check for worried/anxious sentiment
            string[] worriedWords = { "worried", "scared", "afraid", "nervous", "anxious", "overwhelmed", "unsafe", "hacked" };
            foreach (string word in worriedWords)
            {
                if (lowerMessage.Contains(word))
                    return "worried";
            }

            // Check for frustrated sentiment
            string[] frustratedWords = { "frustrated", "annoying", "difficult", "hard", "confusing", "stupid", "hate" };
            foreach (string word in frustratedWords)
            {
                if (lowerMessage.Contains(word))
                    return "frustrated";
            }

            // Check for curious/positive sentiment
            string[] curiousWords = { "curious", "interested", "tell me", "how", "why", "what is", "learn", "explain" };
            foreach (string word in curiousWords)
            {
                if (lowerMessage.Contains(word))
                    return "curious";
            }

            return "neutral";
        }

        // ADDED METHOD - Adjusts response based on sentiment
        private string AdjustResponseForSentiment(string originalResponse, string sentiment)
        {
            if (string.IsNullOrEmpty(originalResponse))
                return originalResponse;

            switch (sentiment)
            {
                case "worried":
                    return " Don't worry! " + originalResponse + " Remember, taking small steps makes a big difference in cybersecurity! 💪";
                case "frustrated":
                    return " I understand this can be frustrating. " + originalResponse + " You've got this! Let's make it simple. 🌟";
                case "curious":
                    return " Great question! " + originalResponse + " Want to learn more about this topic?";
                default:
                    return originalResponse;
            }
        }

        // ADDED METHOD - Stores user interests in memory (works with your existing interested detection)
        private void StoreUserInterest(string topic, string username)
        {
            string filename = "user_interests.txt";
            string interestRecord = $"{username}|{topic}|{DateTime.Now}{Environment.NewLine}";
            File.AppendAllText(filename, interestRecord);
        }

        // ADDED METHOD - Recalls what the user was interested in before
        private string RecallUserInterest(string username)
        {
            string filename = "user_interests.txt";
            if (File.Exists(filename))
            {
                string[] lines = File.ReadAllLines(filename);
                for (int i = lines.Length - 1; i >= 0; i--) // Read from end to get latest
                {
                    if (lines[i].StartsWith(username + "|"))
                    {
                        string[] parts = lines[i].Split('|');
                        if (parts.Length >= 2)
                            return parts[1];
                    }
                }
            }
            return string.Empty;
        }

        // ADDED METHOD - Displays ASCII art in the chat
        private void ShowAsciiArt()
        {
            string asciiArt = @"
╔══════════════════════════════════════════════════════════════╗
║                       CYBERSECURITY CHATBOT                  ║
║                  Your Personal Security Assistant            ║
╚══════════════════════════════════════════════════════════════╝
    ╭━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━╮
    ┃     'Stay Safe Online - Knowledge is Your Best Defense'   ┃
    ╰━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━╯
    
 Available Topics: Passwords, Phishing, Malware, VPNs, Backups, 2FA, Ransomware
 Tip: Say 'I am interested in [topic]' and I'll remember your preference!";

            error_method(" CyberBot", asciiArt);
        }


        // ADDED METHOD - Initialize voice features
        // ADDED METHOD - Initialize voice features (SAFER VERSION)
        private void InitializeVoiceFeatures()
        {
            try
            {
                // Initialize text-to-speech only (more reliable)
                speechSynthesizer = new SpeechSynthesizer();
                speechSynthesizer.SetOutputToDefaultAudioDevice();

                // Try to initialize speech recognition, but don't crash if it fails
                try
                {
                    // Check if there are any recognizers installed
                    if (System.Speech.Recognition.SpeechRecognitionEngine.InstalledRecognizers().Count > 0)
                    {
                        speechRecognizer = new SpeechRecognitionEngine();
                        speechRecognizer.SetInputToDefaultAudioDevice();
                        DictationGrammar dictationGrammar = new DictationGrammar();
                        speechRecognizer.LoadGrammar(dictationGrammar);
                        speechRecognizer.SpeechRecognized += SpeechRecognizer_SpeechRecognized;
                        speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                        voiceEnabled = true;
                        error_method(" System", "Voice features initialized! Click the microphone button and speak.");
                    }
                    else
                    {
                        voiceEnabled = false;
                        error_method(" System", "No speech recognizer found. Voice input disabled. Text input works fine!");
                    }
                }
                catch (Exception ex)
                {
                    voiceEnabled = false;
                    error_method(" System", "Voice recognition unavailable. Text input only. Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                voiceEnabled = false;
                error_method(" System", "Text-to-speech unavailable: " + ex.Message);
            }
        }


        // ADDED EVENT HANDLER for voice recognition
        private void SpeechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // This runs when voice is recognized
            // MUST use Dispatcher to access UI elements
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    string recognizedText = e.Result.Text;
                    user_question.Text = recognizedText;
                    error_method(" Voice Input", "I heard: " + recognizedText);
                    // Auto-send after voice input
                    send(null, null);
                }
                catch (Exception ex)
                {
                    error_method(" Error", "Voice error: " + ex.Message);
                }
            }));
        }

        // ADDED METHOD - Start voice recognition
        private void StartVoiceRecognition()
        {
            if (voiceEnabled && speechRecognizer != null)
            {
                try
                {
                    speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                    error_method("🎤 System", "Listening... Please speak clearly.");
                }
                catch (Exception ex)
                {
                    error_method(" System", "Error starting voice: " + ex.Message);
                }
            }
            else
            {
                error_method(" System", "Voice features not available. Please check microphone and System.Speech reference.");
            }
        }

        // ADDED METHOD - Text to speech
        private void SpeakText(string text)
        {
            if (voiceEnabled && speechSynthesizer != null)
            {
                try
                {
                    // Remove any ASCII art or special characters before speaking
                    string cleanText = text;
                    speechSynthesizer.SpeakAsync(cleanText);
                }
                catch (Exception ex)
                {
                    // Silent fail - don't spam errors
                }
            }
        }

        // ADDED METHOD - Voice input button handler
        // ADDED METHOD - Voice input button handler (SAFER VERSION)
        private void voice_button_click(object sender, RoutedEventArgs e)
        {
            if (voiceEnabled && speechRecognizer != null)
            {
                try
                {
                    error_method(" System", "Listening... Please speak clearly.");
                }
                catch (Exception ex)
                {
                    error_method(" System", "Voice error: " + ex.Message);
                }
            }
            else
            {
                error_method(" System", "Voice input not available. Please type your question instead. ");
                // Optional: Show Windows Speech Recognition tip
                MessageBox.Show("To use voice input:\n1. Press Windows key + H\n2. Click the microphone\n3. Speak, then click Send",
                                "Voice Input Help", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ADDED METHOD - Recall memory button handler
        private void recall_memory_button_click(object sender, RoutedEventArgs e)
        {
            string previousInterest = RecallUserInterest(username);
            if (!string.IsNullOrEmpty(previousInterest))
            {
                error_method(" CyberBot", $"Based on our previous conversation, you were interested in {previousInterest}. Would you like to learn more about {previousInterest}?");
            }
            else if (!string.IsNullOrEmpty(username))
            {
                error_method(" CyberBot", $"Hi {username}! I don't have any saved interests from you yet. Try saying 'I am interested in passwords' and I'll remember!");
            }
            else
            {
                error_method(" CyberBot", "Please tell me your name first using the name field above!");
            }
        }

        // ADDED METHOD - Clear chat history button handler
        private void clear_chat_button_click(object sender, RoutedEventArgs e)
        {
            chats.Items.Clear();
            ShowAsciiArt();
            error_method(" System", "Chat history cleared! Starting fresh conversation.");
        }

        // ADDED METHOD - Help button handler
        private void help_button_click(object sender, RoutedEventArgs e)
        {
            string helpMessage = @" CYBERSECURITY HELP MENU 
    
 PASSWORD SAFETY: Say 'password' or 'Tell me about passwords'
 PHISHING: Say 'phishing' or 'What is phishing'
 MALWARE: Say 'malware' or 'virus'
 VPN: Say 'vpn' or 'virtual private network'
 BACKUP: Say 'backup' or 'data backup'
 2FA: Say '2fa' or 'two factor authentication'
 RANSOMWARE: Say 'ransomware'

 MEMORY: Say 'I am interested in [topic]' - I'll remember!
 VOICE: Click the microphone button and speak naturally
 RECALL: Say 'What did I like before?' or use Recall button";

            error_method(" CyberBot Help", helpMessage);
        }

        ///////////// END OF ADDED METHODS /////////////

    }
}