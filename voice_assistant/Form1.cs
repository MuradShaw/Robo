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
using System.Xml.Linq;
using System.Reflection;

//ROBO (Base)

namespace voice_assistant
{
	public partial class Form1 : Form
	{
		CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
		SpeechSynthesizer s = new SpeechSynthesizer();
		Choices list = new Choices();
		SpeechRecognitionEngine rec = new SpeechRecognitionEngine();

		//Declaration
		Random rnd = new Random();
		string ourDirectory;
		string path;
		int someCounter;

		//Sentance search structure
		string finalSearch = "";

		//Detection variables
		Boolean wake = false;
		public Boolean search = false;

		//Start up
		public Form1()
		{
			//Setting up
			wake = false;
			search = false;
			someCounter = 0;

			//Get paths
			ourDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			path = Path.Combine(ourDirectory, @"commands.txt");

			//Build the commands
			list.Add(File.ReadAllLines(path));
			Grammar grammar = new Grammar(new GrammarBuilder(list));

			//Voice setting
			s.SelectVoiceByHints(VoiceGender.Female);

			//We're ready!
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

		//Restart the application
		public void restart()
		{
			Process.Start(Application.ExecutablePath);
			Environment.Exit(0);
		}

		//Shut down the PC
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

		//Speech detected
		void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			//Get detected words
			String input = e.Result.Text;

			//Google searching
			inputField.Text = input + "\n";
		
			//Did we finish our google search?
			if (input == "finished" && search == true)
			{
				//Do the search
				Process.Start("https://www.google.com/#q=" + finalSearch);
				finalSearch = "";

				inputField.Text = finalSearch + "\n";

				//Wrap it up
				search = false;
				wake = false;
			}
			if (search) //If not, continue building the google search
				finalSearch = finalSearch + " " + input;

			//WAKE ME UP INSIDE
			if (input == "hey robo") wake = true;

			if (!wake)
				return;

			someCounter++;
			Console.WriteLine(someCounter);
			//SYSTEM RELATED:
			// Volume up/down/mute
			double volume = defaultPlaybackDevice.Volume;
			if (input == "turn it up")
				defaultPlaybackDevice.Volume = volume + 20;
			else if (input == "turn it down")
				defaultPlaybackDevice.Volume = volume - 20;
			else if (input == "mute")
				defaultPlaybackDevice.Mute(true);
			else if (input == "unmute")
				defaultPlaybackDevice.Mute(false);

			// Restart/Shutdown
			else if(input == "restart")
				restart();
			else if(input == "shut down")
				shutdown();

			//FETCHING
			// Initiating google searches
			else if(input == "search for" || input == "google")
				search = true;

			// Get weather
			else if(input == "whats the weather like" || input == "weather")
				Process.Start("https://www.google.com/#q=" + "weather");

			// Get time
			else if (input == "what time is it")
				say(DateTime.Now.ToString("h::mm tt"));

			// Get date
			else if (input == "whats todays date")
				say(DateTime.Now.ToString("M/d/yyyy"));

			//OPENING APPLICATIONS
			// Open Chrome
			else if (input == "open chrome" || input == "open google")
				Process.Start("https://www.google.com/");

			// Open youtube
			else if (input == "open you tube")
				Process.Start("https://www.youtube.com/");

			//MISC.
			// Flip a coin
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

			// Roll a die
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

			// Special thanks
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

			//End it
			if (someCounter >= 2 && !search)
			{
				wake = false;
				someCounter = 0;
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			say("Row bo Activated");
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			s.SelectVoiceByHints(VoiceGender.Female);
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			s.SelectVoiceByHints(VoiceGender.Male);
		}
	}
}
