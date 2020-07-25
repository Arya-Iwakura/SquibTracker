using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Xml;

namespace SquibTracker
{ 
    public class GameData
    {
        public List<GameData.ObjectDef> objectList;
        public List<GameData.RandomImageDef> randomImageList;
        public List<GameData.RandomTextDef> randomTextList;
        public Random rng = new Random();

        public float winScaleFactor;
        public float winWidth;
        public float winHeight;
        public float winCellSize;
        public string winDataFile;
        public float winFontSize;
        public string winFontVAlign;
        public string winFontHAlign;
        public float winAspectRatio;
        public float currentAspectRatio;

        public bool dataHasBeenRead = false;

        public void ReadXMLData(MainWindow win)
        {
            XmlDocument settingsDoc = new XmlDocument();
            settingsDoc.Load(@".\\assets\\data\\settings.xml");
            ReadSettingFile(settingsDoc, win);

            XmlDocument doc = new XmlDocument();
            doc.Load(winDataFile);

            ReadSettingNodes(doc, win);
            ReadRandomImageSelectionNodes(doc, win);
            ReadRandomTextSelectionNodes(doc, win);
            RandomizeRandomImageList();
            RandomizeRandomTextList();
            ReadLabelNodes(doc, win);
            ReadObjectNodes(doc, win);

            dataHasBeenRead = true;
        }

        public void ReadSettingFile(XmlDocument doc, MainWindow win)
        {
            XmlNodeList settingNodeList = doc.SelectNodes("//settings");
            foreach (XmlNode settingsNode in settingNodeList)
            {
                if (settingsNode.SelectSingleNode(".//scalefactor") != null)
                {
                    winScaleFactor = float.Parse(settingsNode.SelectSingleNode(".//scalefactor").InnerText);
                }

                if (settingsNode.SelectSingleNode(".//datafile") != null)
                {
                    winDataFile = settingsNode.SelectSingleNode(".//datafile").InnerText;
                    winDataFile = ".\\assets\\data\\" + winDataFile;
                }
            }
        }

        public void ReadSettingNodes(XmlDocument doc, MainWindow win)
        {
            int rows = 5;
            int columns = 5;
            //float windowBorderWidth = 16;
            //float windowBorderHeight = 38;
            float windowBorderWidth = 0;
            float windowBorderHeight = 0;
            string colorString = "black";

            XmlNodeList settingNodeList = doc.SelectNodes("//settings");
            foreach (XmlNode settingsNode in settingNodeList)
            {
                if (settingsNode.SelectSingleNode(".//rows") != null)
                {
                    rows = int.Parse(settingsNode.SelectSingleNode(".//rows").InnerText);
                }

                if (settingsNode.SelectSingleNode(".//columns") != null)
                {
                    columns = int.Parse(settingsNode.SelectSingleNode(".//columns").InnerText);
                }

                if (settingsNode.SelectSingleNode(".//background_color") != null)
                {
                    colorString = settingsNode.SelectSingleNode(".//background_color").InnerText;
                }

                if (settingsNode.SelectSingleNode(".//cellsize") != null)
                {
                    winCellSize = float.Parse(settingsNode.SelectSingleNode(".//cellsize").InnerText);
                }

                if (settingsNode.SelectSingleNode(".//fontsize") != null)
                {
                    winFontSize = float.Parse(settingsNode.SelectSingleNode(".//fontsize").InnerText);
                }

                if (settingsNode.SelectSingleNode(".//fontvalign") != null)
                {
                    winFontVAlign = settingsNode.SelectSingleNode(".//fontvalign").InnerText;
                }

                if (settingsNode.SelectSingleNode(".//fonthalign") != null)
                {
                    winFontHAlign = settingsNode.SelectSingleNode(".//fonthalign").InnerText;
                }
            }

            Object color = null;
            try
            {
                color = (Color)ColorConverter.ConvertFromString(colorString);
            }
            catch (Exception ex)
            {
                throw new FormatException($"string {colorString} does not reprsent a valid color", ex);
            }
            if (color == null)
            {
                color = Colors.Black;
            }
            else
            {
                Application.Current.MainWindow.Background = new SolidColorBrush((Color)color);
            }

            if (rows < 1) { rows = 1; }
            if (rows > 100) { rows = 100; }
            if (columns < 1) { columns = 1; }
            if (columns > 100) { columns = 100; }

            winCellSize = (float)(winCellSize * winScaleFactor);
            winFontSize = (float)(winFontSize * winScaleFactor);

            win.rows = rows;
            win.columns = columns;

            CreateGrid(win, rows, columns, winCellSize);

            Application.Current.MainWindow.Height = (rows * winCellSize) + windowBorderHeight;
            Application.Current.MainWindow.Width = (columns * winCellSize) + windowBorderWidth;
            winHeight = (rows * winCellSize) + windowBorderHeight;
            winWidth = (columns * winCellSize) + windowBorderWidth;

            winAspectRatio = winWidth / winHeight;
            currentAspectRatio = winAspectRatio;
        }

