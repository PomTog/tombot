using System.Diagnostics;
using WebSocketSharp;
using WebSocketSharp.Server;



namespace Web_socket_server
{
    public class Chat : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)  //when the web socket server recevies a message this function runs
        {
            Console.WriteLine("message from user: " + e.Data);  //shows the data received in the console
            string input = e.Data;     //gets the input from the client and assigns it to "output"
            string trimmed = new string(input.Where(c => !char.IsPunctuation(c)).ToArray());       //removes punctuatuion from the input
            string[] words = trimmed.Split(' ');    //splits each word into its own item in an array.
            string path = @"data.txt";      //the path to the text file
            Array.Sort(words);      //sorts the input array alphabetically
            words = Array.ConvertAll(words, x => x.ToLower());      //converts all characters to lowercase
            List<string> responces = new List<string>(); //defining the list of responces
            foreach (var item in words)     //for each item in the array "words"
            {
                string responce = binarySearch.Search(item, path);    //finds the currently selected item in the array and brings it to a variable
                responce = responce[(responce.Split()[0].Length + 1)..];    //removes the keyword from the line
                responces.Add(responce);
            }
            responces.Sort(); //sorts the array in ascending order
            responces.Reverse(); //reverses the order of the sort
            string FinalResponce = responces[0]; //selects the first row of the list
            FinalResponce = FinalResponce[(FinalResponce.Split()[0].Length + 1)..]; //removes the weight from the front
            Send(FinalResponce); //outputs the responce to the client
            Console.WriteLine("output to user: " + FinalResponce);  //records the responce to the console
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var webSocketServer = new WebSocketServer("ws://127.0.0.1:5963");

            webSocketServer.AddWebSocketService<Chat>("/Chat");  //specifies the chat function for the web socket

            webSocketServer.Start();  //starts the web socket
            Console.WriteLine("Websocket has begun");  //informing the user that the websocket is working
            using Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "UI 2.0.html";  //the path to the web UI
            fileopener.Start();  //opening the web UI
            Process.GetCurrentProcess().WaitForExit(); //maintains the operation of the program

            webSocketServer.Stop(); //closes the websocket when the user closes the connection
        }
    }

    public class binarySearch   //defines a new class called binarySearch
    {
        public static string Search(string target, string path)     //new method called search which needs the parameters target and path
        {
            int start = 0;      //makes a variable called start
            int end = File.ReadAllLines(path).Length;   //finds the ammount of lines in the file
            bool found = false;
            string line = " ";
            string actualLine = " ";

            while ((start <= end) && (found != true))   //while found is false and start is less then end then the while loop will run
            {
                int mid = (start + end) / 2;    //calculates the middle between start and end
                line = File.ReadLines(path).Skip(mid - 1).Take(1).First();      //reads the middle line
                line = line.ToLower();      //comverts it to lower case
                actualLine = line;
                string[] words = line.Split(' ');   //splits line into an array of words
                line = words[0]; //selects the first word
                target = target.ToLower();  //converts the target to lower case
                int compare = string.Compare(line, target);     //compares the target to line depending on the result the 

                if (compare == 1)   //if the compare variable is equal to 1 then end will equal mid - 1
                {
                    end = mid - 1;
                }

                else if (compare == -1)
                {
                    start = mid + 1;
                }

                else
                {
                    found = true;
                }


            }

            if (found == false)     //while found is equal to false the function will return a blank string
            {
                actualLine = "keyword 1 hmm sorry i dont know what you mean by that";   //The default responce if a match is not found
            }
            return actualLine; //returns the line which was found or not
        }
    }
}