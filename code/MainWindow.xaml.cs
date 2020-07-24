using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SquibTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameData gameData = new GameData();

        public int rows = 8;
        public int columns = 8;

        public MainWindow()
        {
            InitializeComponent();

            gameData.PopulateObjectList(this);
            Reset();

        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //TODO: Future work to make window resizeable by dragging
        }

        private void ProcessObjectName(Button button, string arg1)
        {
            foreach (GameData.ObjectDef def in gameData.objectList)
            {
                if(button == def.button)
                {
                    ProcessObjectName(def.name, arg1);
                    break;
                }
            }
        }

        private void ProcessObjectName(string objName, string arg1)
        {
            System.Diagnostics.Debug.WriteLine("ProcessObjectName " + objName + " " + arg1);
            foreach (GameData.ObjectDef def in gameData.objectList)
            {
                bool isObjectFound = false;
                int nameItr = 0;

                if (def.nameList.Count > 0)
                {
                    if (def.nameList[0][0] != "")
                    {
                        for (int i = 0; i < def.nameList.Count; i++)
                        {
                            if (def.nameList[i][0] == arg1 && def.nameList[0][0] == objName)
                            {
                                isObjectFound = true;
                                nameItr = i;
                                break;
                            }
                        }

                        if (isObjectFound == false)
                        {
                            for (int i = 0; i < def.nameList.Count; i++)
                            {
                                if (def.nameList[i][0] == objName)
                                {
                                    isObjectFound = true;
                                    nameItr = i;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (isObjectFound)
                {
                    if (arg1 == "off") { ChangeButtonImage(def, objName, "off", nameItr); }
                    else if (arg1 == "") { ChangeButtonImage(def, objName, "on", nameItr); }
                    else { ChangeButtonImage(def, objName, arg1, nameItr); }
                    break;
                }
            }

        }
        private void ChangeButtonImage(GameData.ObjectDef def, string objName, string arg1, int nameItr )
        {
            System.Diagnostics.Debug.WriteLine("ChangeButtonImage " + objName + " " + arg1);

            int onImgNum = Int32.Parse(def.nameList[nameItr][1]);
            int offImgNum = Int32.Parse(def.nameList[nameItr][2]);

            switch (arg1)
            {
                case "cycle":
                    if(def.IsOn == false)
                    {
                        def.IsOn = true;
                        def.SwitchToOnImage(def.Number);
                    }
                    else
                    {
                        def.IncreaseNumber();
                        def.IncreaseOffNumber();
                        def.SwitchToOnImage(def.Number);
                    }
                    break;
                case "cycle_off":
                    if (def.IsOn == false)
                    {
                        def.IncreaseNumber();
                        def.IncreaseOffNumber();
                        def.SwitchToOffImage(def.OffNumber);
                    }
                    else
                    {
                        def.IsOn = false;
                        def.SwitchToOffImage(def.OffNumber);
                    }
                    break;
                case "cycle_invert":
                    if (def.IsOn == false)
                    {
                        def.IsOn = true;
                        def.DecreaseNumber();
                        def.SwitchToOnImage(def.Number);
                    }
                    else
                    {
                        def.DecreaseNumber();
                        def.DecreaseOffNumber();
                        def.SwitchToOnImage(def.Number);
                    }
                    break;
                case "toggle":
                    if (def.button.Content == def.onImageList[onImgNum])
                    {
                        def.SwitchToOffImage(offImgNum);
                    }
                    else
                    {
                        def.SwitchToOnImage(onImgNum);
                    }
                    break;
                case "on":
                    def.IsOn = true;
                    def.SwitchToOnImage(onImgNum);
                    break;
                case "cleared":
                case "done":
                case "completed":
                case "fin":
                case "finished":
                case "complete":
                case "clear":
                    def.IsOn = true;
                    onImgNum = def.Number;
                    def.SwitchToOnImage(onImgNum);
                    break;
                case "off":
                case "remove":
                    def.IsOn = false;
                    def.SwitchToOffImage(offImgNum);
                    def.Number = offImgNum;
                    break;
                default:
                    if (arg1 != "")
                    {
                        def.Number = (byte)onImgNum;
                        def.SwitchToOnImage(onImgNum);
                    }
                    break;
            }
        }

        private void Reset()
        {
            foreach (GameData.ObjectDef def in gameData.objectList)
            {
                def.SetNumber(0);
                ChangeButtonImage(def, def.name, "off", 0);
            }
        }

        public void ClickToggle(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ProcessObjectName(b, "toggle");
        }

        public void ClickCycle(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ProcessObjectName(b, "cycle");
        }

        public void ClickCycleOff(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ProcessObjectName(b, "cycle_off");
        }

        public void ClickCycleInvert(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ProcessObjectName(b, "cycle_invert");
        }

        public void ClickOff(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ProcessObjectName(b, "off");
        }

        public void ClickOn(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ProcessObjectName(b, "on");
        }
    }
}
