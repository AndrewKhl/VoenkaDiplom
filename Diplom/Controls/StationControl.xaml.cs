﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diplom.Models
{
    /// <summary>
    /// Interaction logic for StationControl.xaml
    /// </summary>
    public partial class StationControl : UserControl, IFocusable
    {
        private static Uri ImageUri { get; } = new Uri("pack://application:,,,/Resources/Canvas/pdh_relay.png");
		public DataStation Data;
        public ConnectionLine firstLine;
        public ConnectionLine secondLine;

		public StationControl(WorkWindow window, string name, int number, Color color)
        {
            InitializeComponent();
            (Resources["fontColor"] as SolidColorBrush).Color = color;
            image.Source = new BitmapImage(ImageUri);
            BorderThickness = new Thickness(2);
			Data = new DataStation();

            stationName.Text = $"{name} [{number}]";
			Data.Name = name;
			Data.Number = number;

			DataNetwork.Stations.Add(this);

            this.window = window;

			BitmapImage gauge = new BitmapImage();
			gauge.BeginInit();
			gauge.UriSource = new Uri("pack://application:,,,/Resources/gauge_1_30.png");
			gauge.EndInit();
			stationGauge.Source = gauge;
			stationGauge.Visibility = Visibility.Hidden;
		}

        public WorkWindow window { get; }

		public event Action FocusedElement;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            SetFocusBorder();
            window.SetFocus(this);
            e.Handled = true;
        }

        public void SetFocusBorder()
        {
            stationImageBorder.BorderBrush = new SolidColorBrush(Colors.White);
            stationNameBorder.BorderBrush = new SolidColorBrush(Colors.White);
            stationImageBorder.Background.Opacity = 0.5;
            stationNameBorder.Background.Opacity = 0.5;
            FocusedElement?.Invoke();
        }

        public void UnsetFocusBorder()
        {
            stationImageBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            stationNameBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            stationImageBorder.Background.Opacity = 0.2;
            stationNameBorder.Background.Opacity = 0.2;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData("Station", this);
                data.SetData("shiftX", e.GetPosition(this).X);
                data.SetData("shiftY", e.GetPosition(this).Y);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

		public void ShowParametrWindow(object sender, RoutedEventArgs e)
		{
			ParamsWindow wnd = new ParamsWindow(this);
			wnd.Owner = Stock.workWindow;
			wnd.Show();
		}

		private void Connect_Click(object sender, RoutedEventArgs e)
		{
			window.ConnectControls(this);
		}
    }
}
