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

//ROBO (Pro)

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
		XDocument doc;
		string ourDirectory;
		string userDocPath;
		string commandPath;

		//Sentance search structure
		string finalSearch = "";

		//Detection variables
		int applicationStuff = 0;
		Boolean wake = false;
		public Boolean search = false;

		//Start up
		public Form1()
		{
			//Setting up
			wake = false;
			search = false;

			//Get paths
			ourDirectory = Directory.GetCurrentDirectory();
			userDocPath = Path.Combine(Directory.GetCurrentDirectory(), @"\user_info.xml");
			commandPath = Path.Combine(Directory.GetCurrentDirectory(), @"\commands.txt");
			doc = XDocument.Load(userDocPath);

			//Build the commands
			list.Add(File.ReadAllLines(commandPath));
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

		//New application added to open/closer
		void addPath(string appName, string path)
		{
			//add to xml
			XElement application = new XElement("app",
				new XAttribute("Name", appName),
				new XAttribute("Path", path));
			doc.Root.Add(application);
			doc.Save(userDocPath);

			//Add to vocab
			using (StreamWriter sw = File.AppendText(commandPath))
				sw.WriteLine(appName);
		}

		//Getting the path of an application
		string getPathOfApp(string applicationName)
		{
			string path = "";

			XElement xmlTree2 = new XElement("application_paths",
				from el in doc.Elements()
				where ((int)el >= 3 && (int)el <= 5)
				select el
			);

			return path;
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
				Process.Start("https://www.google.com/#q=" + finalSearch);
				finalSearch = "";

				inputField.Text = finalSearch + "\n";
				search = false;
			}
			if (search) //If not, continue building the google search
				finalSearch = finalSearch + " " + input;

			//Are we trying to find an application to mess with?
			if (applicationStuff > 0)
			{
				//Couldn't find app
				if (getPathOfApp(input) == null)
				{
					say("Could not find application " + input);
					applicationStuff = 0;

					return;
				}

				//Start application
				if (applicationStuff == 1)
					Process.Start(getPathOfApp(input));
				else //Close application
					foreach (Process proc in Process.GetProcessesByName(input))
						proc.Kill();

				//Feedback
				string yerp = (applicationStuff == 1) ? "Opening" : "Closing";
				say(yerp + " application " + input);

				//Preperation
				applicationStuff = 0;
			}

			//WAKE ME UP INSIDE
			if (input == "hey robo") wake = true;

			if (!wake)
				return;

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
			if (input == "restart")
				restart();
			if (input == "shut down")
				shutdown();

			//FETCHING
			// Initiating google searches
			if (input == "search for" || input == "google")
				search = true;

			// Get weather
			if (input == "whats the weather like")
				Process.Start("https://www.google.com/#q=" + "weather");

			// Get time
			else if (input == "what time is it")
				say(DateTime.Now.ToString("h::mm tt"));

			// Get date
			else if (input == "whats todays date")
				say(DateTime.Now.ToString("M/d/yyyy"));

			//OPENING APPLICATIONS
			//opening something
			else if (input == "open")
				applicationStuff = 1;

			//closing something
			else if (input == "close")
				applicationStuff = 2;

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
		}
	}
}