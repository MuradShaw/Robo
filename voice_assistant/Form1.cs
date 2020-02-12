using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;
using System.Xml;
using System.IO;
using AudioSwitcher.AudioApi.CoreAudio;

//CAN DO:
//Google things
//Tell the time
//Give the date
//Shut down pc on command
//Change volume

//TODO:
//Open/close a large list of programs on command
//Play youtube videos seamlessly with only your voice
//Set schedules
//Give the weather
//Possible gamemode (?)
//
//Artifical learning to help structure sentences:
//FOR EXAMPLE: Search for food
//can be easily autofilled with "places near me" with learning
//algarithms
//Listen for specific keywords before being called
namespace voice_assistant
{
	public partial class Form1 : Form
	{
		CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
		SpeechSynthesizer s = new SpeechSynthesizer();
		Choices list = new Choices();
		Choices response = new Choices();
		Boolean listening = false;
		int awaitResponse = 0;
		SpeechRecognitionEngine rec = new SpeechRecognitionEngine();

		string temp;
		string condition;
		string high;
		string low;
		string finalSearch = "";
		Boolean wake = false;
		public Boolean search = false;

		public Form1()
		{
			wake = false;
			search = false;
			list.Add(File.ReadAllLines(@"C:\Users\fruit\OneDrive\Documents\Visual Studio 2017\VoiceBotCommands\commands.txt"));

			//Responses: UNUSED
			response.Add(new string[] { "office" });
			
			Grammar grammar = new Grammar(new GrammarBuilder(list));

			s.SelectVoiceByHints(VoiceGender.Female);

			try
			{
				rec.RequestRecognizerUpdate();
				rec.LoadGrammarAsync(grammar);
				rec.SetInputToDefaultAudioDevice();
				rec.RecognizeAsync(RecognizeMode.Multiple);
				rec.SpeechRecognized += rec_SpeechRecognized;
			}
			catch
			{
				return;
			}

			InitializeComponent();
		}

		public void say(String message)
		{
			s.Speak(message);
			wake = false;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string[] statements = new string[]
			{ "hello world" };
			
		}

		public void restart()
		{
			Process.Start(@"C:\Users\fruitbot\fruitbot");
			Environment.Exit(0);
		}
		public void shutdown()
		{
			Process.Start("shutdown", "/s /t 0");
		}

		//Taken:
		public String GetWeather(String input)
		{
			String query = String.Format("https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text='rockledge, pa')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
			XmlDocument wData = new XmlDocument();
			try
			{
				wData.Load(query);
			}
			catch
			{
				say("An error has occured occurred while trying to get the weather. Check if your device is connected to the internet and try again");
				return "Error, no internet connection";
			}
			

			XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
			manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

			XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
			XmlNodeList nodes = wData.SelectNodes("query/results/channel");
			try
			{
				temp = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;
				condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
				high = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["high"].Value;
				low = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["low"].Value;

				if (input == "temp")
				{
					return temp;
				}
				if (input == "high")
				{
					return high;
				}
				if (input == "low")
				{
					return low;
				}
				if (input == "cond")
				{
					return condition;
				}
			}
			catch
			{
				return "Error Reciving data";
			}
			return "error";
		}

		void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			String input = e.Result.Text;
			List<string> greetings = new List<string> { "hows it going", "whats up", "whats going on", "Want to hear about"};

			inputField.Text = input + "\n";
			if (input == "finished" && search == true)
			{
				Process.Start("https://www.google.com/#q=" + finalSearch);
				finalSearch = "";

				inputField.Text = finalSearch + "\n";
				search = false;
			}
			if (search)
			{
				finalSearch = finalSearch + " " + input;
			}

			Random rnd = new Random();

			if (input == "hey robo") wake = true;

			if (!wake)
				return;

			double volume = defaultPlaybackDevice.Volume;
			if (input == "turn it up")
				defaultPlaybackDevice.Volume = volume + 20;
			else if (input == "turn it down")
				defaultPlaybackDevice.Volume = volume - 20;

			if (input == "restart" || input == "update")
			{
				restart();
			}

			if (input == "shut down")
			{
				shutdown();
			}

			if (input == "search for" || input == "google")
			{
				search = true;
			}
			if (input == "whats the weather like")
			{
				say("the sky is " + GetWeather("cond") + " with a tempature of " + GetWeather("temp") + ". The lows for today is " + GetWeather("low") + " degrees with the highs being " + GetWeather("high"));
			}

			else if (input == "what time is it")
			{
				say(DateTime.Now.ToString("h::mm tt"));
			}
			else if (input == "whats todays date")
			{
				say(DateTime.Now.ToString("M/d/yyyy"));
			}
			else if (input == "open chrome")
			{
				Process.Start("https://www.google.com/");
			}
			else if (input == "open you tube")
			{
				Process.Start("https://www.youtube.com/");
			}
			else if (input == "open my play list")
			{
				Process.Start("https://www.youtube.com/playlist?list=PL2jKQb-296C-VmMDQ8GrKZDFDvUWJ2nbF");
			}
			else if (input == "flip a coin")
			{
				int response = rnd.Next(1, 101);

				if (response == 101)
					say("Coin landed on its side");
				else if (response <= 50)
					say("Coin landed on Heads");
				else if (response > 50 && response <= 100)
					say("Coin landed on Tails");
			}
			else if (input == "roll a die")
			{
				int response = rnd.Next(1, 6);

				switch (response)
				{
					case 1:
						say("Dice rolled on One");
						break;
					case 2:
						say("Dice rolled on Two");
						break;
					case 3:
						say("Dice rolled on Three");
						break;
					case 4:
						say("Dice rolled on Four");
						break;
					case 5:
						say("Dice rolled on Five");
						break;
					case 6:
						say("Dice rolled on Six");
						break;
				}
			}
			else if (input == "thank you")
			{
				int response = rnd.Next(1, 4);

				if (response == 1)
					say("no problem");
				else if (response == 2)
					say("any time");
				else if (response == 3)
					say("you're welcome");
			}

			//last thing to prevent lag
			if (!search)
			{
				for (int i = 0; i < greetings.Count() - 1; i++)
				{
					if (greetings[i].Equals(input))
					{
						switch (i)
						{
							case 0:
								output.Text = "Greeting with the purpose of porpoising a question. Assume subject is asking about me and return accordingly." + "\n";
								break;
							case 1:
								output.Text = "Greeting with the purpose of porpoising a question. Assume subject is asking about me and return accordingly." + "\n";
								break;
							case 2:
								output.Text = "Greeting with the purpose of porpoising a question. Assume subject is asking about me and return accordingly." + "\n";
								break;
							case 3:
								output.Text = "Greeting detected. Subject wants to discuss his topic as a conversation starter." + "\n";
								break;
						}
					}
				}
			}
		}

		private void richTextBox2_TextChanged(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}
	}
}