        public void CreateGrid(MainWindow win, int inRows, int inColumns, float inCellSize)
        {
            win.maingrid.Margin = new Thickness(0);

            for (int x = 0; x < inColumns; x++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(inCellSize);
                //col.Width = GridLength.Auto;
                win.maingrid.ColumnDefinitions.Add(col);
            }

            for (int y = 0; y < inRows; y++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(inCellSize);
                //row.Height = GridLength.Auto;
                win.maingrid.RowDefinitions.Add(row);
            }

            for (int x = 0; x < inColumns; x++)
            {
                for (int y = 0; y < inRows; y++)
                {
                    Button button = new Button();
                    TextBlock textBlock = new TextBlock();

                    Grid.SetRow(button, y);
                    Grid.SetColumn(button, x);

                    Grid.SetRow(textBlock, y);
                    Grid.SetColumn(textBlock, x);

                    button.Name = "b" + x.ToString() + y.ToString();
                    button.BorderThickness = new Thickness(0);
                    button.Background = new SolidColorBrush(Colors.Transparent);

                    textBlock.Name = button.Name;
                    textBlock.FontSize = winFontSize;
                    textBlock.FontWeight = FontWeights.Bold;
                    textBlock.FontFamily = new FontFamily ("Calibri");

                    if(winFontHAlign == "center")
                    {
                        textBlock.TextAlignment = TextAlignment.Center;
                    }
                    else if(winFontHAlign == "right")
                    {
                        textBlock.TextAlignment = TextAlignment.Right;
                    }
                    else if (winFontHAlign == "justify")
                    {
                        textBlock.TextAlignment = TextAlignment.Justify;
                    }
                    else
                    {
                        textBlock.TextAlignment = TextAlignment.Left;
                    }

                    if (winFontVAlign == "center")
                    {
                        textBlock.VerticalAlignment = VerticalAlignment.Center;
                    }
                    else if (winFontVAlign == "bottom")
                    {
                        textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                    else if (winFontVAlign == "stretch")
                    {
                        textBlock.VerticalAlignment = VerticalAlignment.Stretch;
                    }
                    else
                    {
                        textBlock.VerticalAlignment = VerticalAlignment.Top;
                    }

                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.Background = Brushes.Transparent;
                    textBlock.Foreground = Brushes.White;
                    textBlock.Padding = new Thickness(0);
                    textBlock.Text = "";
                    textBlock.IsHitTestVisible = false;

                    win.maingrid.Children.Add(button);
                    win.maingrid.Children.Add(textBlock);
                }
            }
        }

        public void ReadRandomImageSelectionNodes(XmlDocument doc, MainWindow win)
        {
            XmlNodeList randomSelectionNodeList = doc.SelectNodes("//random_selection_image");
            foreach (XmlNode randomSelectionNode in randomSelectionNodeList)
            {
                RandomImageDef def = new RandomImageDef();

                if (randomSelectionNode.SelectSingleNode(".//image_on") != null)
                {
                    def.onImage = randomSelectionNode.SelectSingleNode(".//image_on").InnerText;
                }

                if (randomSelectionNode.SelectSingleNode(".//image_off") != null)
                {
                    def.offImage = randomSelectionNode.SelectSingleNode(".//image_off").InnerText;
                }

                randomImageList.Add(def);
            }
        }

        public void ReadRandomTextSelectionNodes(XmlDocument doc, MainWindow win)
        {
            XmlNodeList randomSelectionNodeList = doc.SelectNodes("//random_selection_text");
            foreach (XmlNode randomSelectionNode in randomSelectionNodeList)
            {
                RandomTextDef def = new RandomTextDef();

                if (randomSelectionNode.SelectSingleNode(".//text") != null)
                {
                    def.text = randomSelectionNode.SelectSingleNode(".//text").InnerText;
                }

                randomTextList.Add(def);
            }
        }

        public void RandomizeRandomImageList()
        {
            Random rng = new Random();
            List<GameData.RandomImageDef> tempRandomList = new List<GameData.RandomImageDef>();
            int count = randomImageList.Count;
            while (count > 0)
            {
                count--;
                int randomNum = rng.Next(count + 1);
                tempRandomList.Add(randomImageList[randomNum]);
                randomImageList.RemoveAt(randomNum);
            }
            count = tempRandomList.Count;
            while (count > 0)
            {
                count--;
                int randomNum = rng.Next(count + 1);
                randomImageList.Add(tempRandomList[randomNum]);
                tempRandomList.RemoveAt(randomNum);
            }
        }

        public void RandomizeRandomTextList()
        {
            Random rng = new Random();
            List<GameData.RandomTextDef> tempRandomList = new List<GameData.RandomTextDef>();
            int count = randomTextList.Count;
            while (count > 0)
            {
                count--;
                int randomNum = rng.Next(count + 1);
                tempRandomList.Add(randomTextList[randomNum]);
                randomTextList.RemoveAt(randomNum);
            }
            count = tempRandomList.Count;
            while (count > 0)
            {
                count--;
                int randomNum = rng.Next(count + 1);
                randomTextList.Add(tempRandomList[randomNum]);
                tempRandomList.RemoveAt(randomNum);
            }
        }

        public void ReadLabelNodes(XmlDocument doc, MainWindow win)
        {
            XmlNodeList labelNodeList = doc.SelectNodes("//label");
            foreach (XmlNode labelNode in labelNodeList)
            {
                string name = "";
                int row = 0;
                int column = 0;
                string image = "";

                if (labelNode.SelectSingleNode(".//name") != null)
                {
                    name = labelNode.SelectSingleNode(".//name").InnerText;
                }

                if (labelNode.SelectSingleNode(".//row") != null)
                {
                    row = int.Parse(labelNode.SelectSingleNode(".//row").InnerText);
                }

                if (labelNode.SelectSingleNode(".//column") != null)
                {
                    column = int.Parse(labelNode.SelectSingleNode(".//column").InnerText);
                }

                if (labelNode.SelectSingleNode(".//image") != null)
                {
                    image = labelNode.SelectSingleNode(".//image").InnerText;
                }
                if (row < win.rows && column < win.columns)
                {
                    AddImage(win, image, row, column);
                }
            }
        }

        public void AddImage(MainWindow win, string inName, int inRow, int inColumn)
        {
            Image img = new Image();
            BitmapImage bitImg = new BitmapImage();
            bitImg.BeginInit();
            var path = Path.Combine(Environment.CurrentDirectory + inName);
            bitImg.UriSource = new Uri(path);
            bitImg.EndInit();
            img.Source = bitImg;
            img.Stretch = Stretch.Uniform;
            img.Width = winCellSize;
            img.Height = winCellSize;
            img.IsHitTestVisible = false;
            img.Margin = new Thickness(0);
            Grid.SetRow(img, inRow);
            Grid.SetColumn(img, inColumn);
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
            if (img.Source != null)
            {
                win.maingrid.Children.Add(img);
            }
        }

        public void ReadObjectNodes(XmlDocument doc, MainWindow win)
        {
            XmlNodeList objectNodeList = doc.SelectNodes("//object");

            foreach (XmlNode objectNode in objectNodeList)
            {
                ObjectDef def = new ObjectDef();
                def.Setup();
                ReadObjectCoreData(objectNode, ref def, win);
                ReadObjectNames(objectNode, ref def);
                if(def.useRandomTextSelection == true)
                {
                    ReadObjectRandomTextData(objectNode, ref def);
                }
                else
                {
                    ReadObjectTextData(objectNode, ref def);
                }
                if (def.useRandomImageSelection == true)
                {
                    ReadObjectRandomImageData(objectNode, ref def);
                }
                else
                {
                    ReadObjectOnImages(objectNode, ref def);
                    ReadObjectOffImages(objectNode, ref def);
                }
                def.PostLoad();
                objectList.Add(def);
            }
        }

        public void ReadObjectCoreData(XmlNode objectNode, ref ObjectDef def, MainWindow win)
        {
            if (objectNode.SelectSingleNode(".//name") != null)
            {
                def.name = objectNode.SelectSingleNode(".//name").InnerText;
                def.nameList.Add(new string[] { def.name, "0", "0" });
            }

            if (objectNode.SelectSingleNode(".//button") != null)
            {
                def.button = ResolveButtonName(win, objectNode.SelectSingleNode(".//button").InnerText);
                def.textBlock = ResolveTextBlockName(win, objectNode.SelectSingleNode(".//button").InnerText);
            }

            if (objectNode.SelectSingleNode(".//leftclick") != null)
            {
                ReadLeftMouseCLick(objectNode, ref def, win);
            }

            if (objectNode.SelectSingleNode(".//rightclick") != null)
            {
                ReadRightMouseClick(objectNode, ref def, win);
            }

            if (objectNode.SelectSingleNode(".//use_random_text_selection") != null)
            {
                def.useRandomTextSelection = bool.Parse(objectNode.SelectSingleNode(".//use_random_text_selection").InnerText);
            }

            if (objectNode.SelectSingleNode(".//use_random_image_selection") != null)
            {
                def.useRandomImageSelection = bool.Parse(objectNode.SelectSingleNode(".//use_random_image_selection").InnerText);
            }
        }

        public void ReadLeftMouseCLick(XmlNode objectNode, ref ObjectDef def, MainWindow win)
        {
            if (def.button != null)
            {
                switch (objectNode.SelectSingleNode(".//leftclick").InnerText)
                {
                    case "toggle":
                        def.button.Click += new RoutedEventHandler(win.ClickToggle);
                        break;
                    case "on":
                        def.button.Click += new RoutedEventHandler(win.ClickOn);
                        break;
                    case "cycle":
                        def.button.Click += new RoutedEventHandler(win.ClickCycle);
                        break;
                    case "cycle_off":
                        def.button.Click += new RoutedEventHandler(win.ClickCycleOff);
                        break;
                    case "cycle_invert":
                        def.button.Click += new RoutedEventHandler(win.ClickCycleInvert);
                        break;
                    case "off":
                    default:
                        def.button.Click += new RoutedEventHandler(win.ClickOff);
                        break;
                }
            }
        }

        public void ReadRightMouseClick(XmlNode objectNode, ref ObjectDef def, MainWindow win)
        {
            if (def.button != null)
            {
                switch (objectNode.SelectSingleNode(".//rightclick").InnerText)
                {
                    case "toggle":
                        def.button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(win.ClickToggle);
                        break;
                    case "on":
                        def.button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(win.ClickOn);
                        break;
                    case "cycle":
                        def.button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(win.ClickCycle);
                        break;
                    case "cycle_off":
                        def.button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(win.ClickCycleOff);
                        break;
                    case "cycle_invert":
                        def.button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(win.ClickCycleInvert);
                        break;
                    case "off":
                    default:
                        def.button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(win.ClickOff);
                        break;
                }
            }
        }

        public void ReadObjectNames(XmlNode objectNode, ref ObjectDef def)
        {
            XmlNodeList altNameNodes = objectNode.SelectNodes(".//name_alt");

            foreach (XmlNode altNameNode in altNameNodes)
            {
                string name = altNameNode.InnerText;
                string onImageSlot = "0";
                string offImageSlot = "0";
                if (altNameNode.Attributes["onImageSlot"] != null)
                {
                    onImageSlot = altNameNode.Attributes["onImageSlot"].Value;
                }
                if (altNameNode.Attributes["offImageSlot"] != null)
                {
                    offImageSlot = altNameNode.Attributes["offImageSlot"].Value;
                }
                def.nameList.Add(new string[] { name, onImageSlot, offImageSlot });
            }
        }

        public void ReadObjectTextData(XmlNode objectNode, ref ObjectDef def)
        {
            XmlNodeList textNodes = objectNode.SelectNodes(".//text");

            foreach (XmlNode textNode in textNodes)
            {
                string text = textNode.InnerText;
                int slot = 0;
                if (textNode.Attributes["slot"] != null)
                {
                    slot = int.Parse(textNode.Attributes["slot"].Value);
                }
                def.textBlock.Text = objectNode.SelectSingleNode(".//text").InnerText;
            }
        }

        public void ReadObjectRandomTextData(XmlNode objectNode, ref ObjectDef def)
        {
            def.textBlock.Text = randomTextList[0].text;
            randomTextList.RemoveAt(0);
        }

        public void ReadObjectOnImages(XmlNode objectNode, ref ObjectDef def)
        {
            XmlNodeList onImageNodes = objectNode.SelectNodes(".//image_on");

            foreach (XmlNode onImageNode in onImageNodes)
            {
                string name = onImageNode.InnerText;
                int slot = 0;
                if (onImageNode.Attributes["slot"] != null)
                {
                    slot = int.Parse(onImageNode.Attributes["slot"].Value);
                }

                Image img = new Image();
                BitmapImage bitImg = new BitmapImage();
                bitImg.BeginInit();
                var path = Path.Combine(Environment.CurrentDirectory + name);
                bitImg.UriSource = new Uri(path);
                bitImg.EndInit();
                img.Source = bitImg;
                img.Width = winCellSize;
                img.Height = winCellSize;
                img.Stretch = Stretch.Uniform;
                img.Margin = new Thickness(0);
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
                if (img.Source != null)
                {
                    def.onImageList.Add(img);
                }
            }
        }

        public void ReadObjectOffImages(XmlNode objectNode, ref ObjectDef def)
        {
            XmlNodeList offImageNodes = objectNode.SelectNodes(".//image_off");

            foreach (XmlNode offImageNode in offImageNodes)
            {
                string name = offImageNode.InnerText;
                int slot = 0;
                if (offImageNode.Attributes["slot"] != null)
                {
                    slot = int.Parse(offImageNode.Attributes["slot"].Value);
                }

                Image img = new Image();
                BitmapImage bitImg = new BitmapImage();
                bitImg.BeginInit();
                var path = Path.Combine(Environment.CurrentDirectory + name);
                bitImg.UriSource = new Uri(path);
                bitImg.EndInit();
                img.Source = bitImg;
                img.Width = winCellSize;
                img.Height = winCellSize;
                img.Stretch = Stretch.Uniform;
                img.Margin = new Thickness(0);
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);

                if (img.Source != null)
                {
                    def.offImageList.Add(img);
                }
            }
        }

