using System;
using System.Threading;
using System.Threading.Tasks;

namespace ai_chat

{
    public  class EnhancedConsoleChatbot
    {
        public  void SetColors()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
        }

        public  void PrintBorder(char corner = '+', char horizontal = '-', char vertical = '|')
        {
            Console.WriteLine(new string(horizontal, 70));
        }

        public  void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            PrintBorder();
            Console.WriteLine($"|{title,-68}|");
            PrintBorder();
            Console.ResetColor();
        }

        public void PrintSection(string sectionName)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  >>> {sectionName} <<<\n");
            Console.ResetColor();
        }

        public void PrintDivider()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  {new string('─', 66)}");
            Console.ResetColor();
        }

        public  async Task TypewriterEffect(string text, int delayMs = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                await Task.Delay(delayMs);
            }
            Console.WriteLine();
        }

        public  async Task TypingIndicator(int durationMs = 800)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  Bot is typing");
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(durationMs / 3);
                Console.Write(".");
            }
            Console.ResetColor();
            Console.WriteLine();
            await Task.Delay(200);
        }

        public void PrintBotMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[BOT] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        public void PrintUserMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[YOU] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        public void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ {message}");
            Console.ResetColor();
        }

        public void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ {message}");
            Console.ResetColor();
        }
    }
}