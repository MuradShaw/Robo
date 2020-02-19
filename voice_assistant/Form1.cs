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

//ROBO (BASE)

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

		string ourDirectory;
		string temp;
		string condition;
		string high;
		string low;
		string finalSearch = "";
		Boolean wake = false;
		public Boolean search = false;

		//Start up
		public Form1()
		{
			//Get current dir
			ourDirectory = Directory.GetCurrentDirectory();

			//Let's set some things up
			wake = false;
			search = false;
			list.Add(File.ReadAllLines(@"C:\Users\fruit\OneDrive\Documents\Visual Studio 2017\VoiceBotCommands\commands.txt"));

			Console.WriteLine(Directory.GetCurrentDirectory());

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

		//Some functions
		public void restart()
		{
			Process.Start(@"C:\Users\fruitbot\fruitbot");
			Environment.Exit(0);
		}
		public void shutdown()
		{
			Process.Start("shutdown", "/s /t 0");
		}

		//For sending a message through the speaker
		public void say(String message)
		{
			s.Speak(message);
			wake = false;
		}

		//This all needs to get fixed:
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
			//Get detected words
			String input = e.Result.Text;

			//Google searching
			inputField.Text = input + "\n";

			//Did we finish our google search?
			if (input == "finished" && search == true)
			{
				Process.Start("https://www.google.com/#q=" + finalSearch);
				finalSearch = "";

				inputField.Text = finalSearch + "\n";
				search = false;
			}
			if (search) //If not, continue building the google search
			{
				finalSearch = finalSearch + " " + input;
			}

			//Rng stuff?
			Random rnd = new Random();

			//WAKE ME UP INSIDE
			//ToDo: custom wake up calls for Pro
			if (input == "hey robo") wake = true;

			if (!wake)
				return;

			//SYSTEM RELATED:
			//	Volume up/down/mute
			double volume = defaultPlaybackDevice.Volume;
			if (input == "turn it up")
				defaultPlaybackDevice.Volume = volume + 20;
			else if (input == "turn it down")
				defaultPlaybackDevice.Volume = volume - 20;
			else if (input == "mute")
				defaultPlaybackDevice.Mute(true);
			else if (input == "unmute")
				defaultPlaybackDevice.Mute(false);

			//	Restart/Shutdown
			if (input == "restart")
				restart();
			if (input == "shut down")
				shutdown();

			//FETCHING
			//	Initiating google searches
			if (input == "search for" || input == "google")
				search = true;

			//	Get weather (w/ google search because base)
			if (input == "whats the weather like")
				Process.Start("https://www.google.com/#q=" + "weather");

			// Get time
			else if (input == "what time is it")
				say(DateTime.Now.ToString("h::mm tt"));

			//	Get date
			else if (input == "whats todays date")
				say(DateTime.Now.ToString("M/d/yyyy"));

			//OPENING APPLICATIONS
			// Open Chrome
			else if (input == "open chrome" || input == "open google")
				Process.Start("https://www.google.com/");

			//	Open youtube
			else if (input == "open you tube")
				Process.Start("https://www.youtube.com/");
			
			//MISC.
			//	Flip a coin
			else if (input == "flip a coin")
			{
				int neither = rnd.Next(1, 100);

				if (neither == 50)
				{
					say("Coin landed on its side");
					return;
				}

				int response = rnd.Next(1, 2);

				if (response == 1)
					say("Coin landed on Heads");
				else if (response == 2)
					say("Coin landed on Tails");
			}

			//	Roll a die
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

			//	Special thanks
			else if (input == "thank you")
			{
				int response = rnd.Next(1, 3);

				if (response == 1)
					say("no problem");
				else if (response == 2)
					say("any time");
				else if (response == 3)
					say("you're welcome");
			}
		}
	}
}