        public void ReadObjectRandomImageData(XmlNode objectNode, ref ObjectDef def)
        {
            string onName = randomImageList[0].onImage;

            Image onImg = new Image();
            BitmapImage onBitImg = new BitmapImage();
            onBitImg.BeginInit();
            var onPath = Path.Combine(Environment.CurrentDirectory + onName);
            onBitImg.UriSource = new Uri(onPath);
            onBitImg.EndInit();
            onImg.Source = onBitImg;

            onImg.Width = winCellSize;
            onImg.Height = winCellSize;
            onImg.Stretch = Stretch.Uniform;
            onImg.Margin = new Thickness(0);
            RenderOptions.SetBitmapScalingMode(onImg, BitmapScalingMode.NearestNeighbor);

            if (onImg.Source != null)
            {
                def.onImageList.Add(onImg);
            }

            string offName = randomImageList[0].offImage;

            Image offImg = new Image();
            BitmapImage offBitImg = new BitmapImage();
            offBitImg.BeginInit();
            var offPath = Path.Combine(Environment.CurrentDirectory + offName);
            offBitImg.UriSource = new Uri(offPath);
            offBitImg.EndInit();
            offImg.Source = offBitImg;

            offImg.Width = winCellSize;
            offImg.Height = winCellSize;
            offImg.Stretch = Stretch.Uniform;
            offImg.Margin = new Thickness(0);
            RenderOptions.SetBitmapScalingMode(offImg, BitmapScalingMode.NearestNeighbor);

            if (offImg.Source != null)
            {
                def.offImageList.Add(offImg);
            }

            randomImageList.RemoveAt(0);
        }

        public void PopulateObjectList(MainWindow win)
        {
            objectList = new List<GameData.ObjectDef>();
            randomImageList = new List<GameData.RandomImageDef>();
            randomTextList = new List<GameData.RandomTextDef>();
            ReadXMLData(win);
        }

        public Button ResolveButtonName(MainWindow win, string inName)
        {
            foreach (Object obj in win.maingrid.Children)
            {
                Button button = obj as Button;
                if (button != null)
                {
                    if (button.Name == inName)
                    {
                        return button;
                    }
                }
            }
            return null;
        }

        public TextBlock ResolveTextBlockName(MainWindow win, string inName)
        {
            foreach (Object obj in win.maingrid.Children)
            {
                TextBlock textBlock = obj as TextBlock;
                if (textBlock != null)
                {
                    if (textBlock.Name == inName)
                    {
                        return textBlock;
                    }
                }
            }
            return null;
        }

        public class ObjectDef
        {
            public string name;
            public Button button;
            //public StackPanel stackPanel;
            public TextBlock textBlock;

            public Brush onBrush;
            public Brush offBrush;

            public bool useRandomTextSelection;
            public bool useRandomImageSelection;
            public List<string[]> nameList;
            public List<Image> onImageList;
            public List<Image> offImageList;
            public List<String> textList;

            private int number;
            private int maxNumber;

            private int offNumber;
            private int offMaxNumber;

            private bool isOn;
            public string message;

            public ObjectDef()
            {
            }

            public void Setup()
            {
                nameList = new List<string[]>();
                onImageList = new List<Image>();
                offImageList = new List<Image>();
                textList = new List<String>();

                onBrush = new SolidColorBrush(Color.FromArgb(255,255,255,255));
                offBrush = new SolidColorBrush(Color.FromArgb(255,69,69,69));

                useRandomTextSelection = false;
                useRandomImageSelection = false;

                number = 0;
                maxNumber = 1;

                offNumber = 0;
                offMaxNumber = 1;

                isOn = false;
            }

            public void PostLoad()
            {
                maxNumber = onImageList.Count;
                offMaxNumber = offImageList.Count;
            }

            public int OnImgCount
            {
                get
                {
                    return onImageList.Count;
                }
            }

            public int OffImgCount
            {
                get
                {
                    return offImageList.Count;
                }
            }

            public int Number
            {
                get
                {
                    return number;
                }
                set
                {
                    number = value;
                }
            }

            public int MaxNumber
            {
                get
                {
                    return onImageList.Count;
                }
                set
                {
                    maxNumber = value;
                }
            }

            public int OffNumber
            {
                get
                {
                    return offNumber;
                }
                set
                {
                    offNumber = value;
                }
            }

            public int OffMaxNumber
            {
                get
                {
                    return offMaxNumber;
                }
                set
                {
                    offMaxNumber = value;
                }
            }

            public void DecreaseNumber()
            {
                number -= 1;
                if (number < 0) { number = maxNumber - 1; }
            }

            public void IncreaseNumber()
            {
                number += 1;
                if (number >= maxNumber) { number = 0; }
            }

            public void SetNumber(byte inNumber)
            {
                number = inNumber;
                if (number < 0) { number = 0; }
                else if (number >= maxNumber) { number = maxNumber; }
            }

            public void DecreaseOffNumber()
            {
                offNumber -= 1;
                if (offNumber < 0) { offNumber = offMaxNumber - 1; }
            }

            public void IncreaseOffNumber()
            {
                offNumber += 1;
                if (offNumber >= offMaxNumber) { offNumber = 0; }
            }

            public bool IsOn
            {
                get { return isOn; }
                set { isOn = value; }
            }

            public void SwitchToOnImage(int imgNum)
            {
                if (button != null)
                {
                    button.Content = onImageList[imgNum];
                    textBlock.Foreground = onBrush;
                }
            }

            public void SwitchToOffImage(int imgNum)
            {
                if (button != null)
                {
                    button.Content = offImageList[imgNum];
                    textBlock.Foreground = offBrush;
                }
            }
        }
        public class RandomImageDef
        {
            public string onImage;
            public string offImage;

            public RandomImageDef()
            {
            }
        }
        public class RandomTextDef
        {
            public string text;

            public RandomTextDef()
            {
            }
        }
    }
}
